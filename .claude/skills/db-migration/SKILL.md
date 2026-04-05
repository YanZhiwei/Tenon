---
name: db-migration
description: NovaClaw 项目专用数据库迁移助手。当需要对数据库做任何结构变更时使用此 skill，包括：新建表、新增列、修改索引、创建视图等。项目同时支持 SQLite（本地开发）和 PostgreSQL（生产环境），迁移代码必须兼容两者。每当提到"加个字段"、"建张表"、"改 schema"、"数据库迁移"、"DB migration"时，务必调用此 skill。
---

# NovaClaw 数据库迁移指南

## 项目概述

NovaClaw 没有使用 Drizzle / Prisma / Knex 等迁移工具，而是采用**内嵌式原始 SQL**方式管理 schema，所有迁移逻辑集中在：

**`packages/platform/src/db-schema.ts`**（约 1700 行）

---

## 核心结构

```
db-schema.ts
├── SCHEMA_SQL          ← SQLite 建表 DDL（CREATE TABLE IF NOT EXISTS ...）
├── SCHEMA_SQL_PG       ← 由 SCHEMA_SQL 自动转换而来（字符串替换）
├── runMigrations(db)   ← SQLite 增量迁移（ALTER TABLE ADD COLUMN...）
├── preMigrationsPg()   ← PG 预迁移（在 SCHEMA_SQL_PG 之前执行的补列）
└── runMigrationsPg()   ← PG 增量迁移（ALTER TABLE ... ADD COLUMN IF NOT EXISTS）
```

**执行顺序（启动时自动触发）：**
- SQLite：`runMigrations(db)` → 执行 `SCHEMA_SQL`
- PostgreSQL：`preMigrationsPg()` → 执行 `SCHEMA_SQL_PG` → `runMigrationsPg()`

---

## SQLite ↔ PostgreSQL 自动转换规则

`SCHEMA_SQL_PG` 通过以下替换从 `SCHEMA_SQL` 生成（`db-schema.ts` 约第 1371 行）：

| SQLite 写法 | PostgreSQL 写法 | 说明 |
|------------|----------------|------|
| `INTEGER PRIMARY KEY AUTOINCREMENT` | `BIGSERIAL PRIMARY KEY` | 自增整型主键 |
| `REAL` | `DOUBLE PRECISION` | 浮点数类型 |
| `datetime('now')` | `NOW()` | 当前时间默认值 |

**其余类型两者通用**，直接写即可：
- `TEXT`、`INTEGER`、`TEXT PRIMARY KEY`（UUID/nanoid 主键）
- `NOT NULL DEFAULT`、`UNIQUE`、`REFERENCES`、`ON DELETE CASCADE`
- `CREATE INDEX IF NOT EXISTS`

---

## 场景一：新建表

### 步骤

**1. 在 `SCHEMA_SQL` 中加建表语句**

在 `SCHEMA_SQL` 变量末尾（约第 490 行之后，`);` 结束符之前）添加：

```sql
-- ═══ 新表名（功能描述） ═══
CREATE TABLE IF NOT EXISTS my_new_table (
  id TEXT PRIMARY KEY,
  tenant_id TEXT NOT NULL REFERENCES tenants(id),
  name TEXT NOT NULL,
  status TEXT NOT NULL DEFAULT 'active',
  created_at TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_my_new_table_tenant ON my_new_table(tenant_id, status);
```

规范：
- 主键优先用 `TEXT PRIMARY KEY`（配合项目的 `generateId()` / nanoid）
- 时间字段用 `TEXT NOT NULL`（项目统一存 ISO 字符串）
- 整型布尔字段用 `INTEGER NOT NULL DEFAULT 0`
- JSON 字段用 `TEXT DEFAULT '{}'` 或 `TEXT DEFAULT '[]'`
- 每张表必须带 `tenant_id`（多租户架构）

**2. 检查 `SCHEMA_SQL_PG` 是否需要特殊处理**

自动替换已覆盖大多数场景。只有以下情况需要手动介入：
- 新表用了 `INTEGER PRIMARY KEY AUTOINCREMENT`（自动转 `BIGSERIAL`，通常不用操心）
- 有 SQLite 特有语法（如 `STRICT`、`WITHOUT ROWID`）——项目目前没用，建议也别用

**3. 无需额外改动 `runMigrations` / `runMigrationsPg`**

新表通过 `CREATE TABLE IF NOT EXISTS` 在初始化时创建，对已有数据库也安全（幂等）。

---

## 场景二：给已有表加字段（最常见）

加字段需要**同时**改 SQLite 和 PG 两处迁移函数，并且也要更新 `SCHEMA_SQL` 让新数据库直接带上该字段。

### 步骤

**1. 在 `SCHEMA_SQL` 对应表的 DDL 中加列定义**（让全新安装的数据库直接包含该列）

**2. 在 `runMigrations(db)` 中加 SQLite 迁移逻辑**

```typescript
// ═══ 在函数末尾添加 ═══
{
  const columns = db.prepare('PRAGMA table_info(my_table)').all() as Array<{ name: string }>;
  const colNames = columns.map((c) => c.name);
  if (!colNames.includes('my_new_column')) {
    db.exec("ALTER TABLE my_table ADD COLUMN my_new_column TEXT");
  }
}
```

注意：SQLite 的 `ALTER TABLE ADD COLUMN` **不支持 `IF NOT EXISTS`**，必须用 `PRAGMA table_info` 手动检查。

**3. 在 `runMigrationsPg(pool)` 中加 PG 迁移逻辑**

```typescript
// 使用已有的 addColumn 辅助函数
await addColumn('my_table', 'my_new_column', 'TEXT');
// 或带默认值：
await addColumn('my_table', 'my_new_column', "TEXT NOT NULL DEFAULT 'default_value'");
```

PostgreSQL 支持 `ADD COLUMN IF NOT EXISTS`，`addColumn` 辅助函数已封装好，直接用。

### 示例：完整地给 `employees` 加一个 `priority` 字段

```typescript
// 1. SCHEMA_SQL 中（employees 表 DDL 内加一行）：
//   priority INTEGER NOT NULL DEFAULT 0,

// 2. runMigrations(db) 末尾：
{
  const columns = db.prepare('PRAGMA table_info(employees)').all() as Array<{ name: string }>;
  if (!columns.map(c => c.name).includes('priority')) {
    db.exec('ALTER TABLE employees ADD COLUMN priority INTEGER NOT NULL DEFAULT 0');
  }
}

// 3. runMigrationsPg(pool) 末尾：
await addColumn('employees', 'priority', 'INTEGER NOT NULL DEFAULT 0');
```

---

## 场景三：新建表，但表本身在首次启动时可能不存在（Migration 中建表）

某些在 `runMigrations` 中新增的表（而非 `SCHEMA_SQL` 中的核心表），需要在 PG 的 `runMigrationsPg` 里也显式 `CREATE TABLE IF NOT EXISTS`。

**参考模式（db-schema.ts 约第 1456 行）：**

```typescript
// runMigrationsPg 中：
await pool.query(`
  CREATE TABLE IF NOT EXISTS my_optional_table (
    id TEXT PRIMARY KEY,
    tenant_id TEXT NOT NULL,
    ...
  );
  CREATE INDEX IF NOT EXISTS idx_my_optional_table_tenant ON my_optional_table(tenant_id);
`);
```

---

## 场景四：`preMigrationsPg` 的使用时机

只有一种情况需要用 `preMigrationsPg`：**新列被 `SCHEMA_SQL_PG` 中的索引引用，但该列是后来才加的**。

例如，如果在 `SCHEMA_SQL` 里给 `artifacts` 表的索引加了 `employee_id` 列，但这列是后来才加的，则需要在 `preMigrationsPg` 里先补列，确保执行 `SCHEMA_SQL_PG`（含该索引）之前列已存在。

普通加列不需要 `preMigrationsPg`，直接在 `runMigrationsPg` 里加即可。

---

## 类型速查表

| 数据含义 | SQLite 写法 | PG 写法（自动转换） |
|---------|-----------|-----------------|
| 文本/UUID主键 | `TEXT PRIMARY KEY` | `TEXT PRIMARY KEY`（不变） |
| 自增整型主键 | `INTEGER PRIMARY KEY AUTOINCREMENT` | 自动→ `BIGSERIAL PRIMARY KEY` |
| 普通整数 | `INTEGER` | `INTEGER`（不变） |
| 浮点数 | `REAL` | 自动→ `DOUBLE PRECISION` |
| 文本 | `TEXT` | `TEXT`（不变） |
| 布尔（项目惯例） | `INTEGER NOT NULL DEFAULT 0` | `INTEGER NOT NULL DEFAULT 0`（不变） |
| JSON（项目惯例） | `TEXT DEFAULT '{}'` | `TEXT DEFAULT '{}'`（不变） |
| 时间戳（项目惯例） | `TEXT NOT NULL` | `TEXT NOT NULL`（不变） |
| 当前时间默认值 | `DEFAULT (datetime('now'))` | 自动→ `DEFAULT NOW()` |

---

## 常见陷阱

1. **只改了 SQLite，忘了改 PG**（或反之）：每次加列，两处必须同步。
2. **SQLite 中对已有表用 `NOT NULL` 且没有 `DEFAULT`**：SQLite 会报错，必须提供 DEFAULT 值。
3. **在 `SCHEMA_SQL` 中使用 SQLite 特有函数**：如 `strftime`、`json_extract`，PG 不支持，要么避免用在 DDL 默认值里，要么在 `SCHEMA_SQL_PG` 里手动替换。
4. **迁移顺序**：`runMigrationsPg` 里的 `addColumn` 调用是顺序执行的，先加的列可以被后加的外键或索引引用，保持合理顺序。
5. **PG 的 `TEXT` 外键**：PG 不会自动建外键索引，如果 `REFERENCES` 的列查询频繁，记得加索引。

---

## 修改完成后的验证

```bash
# 本地 SQLite — 直接启动验证
pnpm dev:serve

# 生产 PostgreSQL — 设置 DATABASE_URL 后启动
DATABASE_URL=postgres://... pnpm dev:serve

# 如需跑 PG schema 测试（需要本地 PG）
pnpm test:pg
```

启动时若 schema 出错，会在终端直接报错，容易定位。

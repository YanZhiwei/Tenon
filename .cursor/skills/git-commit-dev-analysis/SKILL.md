---
name: git-commit-dev-analysis
description: >-
  基于真实 Git 输出做提交与 PR/MR 相关工作：对比分支、生成 PR 标题与描述、起草/润色
  commit message、从区间提交整理 changelog 或发布说明、log/blame/range 分析。只要用户提到
  git commit、提交信息、待推送的改动、开 PR、写 PR 描述、Merge Request、代码评审说明、
  和 main/develop 差多少、两个分支之间改了什么、发布前变更摘要、最近谁改了这块、
  changelog、release notes、合并前检查，就必须使用本 skill。用户使用 `/commit` 或 `/pr` 时，
  在取得明确确认后可代为执行 `git add`/`commit`/`push`、建分支、`gh pr create`/`glab mr create`。
  本地若无 GitHub/GitLab CLI，仍用 git 取证；有 `gh`/`glab` 时可辅助创建 PR，但须与 `git` 结果核对。
---

# Git：提交、PR 与演进分析

## 目标

先用 `git`（及可选 `gh` / `glab`）**取证**，再写 PR 描述、commit message、变更摘要或风险点。禁止凭记忆编造提交哈希、作者或提交说明。

当用户使用 **`/commit`**、**`/pr`** 或明确说「帮我提交 / 推送 / 开 PR」时，在**每一步写入远端或改写仓库状态之前**展示将执行的命令与影响范围，并取得用户**明确口头确认**后再执行（见下节）。未确认前只做只读命令与文案草稿。

## 经确认后的执行（/commit、/pr）

1. **确认门槛**：`git add`、`git commit`、`git push`、创建分支、`gh pr create` / `glab mr create` 均属写入类操作。必须先列出拟执行命令与关键参数（暂存路径、message、远程与分支、PR base/title/body），用户回复如「确认」「执行」「可以」后再运行。**不得**默认用户已同意。
2. **推送与 PR**：先 `push`（常用 `git push -u origin <branch>`），再创建 PR/MR；若未装 `gh`/`glab`，给出网页创建步骤与可复制标题/正文。
3. **安全**：除非用户明确要求，不执行 `reset --hard`、`push --force`、`push --force-with-lease`。
4. **多行 commit message**：优先 `git commit -F file`，避免 shell 引号问题（含 Windows）。

## 基本原则

1. **先取证，后结论**：分析历史或差异前在仓库内执行 `git`（或采用用户粘贴的完整输出）。
2. **范围写清楚**：默认与用户确认或明确写出——基线分支（如 `main`/`develop`）、对比端（当前 `HEAD` 或指定分支）、时间或 tag 区间、是否包含 merge commit、关注路径。
3. **安全**（与上节一致）：不擅自改写历史或强推。
4. **约定优先**：若存在 CONTRIBUTING、commitlint、`.gitmessage`、`.github/pull_request_template.md` 等，优先遵守。

## 何时用本 skill（与相近场景）

| 场景 | 典型诉求 | 取证要点 |
|------|----------|----------|
| **准备 PR / MR** | 标题、描述、Review 说明 | `git log BASE..HEAD`、`git diff BASE...HEAD`、按文件/模块归类 |
| **写 commit message** | 单次提交说明 | `git status`、`git diff`、`git diff --cached` |
| **合并前自检** | 能否合、风险点 | `--stat`、关注迁移/配置/锁文件、大删改 |
| **发布 / changelog** | 某版本以来改了什么 | `git log TAG..HEAD`、`--no-merges` 等按团队习惯 |
| **溯源** | 谁改的、何时、上下文 | `git blame`、`git log -p --follow -- path` |
| **变基/改历史后对比** | 和远端差异是否一致 | `git range-diff`（若适用） |

与「纯代码搜索」的分工：Git 回答**何时、谁、提交粒度**；理解**当前实现**需结合读文件与搜索。

## 工作流 A：PR / Merge Request（常用）

1. **确定 BASE**：常见为远程默认分支（`origin/main`）；多分支模型可能是 `develop`。可用 `git symbolic-ref refs/remotes/origin/HEAD` 或询问用户。
2. **提交列表**（合并基常识：看「进了 PR 的提交」）：
   ```bash
   git fetch origin
   git log --oneline origin/BASE..HEAD
   ```
3. **文件与体量**：
   ```bash
   git diff origin/BASE...HEAD --stat
   git diff origin/BASE...HEAD -- path/of/interest
   ```
4. **可选 CLI 元数据**（在已登录且仓库已关联远端时）：
   - GitHub：`gh pr view`、`gh pr diff`（仍以 `git` 结果为准交叉核对）
   - GitLab：`glab mr view` 等
5. **交付物建议**：
   - **标题**：一句说清「做什么 + 影响范围」，与团队前缀规范一致（如 `feat(api): ...`）。
   - **正文**：动机 / 主要变更点（可对应提交分组）/ 如何验证 / 破坏性变更或回滚注意 / 关联 issue（若用户提及）。
   - **Review 注释**：难读 diff、需重点看的路径、假设与未决问题。
6. **`/pr` 落地**：用户确认后执行 `git push -u` 与 `gh pr create` / `glab mr create`（或改网页创建）；流程细节以 `.cursor/commands/pr.md` 为准。

## 工作流 B：Commit message

1. 用 `git status` 与 `git diff`（及 `--cached`）对齐**实际改动**再写。
2. Subject：祈使、约 50 字符内、单主题；body 写动机、边界情况、关联单号。
3. 改动混杂时优先建议拆分提交；若必须单次提交，在 body 分条列出但仍建议拆分。
4. **`/commit` 落地**：用户确认后再 `git add` / `git commit`；多行正文优先 `git commit -F`；推送须再次确认。细节以 `.cursor/commands/commit.md` 为准。

## 工作流 C：历史、blame、changelog / 发布说明

- **最近发生了什么**：`git log --oneline -n`、`--since=`、`git shortlog -sn`
- **单文件历史**：`git log --oneline -- path`、`git log -p --follow -- path`
- **行级归属**：`git blame -L start,end -- path`
- **Changelog 片段**（示例）：
  ```bash
  git log v1.0.0..HEAD --pretty=format:"- %s (%h)" --no-merges
  ```
  按「新增 / 修复 / 破坏性 / 其他」归类；merge 是否保留按团队约定。

## 交付物模板（按需选用）

### PR 描述（Markdown 骨架）

```markdown
## 摘要
<!-- 一句话 + 类型（feat/fix/refactor/docs/chore） -->

## 变更说明
<!-- 分点或按模块；重要 commit 可带 hash -->

## 如何验证
<!-- 命令、页面、配置步骤 -->

## 风险与回滚
<!-- 破坏性变更、数据迁移、feature flag -->

## 关联
<!-- Closes #123 / 相关 MR -->
```

### Commit message 草稿

- Subject 一行；Body 可选；用户要双语时可附中英文。

### 历史/演进简报

- 范围（分支或时间与 tag）
- 主题分组 + 代表 hash
- 测试与回归关注点

## 需要追问用户的情况

- 未给出 BASE 分支或「最近」的具体时间范围。
- 多个长期分支并存，默认合入目标不明。
- 是否包含 merge commit、子模块、LFS、大文件重命名等特殊项。

## 与代码搜索的配合

先 `git log` / `git show` / `git diff` 定位变更与动机，再跳到当前代码阅读实现，避免只看历史不看现状。

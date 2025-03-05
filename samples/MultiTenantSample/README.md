# Tenon 多租户示例项目

这是一个基于 Tenon 框架的多租户示例项目，展示了如何使用 Tenon 的多租户功能实现数据隔离和租户上下文管理。

## 项目结构

- **Controllers**: API 控制器
  - `ProductsController`: 产品相关 API
  - `OrdersController`: 订单相关 API
- **Data**: 数据访问层
  - `MultiTenantDbContext`: 多租户数据库上下文
  - `SeedData`: 种子数据初始化
- **Entities**: 实体类
  - `Product`: 产品实体
  - `Category`: 类别实体
  - `Order`: 订单实体
  - `OrderItem`: 订单项实体
- **Models**: DTO 模型
  - `ProductDto`: 产品数据传输对象
  - `CategoryDto`: 类别数据传输对象
  - `OrderDto`: 订单数据传输对象
  - `OrderItemDto`: 订单项数据传输对象
- **Services**: 业务服务
  - `ProductService`: 产品服务
  - `OrderService`: 订单服务
- **TenantResolvers**: 租户解析器
  - `HttpContextTenantResolver`: HTTP 上下文租户解析器

## 多租户功能

本示例项目展示了以下多租户功能：

1. **自动租户数据隔离**：通过全局查询过滤器实现租户数据隔离
2. **租户上下文管理**：支持动态切换租户上下文
3. **临时禁用租户过滤**：支持临时禁用租户过滤器
4. **与审计功能集成**：多租户与审计功能无缝集成

## 使用方法

### 运行项目

1. 确保已安装 .NET 9 SDK
2. 克隆仓库并进入项目目录
3. 运行以下命令启动项目：

```bash
dotnet run
```

### 测试 API

项目启动后，可以通过 Swagger UI 测试 API，访问地址：`https://localhost:5001/swagger`

### 设置租户 ID

在请求头中设置以下字段：

- `X-Tenant-Id`: 租户 ID（示例中有租户 1 和租户 2）
- `X-User-Id`: 用户 ID（可选，用于审计）

### API 示例

#### 产品 API

- `GET /api/products`: 获取当前租户的所有产品
- `GET /api/products/{id}`: 获取当前租户的指定产品
- `POST /api/products`: 创建产品
- `PUT /api/products/{id}`: 更新产品
- `DELETE /api/products/{id}`: 删除产品
- `GET /api/products/categories`: 获取当前租户的所有类别
- `GET /api/products/tenant/{tenantId}`: 获取指定租户的产品（演示用）
- `GET /api/products/all-tenants`: 获取所有租户的产品（演示用）

#### 订单 API

- `GET /api/orders`: 获取当前租户的所有订单
- `GET /api/orders/{id}`: 获取当前租户的指定订单
- `POST /api/orders`: 创建订单
- `PUT /api/orders/{id}/status`: 更新订单状态
- `PUT /api/orders/{id}/cancel`: 取消订单

## 技术栈

- .NET 9
- ASP.NET Core WebAPI
- Entity Framework Core
- SQLite 数据库
- Tenon 框架

## 多租户实现细节

### 1. 实体定义

所有实体都继承自 `EfTenantEntity` 或 `EfTenantFullAuditableEntity`，自动具备租户隔离能力：

```csharp
public class Product : EfTenantEntity
{
    public string Name { get; set; }
    // ...
}

public class Order : EfTenantFullAuditableEntity
{
    public string OrderNumber { get; set; }
    // ...
}
```

### 2. 租户解析器

通过 `HttpContextTenantResolver` 从 HTTP 请求头中解析租户 ID：

```csharp
public class HttpContextTenantResolver : IEfTenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    // ...
    
    public long? TenantId => GetTenantIdFromHeader();
    
    private long? GetTenantIdFromHeader()
    {
        // 从请求头 X-Tenant-Id 中获取租户 ID
    }
}
```

### 3. 数据库上下文

在 `MultiTenantDbContext` 中应用租户过滤器：

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // 应用租户过滤器
    modelBuilder.ApplyTenantFilter();
}
```

### 4. 服务注册

在 `Program.cs` 中注册多租户服务：

```csharp
// 添加 HttpContextTenantResolver
services.AddHttpContextAccessor();
services.AddScoped<HttpContextTenantResolver>();

// 添加多租户 EF Core 服务
services.AddTenantEfCore<long, long, HttpContextTenantResolver>();
```

### 5. 仓储使用

在服务层使用 `ITenantRepository` 接口操作数据：

```csharp
public class ProductService
{
    private readonly ITenantRepository<Product, long, long> _productRepository;
    
    // ...
    
    public async Task<List<ProductDto>> GetProductsForCurrentTenantAsync()
    {
        var products = await _productRepository.GetListForCurrentTenantAsync();
        // ...
    }
}
```

## 许可证

MIT

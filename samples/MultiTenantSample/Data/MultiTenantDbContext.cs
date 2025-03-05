using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MultiTenantSample.Entities;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Extensions;

namespace MultiTenantSample.Data;

/// <summary>
/// 多租户数据库上下文
/// </summary>
public class MultiTenantDbContext : TenonDbContext
{
    private readonly IEfTenantResolver _tenantResolver;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">数据库上下文选项</param>
    /// <param name="tenantResolver">租户解析器</param>
    public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options, IEfTenantResolver tenantResolver)
        : base(options, tenantResolver)
    {
        _tenantResolver = tenantResolver;
    }
    
    /// <summary>
    /// 获取实体所在程序集
    /// </summary>
    protected override Assembly EntityAssembly => typeof(Product).Assembly;
    
    /// <summary>
    /// 产品
    /// </summary>
    public DbSet<Product> Products { get; set; }
    
    /// <summary>
    /// 类别
    /// </summary>
    public DbSet<Category> Categories { get; set; }
    
    /// <summary>
    /// 订单
    /// </summary>
    public DbSet<Order> Orders { get; set; }
    
    /// <summary>
    /// 订单项
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; }
    
    /// <summary>
    /// 配置模型
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 配置实体映射
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        });
        
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
        });
        
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
        });
        
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);
            
            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);
            
            entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
        });
        
        // 应用租户过滤器
        if (_tenantResolver != null)
        {
            modelBuilder.ApplyTenantFilter(_tenantResolver.TenantId);
        }
    }
}

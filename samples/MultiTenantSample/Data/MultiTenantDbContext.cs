using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MultiTenantSample.Entities;
using Tenon.Repository.EfCore.MultiTenant;

namespace MultiTenantSample.Data;

/// <summary>
/// 多租户数据库上下文
/// </summary>
public class MultiTenantDbContext : TenantDbContext
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">数据库上下文选项</param>
    /// <param name="tenantResolver">租户解析器</param>
    public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options, IEfTenantResolver tenantResolver)
        : base(options, tenantResolver)
    {
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}

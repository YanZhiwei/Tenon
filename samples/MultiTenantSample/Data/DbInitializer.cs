using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiTenantSample.Entities;
using MultiTenantSample.Extensions;
using Tenon.Repository.EfCore;

namespace MultiTenantSample.Data;

/// <summary>
/// 数据库初始化器
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<MultiTenantDbContext>();
            var tenantResolver = services.GetRequiredService<IEfTenantResolver>();
            
            // 确保数据库已创建
            await context.Database.EnsureCreatedAsync();
            
            // 如果没有类别数据，则初始化示例数据
            if (!await context.Categories.AnyAsync())
            {
                await SeedDataAsync(context, tenantResolver);
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<MultiTenantDbContext>>();
            logger.LogError(ex, "初始化数据库时发生错误");
        }
    }
    
    /// <summary>
    /// 添加种子数据
    /// </summary>
    private static async Task SeedDataAsync(MultiTenantDbContext context, IEfTenantResolver tenantResolver)
    {
        // 使用 DisableTenantFilter 来添加多个租户的数据
        using (context.DisableTenantFilter())
        {
            // 租户1的数据
            var tenant1Categories = new[]
            {
                new Category { Name = "电子产品", Description = "各类电子设备", TenantId = 1 },
                new Category { Name = "家居用品", Description = "家庭日常用品", TenantId = 1 }
            };
            
            await context.Categories.AddRangeAsync(tenant1Categories);
            await context.SaveChangesAsync();
            
            var tenant1Products = new[]
            {
                new Product 
                { 
                    Name = "智能手机", 
                    Description = "高性能智能手机", 
                    Price = 3999.00m, 
                    StockQuantity = 100, 
                    CategoryId = tenant1Categories[0].Id,
                    TenantId = 1
                },
                new Product 
                { 
                    Name = "笔记本电脑", 
                    Description = "轻薄笔记本电脑", 
                    Price = 5999.00m, 
                    StockQuantity = 50, 
                    CategoryId = tenant1Categories[0].Id,
                    TenantId = 1
                },
                new Product 
                { 
                    Name = "床上四件套", 
                    Description = "舒适床上用品", 
                    Price = 299.00m, 
                    StockQuantity = 200, 
                    CategoryId = tenant1Categories[1].Id,
                    TenantId = 1
                }
            };
            
            await context.Products.AddRangeAsync(tenant1Products);
            
            // 租户2的数据
            var tenant2Categories = new[]
            {
                new Category { Name = "食品饮料", Description = "各类食品和饮料", TenantId = 2 },
                new Category { Name = "办公用品", Description = "办公室常用物品", TenantId = 2 }
            };
            
            await context.Categories.AddRangeAsync(tenant2Categories);
            await context.SaveChangesAsync();
            
            var tenant2Products = new[]
            {
                new Product 
                { 
                    Name = "矿泉水", 
                    Description = "天然矿泉水", 
                    Price = 2.00m, 
                    StockQuantity = 500, 
                    CategoryId = tenant2Categories[0].Id,
                    TenantId = 2
                },
                new Product 
                { 
                    Name = "巧克力", 
                    Description = "进口巧克力", 
                    Price = 15.00m, 
                    StockQuantity = 1000, 
                    CategoryId = tenant2Categories[0].Id,
                    TenantId = 2
                },
                new Product 
                { 
                    Name = "签字笔", 
                    Description = "黑色签字笔", 
                    Price = 5.00m, 
                    StockQuantity = 300, 
                    CategoryId = tenant2Categories[1].Id,
                    TenantId = 2
                }
            };
            
            await context.Products.AddRangeAsync(tenant2Products);
            await context.SaveChangesAsync();
        }
    }
}

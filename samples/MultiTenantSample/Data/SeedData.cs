using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiTenantSample.Entities;
using MultiTenantSample.Extensions;
using Tenon.Repository.EfCore.Extensions;

namespace MultiTenantSample.Data;

/// <summary>
/// 种子数据初始化
/// </summary>
public static class SeedData
{
    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    public static async Task InitializeAsync(MultiTenantDbContext dbContext)
    {
        // 禁用租户过滤器，以便初始化多个租户的数据
        using (dbContext.DisableTenantFilter())
        {
            // 检查是否已有数据
            if (await dbContext.Categories.AnyAsync())
            {
                return; // 数据库已初始化
            }
            
            // 初始化租户1的数据
            await InitializeTenantDataAsync(dbContext, 1);
            
            // 初始化租户2的数据
            await InitializeTenantDataAsync(dbContext, 2);
            
            // 保存所有更改
            await dbContext.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// 初始化指定租户的数据
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="tenantId">租户ID</param>
    private static async Task InitializeTenantDataAsync(MultiTenantDbContext dbContext, long tenantId)
    {
        // 使用租户上下文
        using (dbContext.ChangeTenant(tenantId))
        {
            // 添加类别
            var categories = new List<Category>
            {
                new Category
                {
                    Name = $"电子产品-租户{tenantId}",
                    Description = "包括手机、电脑、平板等电子设备",
                    TenantId = tenantId
                },
                new Category
                {
                    Name = $"家居用品-租户{tenantId}",
                    Description = "包括家具、装饰品、厨房用品等",
                    TenantId = tenantId
                },
                new Category
                {
                    Name = $"服装-租户{tenantId}",
                    Description = "包括男装、女装、童装等各类服饰",
                    TenantId = tenantId
                }
            };
            
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
            
            // 添加产品
            var products = new List<Product>
            {
                // 电子产品
                new Product
                {
                    Name = $"智能手机-租户{tenantId}",
                    Description = "最新款智能手机，搭载高性能处理器",
                    Price = 4999.00m,
                    StockQuantity = 100,
                    CategoryId = categories[0].Id,
                    TenantId = tenantId
                },
                new Product
                {
                    Name = $"笔记本电脑-租户{tenantId}",
                    Description = "轻薄笔记本电脑，适合办公和娱乐",
                    Price = 6999.00m,
                    StockQuantity = 50,
                    CategoryId = categories[0].Id,
                    TenantId = tenantId
                },
                
                // 家居用品
                new Product
                {
                    Name = $"沙发-租户{tenantId}",
                    Description = "舒适的三人沙发，多种颜色可选",
                    Price = 3999.00m,
                    StockQuantity = 20,
                    CategoryId = categories[1].Id,
                    TenantId = tenantId
                },
                new Product
                {
                    Name = $"餐桌-租户{tenantId}",
                    Description = "实木餐桌，可容纳6人",
                    Price = 2999.00m,
                    StockQuantity = 15,
                    CategoryId = categories[1].Id,
                    TenantId = tenantId
                },
                
                // 服装
                new Product
                {
                    Name = $"男士T恤-租户{tenantId}",
                    Description = "纯棉T恤，舒适透气",
                    Price = 199.00m,
                    StockQuantity = 200,
                    CategoryId = categories[2].Id,
                    TenantId = tenantId
                },
                new Product
                {
                    Name = $"女士连衣裙-租户{tenantId}",
                    Description = "时尚连衣裙，适合春夏季节",
                    Price = 299.00m,
                    StockQuantity = 150,
                    CategoryId = categories[2].Id,
                    TenantId = tenantId
                }
            };
            
            await dbContext.Products.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();
            
            // 添加订单
            var order = new Order
            {
                OrderNumber = $"ORD-{tenantId}-{DateTime.Now.ToString("yyyyMMdd")}-001",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Paid,
                TotalAmount = 0, // 将在添加订单项后计算
                TenantId = tenantId,
                CreatedBy = 1, // 假设用户ID为1
                CreatedAt = DateTimeOffset.Now
            };
            
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            
            // 添加订单项
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = products[0].Id,
                    Quantity = 1,
                    UnitPrice = products[0].Price,
                    TenantId = tenantId
                },
                new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = products[2].Id,
                    Quantity = 2,
                    UnitPrice = products[2].Price,
                    TenantId = tenantId
                }
            };
            
            await dbContext.OrderItems.AddRangeAsync(orderItems);
            await dbContext.SaveChangesAsync();
            
            // 更新订单总金额
            order.TotalAmount = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
            await dbContext.SaveChangesAsync();
        }
    }
}

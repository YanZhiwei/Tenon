using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiTenantSample.Entities;
using MultiTenantSample.Extensions;
using MultiTenantSample.Models;
using Tenon.Repository;

namespace MultiTenantSample.Services;

/// <summary>
/// 订单服务
/// </summary>
public class OrderService
{
    private readonly ITenantRepository<Order, long, long> _orderRepository;
    private readonly ITenantRepository<OrderItem, long, long> _orderItemRepository;
    private readonly ITenantRepository<Product, long, long> _productRepository;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="orderRepository">订单仓储</param>
    /// <param name="orderItemRepository">订单项仓储</param>
    /// <param name="productRepository">产品仓储</param>
    public OrderService(
        ITenantRepository<Order, long, long> orderRepository,
        ITenantRepository<OrderItem, long, long> orderItemRepository,
        ITenantRepository<Product, long, long> productRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
    }
    
    /// <summary>
    /// 获取当前租户的所有订单
    /// </summary>
    /// <returns>订单列表</returns>
    public async Task<List<OrderDto>> GetOrdersForCurrentTenantAsync()
    {
        var orders = await _orderRepository.GetListForCurrentTenantAsync(
            null, 
            query => query.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product));
                
        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TotalAmount = o.TotalAmount,
            CreatedBy = o.CreatedBy,
            CreatedAt = o.CreatedAt,
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        }).ToList();
    }
    
    /// <summary>
    /// 获取当前租户的订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    public async Task<OrderDto> GetOrderByIdAsync(long id)
    {
        var order = await _orderRepository.GetForCurrentTenantAsync(
            o => o.Id == id,
            query => query.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product));
                
        if (order == null)
        {
            return null;
        }
        
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedBy = order.CreatedBy,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
    
    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="createOrderDto">订单信息</param>
    /// <returns>创建的订单ID</returns>
    public async Task<long> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        // 创建订单
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
            TotalAmount = 0, // 将在添加订单项后计算
            CreatedBy = 1, // 默认用户ID
            CreatedAt = DateTimeOffset.Now
        };
        
        await _orderRepository.InsertForCurrentTenantAsync(order);
        
        // 添加订单项
        decimal totalAmount = 0;
        foreach (var item in createOrderDto.Items)
        {
            var product = await _productRepository.GetForCurrentTenantAsync(p => p.Id == item.ProductId);
            if (product == null)
            {
                continue;
            }
            
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };
            
            await _orderItemRepository.InsertForCurrentTenantAsync(orderItem);
            
            totalAmount += orderItem.Subtotal;
        }
        
        // 更新订单总金额
        order.TotalAmount = totalAmount;
        await _orderRepository.UpdateForCurrentTenantAsync(order);
        
        return order.Id;
    }
    
    /// <summary>
    /// 更新订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="status">新状态</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateOrderStatusAsync(long id, OrderStatus status)
    {
        var order = await _orderRepository.GetForCurrentTenantAsync(o => o.Id == id);
        if (order == null)
        {
            return false;
        }
        
        order.Status = status;
        await _orderRepository.UpdateForCurrentTenantAsync(order);
        
        return true;
    }
    
    /// <summary>
    /// 删除订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>是否删除成功</returns>
    public async Task<bool> DeleteOrderAsync(long id)
    {
        var order = await _orderRepository.GetForCurrentTenantAsync(o => o.Id == id);
        if (order == null)
        {
            return false;
        }
        
        // 删除订单项
        var orderItems = await _orderItemRepository.GetListForCurrentTenantAsync(oi => oi.OrderId == id);
        foreach (var item in orderItems)
        {
            await _orderItemRepository.RemoveAsync(item);
        }
        
        // 删除订单
        await _orderRepository.RemoveAsync(order);
        
        return true;
    }
}

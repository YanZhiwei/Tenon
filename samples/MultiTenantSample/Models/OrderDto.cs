using System;
using System.Collections.Generic;
using MultiTenantSample.Entities;

namespace MultiTenantSample.Models;

/// <summary>
/// 订单数据传输对象
/// </summary>
public class OrderDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNumber { get; set; }
    
    /// <summary>
    /// 订单日期
    /// </summary>
    public DateTime OrderDate { get; set; }
    
    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; set; }
    
    /// <summary>
    /// 订单状态名称
    /// </summary>
    public string StatusName => Status.ToString();
    
    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// 创建者ID
    /// </summary>
    public long CreatedBy { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// 订单项列表
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

using System;
using System.Collections.Generic;
using Tenon.Repository.EfCore;

namespace MultiTenantSample.Entities;

/// <summary>
/// 订单实体
/// </summary>
public class Order : EfTenantFullAuditableEntity
{
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
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// 订单项列表
    /// </summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

/// <summary>
/// 订单状态枚举
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待付款
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// 已付款
    /// </summary>
    Paid = 1,
    
    /// <summary>
    /// 已发货
    /// </summary>
    Shipped = 2,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4
}

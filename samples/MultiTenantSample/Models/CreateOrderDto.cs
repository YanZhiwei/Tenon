using System.Collections.Generic;

namespace MultiTenantSample.Models;

/// <summary>
/// 创建订单的数据传输对象
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// 订单项列表
    /// </summary>
    public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
}

/// <summary>
/// 创建订单项的数据传输对象
/// </summary>
public class CreateOrderItemDto
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public long ProductId { get; set; }
    
    /// <summary>
    /// 产品数量
    /// </summary>
    public int Quantity { get; set; }
}

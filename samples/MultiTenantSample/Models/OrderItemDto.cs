namespace MultiTenantSample.Models;

/// <summary>
/// 订单项数据传输对象
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// 订单项ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 产品ID
    /// </summary>
    public long ProductId { get; set; }
    
    /// <summary>
    /// 产品名称
    /// </summary>
    public string? ProductName { get; set; }
    
    /// <summary>
    /// 产品数量
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// 产品单价
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// 小计金额
    /// </summary>
    public decimal Subtotal => Quantity * UnitPrice;
}

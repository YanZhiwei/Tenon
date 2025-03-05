using Tenon.Repository.EfCore;

namespace MultiTenantSample.Entities;

/// <summary>
/// 订单项实体
/// </summary>
public class OrderItem : EfTenantFullAuditableEntity
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public long OrderId { get; set; }
    
    /// <summary>
    /// 产品ID
    /// </summary>
    public long ProductId { get; set; }
    
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
    
    /// <summary>
    /// 订单
    /// </summary>
    public virtual Order Order { get; set; }
    
    /// <summary>
    /// 产品
    /// </summary>
    public virtual Product Product { get; set; }
}

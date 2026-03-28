using Tenon.Repository.EfCore.MultiTenant;

namespace MultiTenantSample.Entities;

/// <summary>
/// 产品实体
/// </summary>
public class Product : EfTenantFullAuditableEntity
{
    /// <summary>
    /// 产品名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 产品描述
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// 产品价格
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// 库存数量
    /// </summary>
    public int StockQuantity { get; set; }
    
    /// <summary>
    /// 类别ID
    /// </summary>
    public long CategoryId { get; set; }
    
    /// <summary>
    /// 类别
    /// </summary>
    public virtual Category Category { get; set; }
}

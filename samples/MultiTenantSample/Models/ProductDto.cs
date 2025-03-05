namespace MultiTenantSample.Models;

/// <summary>
/// 产品数据传输对象
/// </summary>
public class ProductDto
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public long Id { get; set; }
    
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
    /// 类别名称
    /// </summary>
    public string CategoryName { get; set; }
}

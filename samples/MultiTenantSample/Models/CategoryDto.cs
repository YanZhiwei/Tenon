namespace MultiTenantSample.Models;

/// <summary>
/// 类别数据传输对象
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// 类别ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 类别名称
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// 类别描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 产品数量
    /// </summary>
    public int ProductCount { get; set; }
}

using System.Collections.Generic;
using Tenon.Repository.EfCore.MultiTenant;

namespace MultiTenantSample.Entities;

/// <summary>
/// 产品类别
/// </summary>
public class Category : EfTenantFullAuditableEntity
{
    /// <summary>
    /// 类别名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 类别描述
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// 产品列表
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

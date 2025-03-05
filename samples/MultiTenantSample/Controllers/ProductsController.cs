using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiTenantSample.Models;
using MultiTenantSample.Services;

namespace MultiTenantSample.Controllers;

/// <summary>
/// 产品控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="productService">产品服务</param>
    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }
    
    /// <summary>
    /// 获取当前租户的所有产品
    /// </summary>
    /// <returns>产品列表</returns>
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetProductsForCurrentTenantAsync();
        return Ok(products);
    }
    
    /// <summary>
    /// 获取当前租户的指定产品
    /// </summary>
    /// <param name="id">产品ID</param>
    /// <returns>产品信息</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(long id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();
            
        return Ok(product);
    }
    
    /// <summary>
    /// 创建产品
    /// </summary>
    /// <param name="productDto">产品信息</param>
    /// <returns>创建的产品</returns>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
    {
        var id = await _productService.CreateProductAsync(productDto);
        var createdProduct = await _productService.GetProductByIdAsync(id);
        return CreatedAtAction(nameof(GetProduct), new { id }, createdProduct);
    }
    
    /// <summary>
    /// 更新产品
    /// </summary>
    /// <param name="id">产品ID</param>
    /// <param name="productDto">产品信息</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(long id, ProductDto productDto)
    {
        var result = await _productService.UpdateProductAsync(id, productDto);
        if (!result)
            return NotFound();
            
        return NoContent();
    }
    
    /// <summary>
    /// 删除产品
    /// </summary>
    /// <param name="id">产品ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
            return NotFound();
            
        return NoContent();
    }
    
    /// <summary>
    /// 获取当前租户的所有类别
    /// </summary>
    /// <returns>类别列表</returns>
    [HttpGet("categories")]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var categories = await _productService.GetCategoriesForCurrentTenantAsync();
        return Ok(categories);
    }
    
    /// <summary>
    /// 获取指定租户的产品列表（仅用于演示）
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    /// <returns>产品列表</returns>
    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult<List<ProductDto>>> GetProductsForTenant(long tenantId)
    {
        var products = await _productService.GetProductsForSpecificTenantAsync(tenantId);
        return Ok(products);
    }
    
    /// <summary>
    /// 获取所有租户的产品列表（仅用于演示）
    /// </summary>
    /// <returns>产品列表</returns>
    [HttpGet("all-tenants")]
    public async Task<ActionResult<List<ProductDto>>> GetProductsForAllTenants()
    {
        var products = await _productService.GetProductsForAllTenantsAsync();
        return Ok(products);
    }
}

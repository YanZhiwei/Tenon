using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiTenantSample.Entities;
using MultiTenantSample.Extensions;
using MultiTenantSample.Models;
using Tenon.Repository;

namespace MultiTenantSample.Services;

/// <summary>
/// 产品服务
/// </summary>
public class ProductService
{
    private readonly ITenantRepository<Product, long, long> _productRepository;
    private readonly ITenantRepository<Category, long, long> _categoryRepository;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="productRepository">产品仓储</param>
    /// <param name="categoryRepository">类别仓储</param>
    public ProductService(
        ITenantRepository<Product, long, long> productRepository,
        ITenantRepository<Category, long, long> categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }
    
    /// <summary>
    /// 获取当前租户的所有产品
    /// </summary>
    /// <returns>产品列表</returns>
    public async Task<List<ProductDto>> GetProductsForCurrentTenantAsync()
    {
        var products = await _productRepository.GetListForCurrentTenantAsync(
            null,
            query => query.Include(p => p.Category));
            
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name
        }).ToList();
    }
    
    /// <summary>
    /// 获取当前租户的指定产品
    /// </summary>
    /// <param name="id">产品ID</param>
    /// <returns>产品信息</returns>
    public async Task<ProductDto> GetProductByIdAsync(long id)
    {
        var product = await _productRepository.GetForCurrentTenantAsync(
            p => p.Id == id,
            query => query.Include(p => p.Category));
            
        if (product == null)
            return null;
            
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name
        };
    }
    
    /// <summary>
    /// 创建产品
    /// </summary>
    /// <param name="productDto">产品信息</param>
    /// <returns>创建的产品ID</returns>
    public async Task<long> CreateProductAsync(ProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            StockQuantity = productDto.StockQuantity,
            CategoryId = productDto.CategoryId
        };
        
        await _productRepository.InsertForCurrentTenantAsync(product);
        return product.Id;
    }
    
    /// <summary>
    /// 更新产品
    /// </summary>
    /// <param name="id">产品ID</param>
    /// <param name="productDto">产品信息</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateProductAsync(long id, ProductDto productDto)
    {
        var product = await _productRepository.GetForCurrentTenantAsync(p => p.Id == id);
        if (product == null)
            return false;
            
        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.Price = productDto.Price;
        product.StockQuantity = productDto.StockQuantity;
        product.CategoryId = productDto.CategoryId;
        
        await _productRepository.UpdateForCurrentTenantAsync(product);
        return true;
    }
    
    /// <summary>
    /// 删除产品
    /// </summary>
    /// <param name="id">产品ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteProductAsync(long id)
    {
        var product = await _productRepository.GetForCurrentTenantAsync(p => p.Id == id);
        if (product == null)
            return false;
            
        await _productRepository.RemoveAsync(product);
        return true;
    }
    
    /// <summary>
    /// 获取当前租户的所有类别
    /// </summary>
    /// <returns>类别列表</returns>
    public async Task<List<CategoryDto>> GetCategoriesForCurrentTenantAsync()
    {
        var categories = await _categoryRepository.GetListForCurrentTenantAsync(
            null,
            query => query.Include(c => c.Products));
            
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ProductCount = c.Products.Count
        }).ToList();
    }
    
    /// <summary>
    /// 切换到指定租户并获取产品列表（仅用于演示）
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    /// <returns>产品列表</returns>
    public async Task<List<ProductDto>> GetProductsForSpecificTenantAsync(long tenantId)
    {
        using (_productRepository.ChangeTenant(tenantId))
        {
            var products = await _productRepository.GetListAsync(p => true);
            var productsWithCategory = products.AsQueryable()
                .Include(p => p.Category)
                .ToList();
                
            return productsWithCategory.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            }).ToList();
        }
    }
    
    /// <summary>
    /// 获取所有租户的产品列表（仅用于演示）
    /// </summary>
    /// <returns>产品列表</returns>
    public async Task<List<ProductDto>> GetProductsForAllTenantsAsync()
    {
        using (_productRepository.DisableTenantFilter())
        {
            var products = await _productRepository.GetListAsync(p => true);
            var productsWithCategory = products.AsQueryable()
                .Include(p => p.Category)
                .ToList();
                
            return productsWithCategory.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            }).ToList();
        }
    }
}

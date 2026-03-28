using Microsoft.AspNetCore.Http;
using Tenon.Repository.EfCore.MultiTenant;

namespace MultiTenantSample.TenantResolvers;

/// <summary>
/// HTTP上下文租户解析器
/// </summary>
public class HttpContextTenantResolver : IEfTenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private long _tenantId = 1; // 默认租户ID
    private long _userId = 1;   // 默认用户ID
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="httpContextAccessor">HTTP上下文访问器</param>
    public HttpContextTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    /// <summary>
    /// 获取或设置当前租户ID
    /// </summary>
    public long TenantId 
    { 
        get => GetTenantIdFromHeader() ?? _tenantId;
        set => _tenantId = value;
    }
    
    /// <summary>
    /// 获取或设置当前用户ID
    /// </summary>
    public long UserId 
    { 
        get => GetUserIdFromHeader() ?? _userId;
        set => _userId = value;
    }
    
    /// <summary>
    /// 从请求头中获取租户ID
    /// </summary>
    private long? GetTenantIdFromHeader()
    {
        if (_httpContextAccessor.HttpContext == null)
            return null;
            
        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdValue) && 
            long.TryParse(tenantIdValue, out var tenantId))
        {
            return tenantId;
        }
        
        return null;
    }
    
    /// <summary>
    /// 从请求头中获取用户ID
    /// </summary>
    private long? GetUserIdFromHeader()
    {
        if (_httpContextAccessor.HttpContext == null)
            return null;
            
        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-User-Id", out var userIdValue) && 
            long.TryParse(userIdValue, out var userId))
        {
            return userId;
        }
        
        return null;
    }
}

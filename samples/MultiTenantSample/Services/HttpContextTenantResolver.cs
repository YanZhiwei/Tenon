using Microsoft.AspNetCore.Http;
using MultiTenantSample.Extensions;
using Tenon.Repository.EfCore;

namespace MultiTenantSample.Services;

/// <summary>
/// HTTP上下文租户解析器
/// </summary>
public class HttpContextTenantResolver : IEfTenantResolver, ITenantFilterable<long>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private long _currentTenantId = 1; // 默认租户ID
    private long _currentUserId = 0;   // 默认用户ID
    private bool _filterEnabled = true;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="httpContextAccessor">HTTP上下文访问器</param>
    public HttpContextTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 获取或设置租户ID
    /// </summary>
    public long TenantId
    {
        get
        {
            if (!_filterEnabled)
            {
                return 0; // 返回0表示不应用租户过滤
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return _currentTenantId;
            }

            // 从请求头中获取租户ID
            if (httpContext.Request.Headers.TryGetValue("X-TenantId", out var tenantIdHeader) && 
                long.TryParse(tenantIdHeader, out var tenantId))
            {
                return tenantId;
            }

            return _currentTenantId;
        }
        set
        {
            _currentTenantId = value;
        }
    }

    /// <summary>
    /// 获取或设置用户ID
    /// </summary>
    public long UserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return _currentUserId;
            }

            // 从请求头中获取用户ID
            if (httpContext.Request.Headers.TryGetValue("X-UserId", out var userIdHeader) && 
                long.TryParse(userIdHeader, out var userId))
            {
                return userId;
            }

            return _currentUserId;
        }
        set
        {
            _currentUserId = value;
        }
    }
    
    /// <summary>
    /// 获取或设置当前租户标识
    /// </summary>
    public long CurrentTenantId
    {
        get => _currentTenantId;
        set => _currentTenantId = value;
    }
    
    /// <summary>
    /// 获取或设置是否启用租户过滤
    /// </summary>
    public bool TenantFilterEnabled
    {
        get => _filterEnabled;
        set => _filterEnabled = value;
    }

    /// <summary>
    /// 禁用租户过滤器
    /// </summary>
    /// <returns>可释放对象，用于恢复租户过滤器</returns>
    public IDisposable DisableTenantFilter()
    {
        var previousState = _filterEnabled;
        _filterEnabled = false;
        
        return new DisposeAction(() =>
        {
            _filterEnabled = previousState;
        });
    }
    
    /// <summary>
    /// 启用租户过滤
    /// </summary>
    public void EnableTenantFilter()
    {
        _filterEnabled = true;
    }

    /// <summary>
    /// 切换租户
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    /// <returns>可释放对象，用于恢复原租户</returns>
    public IDisposable ChangeTenant(long tenantId)
    {
        var previousTenantId = _currentTenantId;
        _currentTenantId = tenantId;
        
        return new DisposeAction(() =>
        {
            _currentTenantId = previousTenantId;
        });
    }
}

/// <summary>
/// 用于管理资源释放的操作类
/// </summary>
public class DisposeAction : IDisposable
{
    private readonly Action _action;
    private bool _disposed;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="action">释放时执行的操作</param>
    public DisposeAction(Action action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _disposed = false;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _action();
        _disposed = true;
    }
}

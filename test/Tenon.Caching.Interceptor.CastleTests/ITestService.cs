using Tenon.Caching.Interceptor.Castle.Attributes;

namespace Tenon.Caching.Interceptor.CastleTests;

internal interface ITestService
{
    [CachingEvict(CacheKeys = new[] { "usr:menus:tree", "usr:menus:relation" })]
    Task<long> CreateAsync(CreationDto input);

    [CachingEvict(CacheKeyPrefix = "usr:users:validatedinfo")]
    Task<bool> UpdatePasswordAsync([CachingParameter] long id, UserChangePwdDto input);

    [CachingEvict(CacheKey = "usr:roles:list")]
    Task<bool> UpdateAsync(long id, RoleUpdationDto input);

    [CachingEvict(CacheKeyPrefix = "usr:users:validatedinfo")]
    Task<bool> ChangeStatusAsync([CachingParameter] IEnumerable<long> ids, int status);
}

internal class TestService : ITestService
{
    public Task<long> CreateAsync(CreationDto input)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdatePasswordAsync(long id, UserChangePwdDto input)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(long id, RoleUpdationDto input)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangeStatusAsync(IEnumerable<long> ids, int status)
    {
        throw new NotImplementedException();
    }
}

internal class RoleUpdationDto
{
    public string Name { get; set; }


    public string Tips { get; set; }

    public int Ordinal { get; set; }
}

internal class UserChangePwdDto
{
    public string OldPassword { get; set; }

    public string Password { get; set; }

    public string RePassword { get; set; }
}

internal class CreationDto
{
    public string Code { get; set; } = string.Empty;
}
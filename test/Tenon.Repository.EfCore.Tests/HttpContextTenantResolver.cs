namespace Tenon.Repository.EfCore.Tests;

public sealed class HttpContextTenantResolver : IEfUserResolver
{
    public long UserId { get; set; }
}
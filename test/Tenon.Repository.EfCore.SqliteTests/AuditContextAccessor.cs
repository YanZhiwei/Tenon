using System.Security.Claims;

namespace Tenon.Repository.EfCore.SqliteTests;

public sealed class AuditContextAccessor : IAuditContextAccessor
{
    public ClaimsPrincipal? Principal { get; set; }
}
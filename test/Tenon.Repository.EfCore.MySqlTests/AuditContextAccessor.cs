using System.Security.Claims;

namespace Tenon.Repository.EfCore.MySqlTests;

public sealed class AuditContextAccessor : IAuditContextAccessor
{
    public ClaimsPrincipal? Principal { get; set; }
}
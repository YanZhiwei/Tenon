using System.Security.Claims;

namespace Tenon.Repository.EfCore;

public interface IAuditContextAccessor
{
    public ClaimsPrincipal? Principal { get; set; }
}
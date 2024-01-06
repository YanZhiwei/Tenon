using System.Collections.Generic;
using System.Security.Claims;

namespace Tenon.Repository;

public class AuditContext
{
    public Dictionary<object, object> Features { get; set; }

    public ClaimsPrincipal User { get; set; }
}
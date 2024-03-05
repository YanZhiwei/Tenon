using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;

namespace Tenon.AspNetCore.Authorization.Bearer
{
    public class BearRequirement : IAuthorizationRequirement
    {
        public BearRequirement()
        {
        }
        public BearRequirement(string name) => Name = name;
        public string Name { get; init; } = string.Empty;
    }
}

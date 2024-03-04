using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;

namespace Tenon.AspNetCore.Authorization.Bearer
{
    public class BearRequirement(string name) : IAuthorizationRequirement
    {
        public string Name { get; init; } = name;
    }
}

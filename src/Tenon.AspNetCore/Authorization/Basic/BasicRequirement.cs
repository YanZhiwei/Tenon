using Microsoft.AspNetCore.Authorization;

namespace Tenon.AspNetCore.Authorization.Basic
{
    public class BasicRequirement(string name) : IAuthorizationRequirement
    {
        public string Name { get; init; } = name;
    }
}

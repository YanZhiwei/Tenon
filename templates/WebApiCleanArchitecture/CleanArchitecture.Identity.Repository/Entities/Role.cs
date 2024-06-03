using Microsoft.AspNetCore.Identity;
using Tenon.Repository;

namespace CleanArchitecture.Identity.Repository.Entities;

public sealed class Role : IdentityRole<long>, IBasicAuditable
{
    public long CreateBy { get; set; }
    public long ModifyBy { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
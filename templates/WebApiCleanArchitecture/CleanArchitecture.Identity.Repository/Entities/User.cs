using Microsoft.AspNetCore.Identity;
using Tenon.Repository;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Repository.Entities;

public sealed class User : IdentityUser<long>, IBasicAuditable, IEntity<long>
{
    public int Status { get; set; }
    public string Account { get; set; }
    public long CreateBy { get; set; }
    public long ModifyBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
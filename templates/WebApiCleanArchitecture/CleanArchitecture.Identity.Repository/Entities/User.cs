using Microsoft.AspNetCore.Identity;
using Tenon.Repository;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Repository.Entities;

public sealed class User : IdentityUser<long>, IBasicAuditable<long>, IEntity<long>
{
    public int Status { get; set; }
    public string Account { get; set; }
    public long CreateBy { get; set; }
    public DateTime CreateTime { get; set; }
    public long ModifyBy { get; set; }
    public DateTime? ModifyTime { get; set; }
}
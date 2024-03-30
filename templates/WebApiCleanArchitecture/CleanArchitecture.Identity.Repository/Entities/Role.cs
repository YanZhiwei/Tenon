using Microsoft.AspNetCore.Identity;
using Tenon.Repository;

namespace CleanArchitecture.Identity.Repository.Entities;

public sealed class Role : IdentityRole<long>, IBasicAuditable<long>
{
    public long CreateBy { get; set; }
    public DateTime CreateTime { get; set; }
    public long ModifyBy { get; set; }
    public DateTime? ModifyTime { get; set; }
}
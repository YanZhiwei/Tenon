using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tenon.Repository.EfCore.MySqlTests.Entities;

public class Blog : EfBasicAuditEntity, IConcurrency
{
    public string Url { get; set; }
    public int Rating { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = default!;

    //[ConcurrencyCheck]
    public byte[] RowVersion { get; set; } = default!;
}
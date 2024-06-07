namespace Tenon.Repository.EfCore;

public class EfAuditableUser : IAuditableUser<long>
{
    public long User { get; set; }
}
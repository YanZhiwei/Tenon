namespace Tenon.Repository
{
    public interface IAuditContextAccessor
    {
        AuditContext Context { get; set; }
    }
}

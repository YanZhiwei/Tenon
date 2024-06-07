namespace Tenon.Repository;

public interface IAuditableUser<TKey>
{
    TKey User { get; set; }
}
namespace Tenon.Repository.EfCore;

public class EfEntity : IEntity<long>
{
    public long Id { get; set; }
}
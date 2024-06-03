namespace Tenon.Repository;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
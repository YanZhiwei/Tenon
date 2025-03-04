namespace Tenon.Repository.EfCore;

public class EfUserResolver : IUserResolver<long>
{
    public long UserId { get; set; }
}
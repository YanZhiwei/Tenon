namespace Tenon.Redis;

public interface IRedisProvider : IRedisStringProvider, IRedisHashProvider, IRedisListProvider, IRedisSortedSetProvider,
    IRedisKeyProvider, IRedisBfProvider
{
}
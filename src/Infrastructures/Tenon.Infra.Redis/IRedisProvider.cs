namespace Tenon.Infra.Redis;

public interface IRedisProvider : IRedisStringProvider, IRedisHashProvider, IRedisListProvider, IRedisSortedSetProvider,
    IRedisKeyProvider, IRedisBfProvider, IRedisLuaProvider
{
}
using System.Threading.Tasks;

namespace Tenon.Redis;

public interface IRedisLuaProvider
{
    /// <summary>
    /// https://redis.io/commands/eval/
    /// </summary>
    Task<dynamic> EvalAsync(string script, object parameters = null);
}
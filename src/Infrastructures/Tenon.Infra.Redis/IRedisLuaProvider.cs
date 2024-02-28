using System.Threading.Tasks;

namespace Tenon.Infra.Redis;

public interface IRedisLuaProvider
{
    /// <summary>
    /// https://redis.io/commands/eval/
    /// </summary>
    Task<dynamic> EvalAsync(string script, object parameters = null);

    /// <summary>
    /// https://redis.io/commands/eval/
    /// </summary>
    dynamic Eval(string script, object parameters = null);
}
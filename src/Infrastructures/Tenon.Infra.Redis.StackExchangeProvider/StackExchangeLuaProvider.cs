using StackExchange.Redis;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public partial class StackExchangeProvider
{
    public async Task<dynamic> EvalAsync(string script, object? parameters = null)
    {
        var prepared = LuaScript.Prepare(script);
        return await _redisDatabase.ScriptEvaluateAsync(prepared, parameters);
    }

    public dynamic Eval(string script, object? parameters = null)
    {
        var prepared = LuaScript.Prepare(script);
        return _redisDatabase.ScriptEvaluate(prepared, parameters);
    }
}
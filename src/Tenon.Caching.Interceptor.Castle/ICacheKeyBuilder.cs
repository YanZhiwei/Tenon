using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.Caching.Interceptor.Castle
{
    public interface ICacheKeyBuilder
    {
        string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix);

        string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix);

        string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix);
    }
}

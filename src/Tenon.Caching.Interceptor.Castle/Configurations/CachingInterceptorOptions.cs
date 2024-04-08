using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.Caching.Interceptor.Castle.Configurations
{
    public class CachingInterceptorOptions
    {
        public int PollyTimeoutSeconds { get; set; } = 10;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tenon.Caching.Abstractions;

namespace Tenon.Caching.RedisStackExchange.Extensions
{
    internal class RedisStackExchangeOptionsExtension:ICachingOptionsExtension
    {
        public void AddServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}

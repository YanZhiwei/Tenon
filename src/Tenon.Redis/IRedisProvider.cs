using System;
using System.Collections.Generic;
using System.Text;

namespace Tenon.Redis
{
    public interface IRedisProvider : IRedisStringProvider, IRedisHashProvider, IRedisListProvider, IRedisSortedSetProvider
    {
    }
}

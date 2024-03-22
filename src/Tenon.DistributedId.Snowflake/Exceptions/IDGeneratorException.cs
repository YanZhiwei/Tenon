using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.DistributedId.Snowflake.Exceptions
{
    public sealed class IdGeneratorException(string message) : Exception(message);
}

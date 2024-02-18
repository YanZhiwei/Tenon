using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.Consul.Options
{
    public sealed class ConsulDiscoveryOptions
    {
        public string ConsulUrl { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }
}

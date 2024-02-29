using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.Infra.RabbitMq.Models
{
    public enum ExchangeType
    {
        Direct,
        Fanout,
        Topic,
        Headers
    }
}

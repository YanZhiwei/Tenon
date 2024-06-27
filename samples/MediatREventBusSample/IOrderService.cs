using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatREventBusSample
{
    public interface IOrderService
    {
        public Task CreateOrder(int orderId);
    }
}

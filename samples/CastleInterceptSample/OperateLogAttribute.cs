using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenon.Infra.Castle.Attributes;

namespace CastleInterceptSample
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class OperateLogAttribute : InterceptAttribute
    {
        public string LogName { get; set; } = string.Empty;
    }
}

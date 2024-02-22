using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Tenon.Caching
{
    public interface ICachingOptionsExtension
    {
        void AddServices(IServiceCollection services);
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Tenon.Caching.Abstractions
{
    public interface ICachingOptionsExtension
    {
        void AddServices(IServiceCollection services);
    }
}

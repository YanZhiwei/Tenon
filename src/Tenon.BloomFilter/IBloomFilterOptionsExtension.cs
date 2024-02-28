using Microsoft.Extensions.DependencyInjection;

namespace Tenon.BloomFilter
{
    public interface IBloomFilterOptionsExtension
    {
        void AddServices(IServiceCollection services);
    }
}
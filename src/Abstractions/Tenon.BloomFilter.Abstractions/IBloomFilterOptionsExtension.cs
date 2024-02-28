using Microsoft.Extensions.DependencyInjection;

namespace Tenon.BloomFilter.Abstractions
{
    public interface IBloomFilterOptionsExtension
    {
        void AddServices(IServiceCollection services);
    }
}
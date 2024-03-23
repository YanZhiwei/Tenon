using Microsoft.Extensions.DependencyInjection;

namespace Tenon.DistributedId.Abstractions;

public interface IDistributedIdOptionsExtension
{
    void AddServices(IServiceCollection services);
}
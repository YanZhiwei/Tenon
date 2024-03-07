using Microsoft.Extensions.DependencyInjection;

namespace Tenon.DistributedLocker.Abstractions;

public interface IDistributedLockerOptionsExtension
{
    void AddServices(IServiceCollection services);
}
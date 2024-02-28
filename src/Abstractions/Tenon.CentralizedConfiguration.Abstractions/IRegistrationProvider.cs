using System;

namespace Tenon.CentralizedConfiguration.Abstractions
{
    /// <summary>
    /// https://learn.microsoft.com/zh-cn/dotnet/architecture/cloud-native/centralized-configuration
    /// </summary>
    public interface IRegistrationProvider
    {
        void Register(Uri serviceAddress, string serviceId);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Tenon.Helper.Internal
{
    public sealed class InterNetworkHelper
    {
        public static IReadOnlyList<string> GetServerIpAddress()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(p.Address))
                .Select(p => p.Address.ToString()).ToList();
        }


        public static IReadOnlyList<string> GetLocalIpAddress(AddressFamily? netType = AddressFamily.InterNetwork)
        {
            var hostName = Dns.GetHostName();
            var addresses = Dns.GetHostAddresses(hostName);

            var ips = new List<string>();
            if (!netType.HasValue)
                ips.AddRange(addresses.Select(t => t.ToString()));
            else
                ips.AddRange(from t in addresses where t.AddressFamily == netType select t.ToString());
            return ips;
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using NetworkManager.Core.Models;
using System.IO;
using System.Collections;

namespace NetworkManager.Core.Services
{
    public interface INetworkService
    {
        Task<object> GetDevicesAsync(bool ipEnabled);
        Task<WMIAdapter> SetDeviceConfigurationAsync(string nicName, string IpAddresses, string SubnetMask, string Gateway, string DnsSearchOrder);
        Task<WMIAdapter> GetDeviceConfigurationAsync(string nicName);
    }
}

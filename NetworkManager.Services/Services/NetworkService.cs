using NetworkManager.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkManager.Services.Services
{
    public class NetworkService : INetworkService
    {
        public NetworkService()
        { }

        public List<string> devices = new List<string>();

        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        public object GetDevices(bool ipEnabled = true)
        {
            return WMIManager.GetDevices(ipEnabled);
        }

        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        public async Task<object> GetDevicesAsync(bool ipEnabled = true)
        {
            return await WMIManager.GetDevicesAsync(ipEnabled);
        }

        /// <summary>
        /// Loads current tcp network configuration for the specified NIC
        /// </summary>
        /// <param name="deviceName"></param>
        public async Task<WMIAdapter> GetDeviceConfigurationAsync(string deviceName)
        {
            return await WMIManager.GetIPAsync(deviceName);
        }

        /// <summary>
        /// Set tcp network configuration for the specified NIC
        /// </summary>
        /// <param name="deviceName, IpAddresses, SubnetMask, Gateway, Dns"></param>
        public async Task<WMIAdapter> SetDeviceConfigurationAsync(string deviceName, string IpAddresses, string SubnetMask, string Gateway, string Dns)
        {
            return await WMIManager.SetIPAsync(deviceName, IpAddresses, SubnetMask, Gateway, Dns);
        }
    }
}
using System;
using System.Collections;
using System.Management;
using System.Threading.Tasks;

namespace NetworkManager.Core.Services
{
	/// <summary>
	/// Class which provides convenient methods to set/get network configurations
	/// configuration
	/// </summary>
	public class WMIManager
	{
		#region Public Static
		/// <summary>
		/// Enable DHCP on the NIC
		/// </summary>
		/// <param name="nicName">Name of the NIC</param>
		public static void SetDHCP( string nicName )
		{
			ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection moc = mc.GetInstances();

			foreach(ManagementObject mo in moc)
			{
				// Make sure this is a IP enabled device. Not something like memory card or VM Ware
				if( (bool)mo["IPEnabled"] )
				{
					if( mo["Caption"].Equals( nicName ) )
					{
						ManagementBaseObject newDNS = mo.GetMethodParameters( "SetDNSServerSearchOrder" );
						newDNS[ "DNSServerSearchOrder" ] = null;
						ManagementBaseObject enableDHCP = mo.InvokeMethod( "EnableDHCP", null, null);
						ManagementBaseObject setDNS = mo.InvokeMethod( "SetDNSServerSearchOrder", newDNS, null);
					}
				}
			}
		}

        /// <summary>
        /// Set IP for the specified network card name
        /// </summary>
        /// <param name="deviceName">Caption of the network card</param>
        /// <param name="IpAddresses">Comma delimited string containing one or more IP</param>
        /// <param name="SubnetMask">Subnet mask</param>
        /// <param name="Gateway">Gateway IP</param>
        /// <param name="Dns">Comma delimited DNS IP</param>
        public static async Task<WMIAdapter> SetIPAsync( string deviceName, string IpAddresses, string SubnetMask, string Gateway, string Dns)
		{
            return await Task.Run(async () =>
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                    if ((bool)mo["IPEnabled"])
                    {
                        if (mo["Caption"].Equals(deviceName))
                        {
                            try
                            {
                                ManagementBaseObject newGate = mo.GetMethodParameters("SetGateways");
                                newGate["DefaultIPGateway"] = new string[] { "192.168.0.1" };
                                newGate["GatewayCostMetric"] = new int[] { 1 };

                                ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                                if (NetworkConfigValidator.isValidIP(IpAddresses))
                                {
                                    newIP["IPAddress"] = IpAddresses.Split(',');
                                    var IpSubnetIps = (string[])mo["IPSubnet"];
                                    newIP["SubnetMask"] = IpSubnetIps[0].Split(',');
                                }

                                ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");
                                newDNS["DNSServerSearchOrder"] = new string[] { "192.168.0.1" };

                                ManagementBaseObject setIP = mo.InvokeMethod("EnableStatic", newIP, null);
                                ManagementBaseObject setGateways = mo.InvokeMethod("SetGateways", newGate, null);
                                ManagementBaseObject setDNS = mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Unable to Set IP : " + ex.Message);
                            }

                            break;
                        }
                    }
                }
                return await GetIPAsync(deviceName);
            });
        }

		/// <summary>
		/// Returns the network device configuration of the specified NIC
		/// </summary>
		/// <param name="deviceName">Name of the Network device</param>
		public static async Task<WMIAdapter> GetIPAsync(string deviceName)
		{
            return await Task.Run(() =>
            {
                WMIAdapter wmiAdapter = new WMIAdapter() { deviceName = deviceName };

                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                    if ((bool)mo["ipEnabled"])
                    {
                        if (mo["Caption"].Equals(deviceName))
                        {
                            wmiAdapter.ipAdresses = (string[])mo["IPAddress"];
                            wmiAdapter.subnets = (string[])mo["IPSubnet"];
                            wmiAdapter.gateways = (string[])mo["DefaultIPGateway"];
                            wmiAdapter.dnses = (string[])mo["DNSServerSearchOrder"];

                            break;
                        }
                    }
                }
                return wmiAdapter;
            });
        }

        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        /// <returns>Array list of string</returns>
        public static object GetDevices(bool ipEnabled = true)
		{
            ArrayList nicNames = new ArrayList { };
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection moc = mc.GetInstances();
            var adaps = moc.GetEnumerator();
			foreach(ManagementObject mo in moc)
			{
                if (ipEnabled)
                    if ((bool)mo["ipEnabled"])
                     nicNames.Add(mo["Caption"]);
                else
                        nicNames.Add(mo["Caption"]);
			}
            return nicNames;
		}
        /// <summary>
        /// Returns the list of Network Interfaces installed
        /// </summary>
        /// <param name="ipEnabled"> true(default) - returns only ipEnabled(network cards) devices</param>
        /// <returns>Array list of string</returns>
        public static async Task<object> GetDevicesAsync(bool ipEnabled = true)
        {
            return await Task.Run(() =>
             {
                 ArrayList nicNames = new ArrayList { };
                 ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                 ManagementObjectCollection moc = mc.GetInstances();
                 var adaps = moc.GetEnumerator();
                 foreach (ManagementObject mo in moc)
                 {
                     if (ipEnabled)
                     {
                         if ((bool)mo["ipEnabled"])
                             nicNames.Add(mo["Caption"]);
                     }
                     else
                         nicNames.Add(mo["Caption"]);
                 }
                 return nicNames;
             });
        }

        #endregion
    }
    public class WMIAdapter
    {
        public string deviceName { get; set; }
        public string[] ipAdresses { get; set; }
        public string[] subnets { get; set; }
        public string[] gateways { get; set; }
        public string[] dnses { get; set; }
    }
}

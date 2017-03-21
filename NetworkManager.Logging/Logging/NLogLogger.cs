using NetworkManager.Core.Logging;
using NetworkManager.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkManager.Logging.Logging
{
    public class NLogLogger : ILogger
    {
        private static readonly Lazy<NLogLogger> LazyLogger = new Lazy<NLogLogger>(() => new NLogLogger());
        private static readonly Lazy<NLog.Logger> LazyNLogger = new Lazy<NLog.Logger>(NLog.LogManager.GetCurrentClassLogger);

        public static ILogger Instance
        {
            get
            {
                return LazyLogger.Value;
            }
        }

        private NLogLogger()
        {
        }

        public void Log(string message)
        {
            LazyNLogger.Value.Info(message);
        }

        public void Log(Exception ex)
        {
            LazyNLogger.Value.Error(ex);
        }
        private static string _fileName = "\\networkLog.txt";
        private static string _filePath
        {
            get
            {
                return System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            }
        }
        public static async Task LogNetworkEventAsync(NetworkManager.Core.Services.WMIAdapter wmiAdapter)
        {
            await Task.Run(() =>
            {
                string ips = NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.ipAdresses);
                string subnets = NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.subnets);
                string gateways = NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.gateways);
                string dnss = NetworkConfigValidator.ConvertArrayOfIpsToString(wmiAdapter.dnses);

                using (StreamWriter w = File.AppendText(_filePath + _fileName))
                {
                    w.WriteLine("  Device:{0} ip:{1} subnet:{2}, gateway:{3} dns:{4} ", wmiAdapter.deviceName, ips, subnets, gateways, dnss);
                    w.WriteLine("Config changed: {0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.Close();
                }
            });
        }
        public static async Task<string[]> GetNetworkLogAsync()
        {
            return await Task.Run(() =>
            {
                return File.ReadAllLines(_filePath + _fileName).Reverse().ToArray();
            });
        }
    }
}

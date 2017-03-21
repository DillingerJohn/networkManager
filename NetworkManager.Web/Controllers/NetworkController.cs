using NetworkManager.Core.Models;
using NetworkManager.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;



namespace NetworkManager.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/Network")]
    public class NetworkController : ApiController
    {
        private readonly INetworkService _networkService;
        public NetworkController(INetworkService networkService)
        {
            _networkService = networkService;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Log")]
        public async Task<IHttpActionResult> Log(bool ipEnabled = true)
        {
            try
            {
                var logEvents = await Logging.Logging.NLogLogger.GetNetworkLogAsync();
                return Ok(new
                {
                    error = false,
                    data = logEvents
                });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, ErrorMessage = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetDevices")]
        public async Task<IHttpActionResult> GetDevices(bool ipEnabled = true)
        {
            try {
                var devices = await _networkService.GetDevicesAsync(ipEnabled);
                return Ok(new
                {
                    error = false,
                    data = devices
                });
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, ErrorMessage = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetDevice/{deviceName?}")]
        public async Task<IHttpActionResult> GetDevice(string deviceName)
        {
            try {
                var device = await _networkService.GetDeviceConfigurationAsync(deviceName);
                return Ok(new
                {
                    error = false,
                    data = device
                });
            }
            catch (Exception ex) { return Ok(new { error = true, ErrorMessage = ex.Message }); }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("SetIp/{deviceName?}")]
        public async Task<IHttpActionResult> SetIp(string deviceName, string ipAdresses = "", string subnets = "", string gateways = "", string dnses = "")
        {
            try
            {
                var setConfigurationResult = await _networkService.SetDeviceConfigurationAsync(deviceName, ipAdresses, subnets, gateways, dnses);
                await Logging.Logging.NLogLogger.LogNetworkEventAsync(setConfigurationResult);
                return Ok(new
                {
                    error = false,
                    data = setConfigurationResult
                });
            }
            catch (Exception ex) { return Ok(new { error = true, ErrorMessage = ex.Message }); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }
}



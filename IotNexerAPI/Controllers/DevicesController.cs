using IoTNexerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace IoTNexerAPI.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class DevicesController : ControllerBase
    {
        private readonly ISensorDataRepository<SensorData> _repository;

        public DevicesController(ISensorDataRepository<SensorData> repository)
        {
            this._repository = repository;
        }

        [HttpGet("devices/{devicename}/data/{sensortype}/{datetime}")]
        public async Task<IActionResult> GetDailyDeviceSensorData(string devicename, string sensortype, string datetime)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(datetime, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out dt))
            {
                throw new InvalidOperationException("The date is incorrect format. Please try yyyy-MM-dd.");
            }

            //TODO: Create a method to verify devicename parameter with Azure Blob Metadata file.

            //TODO: Create a method to verify sensortype parameter with Azure Blob Metadata file.

            var result = await _repository.GetDailyDeviceSensorData(devicename, sensortype, datetime);
            return Ok(result);
        }

        [HttpGet("devices/{devicename}/data/{datetime}")]
        public async Task<IActionResult> GetDailyDeviceData(string devicename, string datetime)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(datetime, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out dt))
            {
                throw new InvalidOperationException("The date is incorrect format. Please try yyyy-MM-dd.");
            }

            //TODO: Create a method to verify devicename parameter with Azure Blob Metadata file.

            var result = await _repository.GetDailyDeviceData(devicename, datetime);
            return Ok(result);
        }

    }
}   

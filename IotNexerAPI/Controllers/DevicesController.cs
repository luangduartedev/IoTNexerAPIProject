using IoTNexerAPI.Domain.Entity;
using IoTNexerAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(await _repository.GetDailyDeviceSensorData(devicename, sensortype, datetime));
        }

        [HttpGet("devices/{devicename}/data/{datetime}")]
        public async Task<IActionResult> GetDailyDeviceData(string devicename, string datetime)
        {
            return Ok(await _repository.GetDailyDeviceData(devicename, datetime));
        }

    }
}   

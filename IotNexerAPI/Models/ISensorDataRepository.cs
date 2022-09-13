using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTNexerAPI.Models
{
    public interface ISensorDataRepository<T>
    {
        Task<IEnumerable<SensorData>> GetDailyDeviceSensorData(string devicename, string datetime, string sensortype);
        Task<IEnumerable<SensorData>> GetDailyDeviceData(string devicename, string datetime);
    }
}

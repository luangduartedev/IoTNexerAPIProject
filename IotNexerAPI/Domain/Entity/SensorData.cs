
namespace IoTNexerAPI.Domain.Entity
{
    public class SensorData
    {
        public string SensorType { get; set; }
        public string Date { get; set; }
        public string Value { get; set; }

        public SensorData(string sensorType, string date, string value)
        {
            SensorType = sensorType;
            Date = date;
            Value = CorrectingStartString(value);
        }

        private string CorrectingStartString(string stringToCheck)
        {

            if (stringToCheck.StartsWith(","))
            {
                stringToCheck = "0" + stringToCheck;
            }
            else if (stringToCheck.StartsWith("-,"))
            {
                stringToCheck = stringToCheck.Replace("-", "-0");
            }
            else if (stringToCheck.StartsWith("+,"))
            {
                stringToCheck = stringToCheck.Replace("+", "0");
            }
            return stringToCheck;
        }

    }
}

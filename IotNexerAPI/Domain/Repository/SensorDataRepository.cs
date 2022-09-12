using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IoTNexerAPI.Domain.Entity;
using IoTNexerAPI.Domain.Interfaces;
using System.Globalization;

namespace IoTNexerAPI.Domain.Repository
{
    public class SensorDataRepository : ISensorDataRepository<SensorData>
    {
        private string _connectionString;

        private string _containerName = "iotbackend";
        public SensorDataRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString").GetValue<string>("AzureBlobStorage"); 
        }

        public async Task<IEnumerable<SensorData>> GetDailyDeviceSensorData(string devicename, string sensortype, string datetime)
        {
            DateTime.ParseExact(datetime, "yyyy-dd-MM", CultureInfo.InvariantCulture);

            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient($"{devicename}/{sensortype}/{datetime}.csv");
           
            MemoryStream memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            memoryStream.Position = 0;

            List<SensorData> ItemsList = new List<SensorData>();

            using (var reader = new StreamReader(memoryStream))
            {
                string[] values;
                while (!reader.EndOfStream)
                {
                    values = reader.ReadLine().Split(';');


                    ItemsList.Add(new SensorData(sensortype, values[0], values[1]));
                }
            }

            return ItemsList;
        }


        public async Task<IEnumerable<SensorData>> GetDailyDeviceData(string devicename, string datetime)
        {
            DateTime.ParseExact(datetime, "yyyy-dd-MM", CultureInfo.InvariantCulture);

            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);

            List<SensorData> ItemsList = new List<SensorData>();

            string fileName = "";
            string sensortype = "";
            int startPosition = 0;

            await foreach (BlobItem blobItem in container.GetBlobsAsync())
            {
                if ((blobItem.Name).EndsWith($"{datetime}.csv") && (blobItem.Name).StartsWith($"{devicename}"))
                { 
                    fileName = blobItem.Name;
                    startPosition = fileName.IndexOf("/") + 1;
                    sensortype = fileName.Substring(startPosition, fileName.LastIndexOf("/") - startPosition);

                    MemoryStream memoryStream = new MemoryStream();
                    BlobClient blobClient = container.GetBlobClient(fileName);
                    await blobClient.DownloadToAsync(memoryStream);
                    memoryStream.Position = 0;

                    using (var reader = new StreamReader(memoryStream))
                    {
                        string[] values;
                        while (!reader.EndOfStream)
                        {
                            values = reader.ReadLine().Split(';');

                            ItemsList.Add(new SensorData(sensortype, values[0], values[1]));
                        }
                    }
                }
            }

            return ItemsList;
        }

    }
}

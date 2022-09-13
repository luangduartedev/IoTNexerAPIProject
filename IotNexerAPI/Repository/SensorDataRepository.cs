using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IoTNexerAPI.Models;
using System.IO.Compression;

namespace IoTNexerAPI.Repository
{
    public class SensorDataRepository : ISensorDataRepository<SensorData>
    {
        private string _connectionString;

        private string _containerName;

        public SensorDataRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionString").GetValue<string>("AzureBlobStorage");
            _containerName = configuration.GetSection("ConnectionString").GetValue<string>("ContainerName");
        }

        public async Task<IEnumerable<SensorData>> GetDailyDeviceSensorData(string devicename, string sensortype, string datetime)
        {

            List<SensorData> responseList = new List<SensorData>();

            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName); //TODO: Refactor code to make this method become private string and to be declared in the scope page as a private Object.

            string fileName = "";
            string sensortypeRead = "";

            await foreach (BlobItem blobItem in container.GetBlobsAsync())
            {
                if (blobItem.Name == $"{devicename}/{sensortype}/{datetime}.csv")
                {
                    fileName = blobItem.Name;

                    //TODO: Refactor code to create a method that access container and download file
                    MemoryStream memoryStream = new MemoryStream();
                    BlobClient blobClient = container.GetBlobClient(fileName);
                    await blobClient.DownloadToAsync(memoryStream); //TODO: Change the method to download file with Azure Blocks in parallel threads. It will reduce more than 50% period to do the complete process.
                    memoryStream.Position = 0;

                    var tupleData = ReadBlobStream(memoryStream);

                    string dateRead;
                    string valueRead;

                    foreach (Tuple<string, string> kvp in tupleData)
                    {
                        dateRead = kvp.Item1;
                        valueRead = kvp.Item2;
                        responseList.Add(new SensorData(sensortypeRead, dateRead, valueRead));
                    }
                    return responseList;
                }
            }

            if (responseList.Count == 0)
            {
                BlobClient blobClientMetadata = container.GetBlobClient($"metadata.csv");

                //TODO: Refactor code to create a method that access container and download file
                MemoryStream memoryStreamMetadata = new MemoryStream();
                await blobClientMetadata.DownloadToAsync(memoryStreamMetadata); //TODO: Change the method to download file with Azure Blocks in parallel threads. It will reduce more than 50% period to do the complete process.
                memoryStreamMetadata.Position = 0;


                var tupleData = ReadBlobStream(memoryStreamMetadata);

                string deviceRead;
                string sensorRead;

                foreach (Tuple<string, string> kvp in tupleData)
                {
                    deviceRead = kvp.Item1;
                    sensorRead = kvp.Item2;

                   if(deviceRead == devicename && sensorRead == sensortype)
                    {
                        //TODO: Refactor code to create a method that access container and download file
                        MemoryStream memoryStreamZip = new MemoryStream();
                        BlobClient blobClientZip = container.GetBlobClient($"{deviceRead}/{sensorRead}/historical.zip");
                        await blobClientZip.DownloadToAsync(memoryStreamZip); //TODO: Change the method to download file with Azure Blocks in parallel threads. It will reduce more than 50% period to do the complete process.
                        memoryStreamZip.Position = 0;

                        ZipArchive archive = new ZipArchive(memoryStreamZip);
                        foreach (ZipArchiveEntry file in archive.Entries)
                        {
                            string dateRead = file.Name;
                            if (dateRead == $"{datetime}.csv")
                            {
                                using (var r = new StreamReader(file.Open()))
                                {
                                    while (!r.EndOfStream)
                                    {
                                        //TODO: Refactor code to create a method that Split the entry parameters and return a SensorData List
                                        string[] values1 = r.ReadLine().Split(';');

                                        string dateRead1 = values1[0];
                                        string valueRead1 = values1[1];

                                        responseList.Add(new SensorData(sensorRead, dateRead1, valueRead1));
                                    }
                                }
                                return responseList;
                            }
                        }
                    }
                }
            }

            return responseList;
        }


        public async Task<IEnumerable<SensorData>> GetDailyDeviceData(string devicename, string datetime)
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);
            List<SensorData> responseList = new List<SensorData>();

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

                    //TODO: Refactor code to create a method that access container and download file
                    MemoryStream memoryStream = new MemoryStream();
                    BlobClient blobClient = container.GetBlobClient(fileName);
                    await blobClient.DownloadToAsync(memoryStream); //TODO: Change the method to download file with Azure Blocks in parallel threads. It will reduce more than 50% period to do the complete process.
                    memoryStream.Position = 0;

                    var tupleData = ReadBlobStream(memoryStream);

                    string dateRead;
                    string valueRead;

                    //TODO: Refactor code to create a method that Split the entry parameters and return a SensorData List
                    foreach (Tuple<string, string> kvp in tupleData)
                    {
                        dateRead = kvp.Item1;
                        valueRead = kvp.Item2;
                        responseList.Add(new SensorData(sensortype, dateRead, valueRead));
                    }
                }
            }

            if(responseList.Count == 0)
            {


                BlobClient blobClientMetadata = container.GetBlobClient($"metadata.csv");

                //TODO: Refactor code to create a method that access container and download file
                MemoryStream memoryStreamMetadata = new MemoryStream();
                await blobClientMetadata.DownloadToAsync(memoryStreamMetadata); //TODO: Change the method to download file with Azure Blocks in parallel threads. It will reduce more than 50% period to do the complete process.
                memoryStreamMetadata.Position = 0;


                var tupleData = ReadBlobStream(memoryStreamMetadata);

                string deviceRead;
                string sensorRead;

                foreach (Tuple<string, string> kvp in tupleData)
                {
                    deviceRead = kvp.Item1;
                    sensorRead = kvp.Item2;

                    //TODO: Refactor code to create a method that access container and download file
                    MemoryStream memoryStreamZip = new MemoryStream();
                    BlobClient blobClientZip = container.GetBlobClient($"{deviceRead}/{sensorRead}/historical.zip");
                    await blobClientZip.DownloadToAsync(memoryStreamZip); //TODO: Change the method to download file with Azure Blocks in parallel threads. It will reduce more than 50% period to do the complete process.
                    memoryStreamZip.Position = 0;

                    ZipArchive archive = new ZipArchive(memoryStreamZip);
                    foreach (ZipArchiveEntry file in archive.Entries)
                    {
                        string dateRead = file.Name;
                        if (dateRead == $"{datetime}.csv")
                        {
                            using (var r = new StreamReader(file.Open()))
                            {
                                while (!r.EndOfStream)
                                {
                                    //TODO: Refactor code to create a method that Split the entry parameters and return a SensorData List
                                    string[] values1 = r.ReadLine().Split(';');

                                    string dateRead1 = values1[0];
                                    string valueRead1 = values1[1];

                                    responseList.Add(new SensorData(sensorRead, dateRead1, valueRead1));
                                }

                            }
                        }
                    }
                }
            }

            return responseList;
        }
        private List<Tuple<string, string>> ReadBlobStream(MemoryStream memoryStream)
        {
            List<Tuple<string, string>> responseList = new List<Tuple<string, string>>();
            using (var reader = new StreamReader(memoryStream))
            {
                string[] values;

                while (!reader.EndOfStream)
                {
                    values = reader.ReadLine().Split(';');
                    responseList.Add(new Tuple<string, string>(values[0], values[1]));
                }
            }
            return responseList;
        }

    }
}

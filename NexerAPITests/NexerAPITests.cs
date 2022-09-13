using Azure.Storage.Blobs;
using IoTNexerAPI.Models;

namespace NexerAPITests
{
    [TestClass]
    public class NexerAPITests
    {

        private string _connectionString = "BlobEndpoint=https://sigmaiotexercisetest.blob.core.windows.net/;QueueEndpoint=https://sigmaiotexercisetest.queue.core.windows.net/;FileEndpoint=https://sigmaiotexercisetest.file.core.windows.net/;TableEndpoint=https://sigmaiotexercisetest.table.core.windows.net/;SharedAccessSignature=sv=2017-11-09&ss=bfqt&srt=sco&sp=rl&se=2028-09-27T16:27:24Z&st=2018-09-27T08:27:24Z&spr=https&sig=eYVbQneRuiGn103jUuZvNa6RleEeoCFx1IftVin6wuA%3D";

        private string _containerName = "iotbackend";

        [TestMethod]
        public void GetBlobClient_WrongDeviceName_ReturnError()
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient("WrongDeviceName/humidity/2019-01-11.csv");

            bool result = blobClient.Exists().Value;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetBlobClient_WrongSensorType_ReturnError()
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient("dockan/wrongSensorType/2019-01-11.csv");

            bool result = blobClient.Exists().Value;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetBlobClient_WrongDateFormat_ReturnError()
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient("dockan/humidity/2019/01/11.csv");

            bool result = blobClient.Exists().Value;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetBlobClient_ConrrectParamatersButFileDoesNotExists_ReturnError()
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient("dockan/humidity/1980-01-11.csv");

            bool result = blobClient.Exists().Value;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetBlobClient_ConrrectParamatersAndFileExists_ReturnSucess()
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString, _containerName);
            BlobClient blobClient = container.GetBlobClient("dockan/humidity/2019-01-11.csv");

            bool result = blobClient.Exists().Value;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CorrectingStartString_StartWithComma_ReturnSucess()
        {
            SensorData sensorData = new SensorData("humidity", "2019-01-11", ",5");

            var result = sensorData.Value;

            Assert.AreEqual(result, "0,5");
        }

        [TestMethod]
        public void CorrectingStartString_StartWithNegativeComma_ReturnSucess()
        {
            SensorData sensorData = new SensorData("humidity", "2019-01-11", "-,5");

            var result = sensorData.Value;

            Assert.AreEqual(result, "-0,5");
        }

        [TestMethod]
        public void CorrectingStartString_StartWithPositiveComma_ReturnSucess()
        {
            SensorData sensorData = new SensorData("humidity", "2019-01-11", "+,5");

            var result = sensorData.Value;

            Assert.AreEqual(result, "0,5");
        }

        //TODO: Create TestMethod to ReadBlobStream method with scenarios: ReadBlobStream_WrongMemoryStream_ReturnError() and ReadBlobStream_CorrectMemoryStream_ReturnSuccess()
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace WeatherLogger.Functions.Functions
{
    public class GetPayload
    {
        private readonly BlobContainerClient _containerClient;

        public GetPayload(IConfiguration configuration)
        {
            var conn = configuration["AzureWebJobsStorage"];
            var blobServiceClient = new BlobServiceClient(conn);
            _containerClient = blobServiceClient.GetBlobContainerClient("weatherpayloads");
            _containerClient.CreateIfNotExists();
        }

        [FunctionName("GetPayload")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payload/{logId}")] HttpRequest req,
            string logId,
            ILogger log)
        {
            string rowKey = logId;
            var blobName = $"{rowKey}.json";
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                return new NotFoundObjectResult($"Blob with RowKey '{rowKey}' not found.");

            var download = await blobClient.DownloadContentAsync();
            var content = download.Value.Content.ToString();

            return new OkObjectResult(content);
        }
    }
}

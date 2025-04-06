using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WeatherLogger.Functions.Models;
using Microsoft.Extensions.Configuration;

namespace WeatherLogger.Functions.Functions
{
    public class GetLogs
    {
        private readonly TableClient _tableClient;

        public GetLogs(IConfiguration configuration)
        {
            var conn = configuration["AzureWebJobsStorage"];
            _tableClient = new TableClient(conn, "WeatherLogs");
            _tableClient.CreateIfNotExists();
        }

        [FunctionName("GetLogs")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logs")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Received request to get logs.");

            if (!DateTimeOffset.TryParse(req.Query["from"], out var from) ||
                !DateTimeOffset.TryParse(req.Query["to"], out var to))
            {
                return new BadRequestObjectResult("Please provide valid 'from' and 'to' query parameters in ISO 8601 format.");
            }

            var result = new List<WeatherLog>();

            await foreach (var entity in _tableClient.QueryAsync<WeatherLog>(x =>
                x.Timestamp >= from && x.Timestamp <= to && x.PartitionKey == "Weather"))
            {
                result.Add(entity);
            }

            var sorted = result
                .OrderBy(x => x.Timestamp)
                .ThenBy(x => x.City)
                .ToList();

            return new OkObjectResult(sorted);
        }
    }
}

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Data.Tables;
using WeatherLogger.Functions.Models;

namespace WeatherLogger.Functions.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableClient _tableClient;

        public TableStorageService(IConfiguration config)
        {
            var connectionString = config["AzureWebJobsStorage"];
            _tableClient = new TableClient(connectionString, "WeatherLogs");
            _tableClient.CreateIfNotExists();
        }

        public async Task SaveLogAsync(WeatherLog log)
        {
            await _tableClient.AddEntityAsync(log);
        }
    }
}

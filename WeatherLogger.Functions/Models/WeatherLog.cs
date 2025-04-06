using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace WeatherLogger.Functions.Models
{
    public class WeatherLog : ITableEntity
    {
        public string PartitionKey { get; set; } = "Weather";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public ETag ETag { get; set; } = ETag.All;

        public string City { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}

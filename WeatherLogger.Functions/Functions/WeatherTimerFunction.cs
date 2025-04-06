using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using WeatherLogger.Functions.Models;
using WeatherLogger.Functions.Services;

namespace WeatherLogger.Functions.Functions
{
    public class WeatherTimerFunction
    {
        private readonly IBlobStorageService _blobService;
        private readonly ITableStorageService _tableService;
        private readonly IWeatherApiService _weatherApiService;

        public WeatherTimerFunction(
            IBlobStorageService blobService,
            ITableStorageService tableService,
            IWeatherApiService weatherApiService)
        {
            _blobService = blobService;
            _tableService = tableService;
            _weatherApiService = weatherApiService;
        }

        [FunctionName("WeatherTimerFunction")]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"[START] WeatherTimerFunction executed at: {DateTime.UtcNow}");

            var city = "London";
            var rowKey = Guid.NewGuid().ToString();
            string content = string.Empty;
            bool success = false;
            string errorMessage = null;

            // 1. Fetch data
            try
            {
                content = await _weatherApiService.GetWeatherForCityAsync(city);
                log.LogInformation($"[API OK] Weather data fetched successfully for city: {city}");
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = $"[API ERROR] Failed to fetch weather data for {city}: {ex.Message}";
                log.LogError(errorMessage);
            }

            // 2. Save payload to blob
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    var blobName = $"{rowKey}.json";
                    await _blobService.SavePayloadAsync(blobName, content);
                    log.LogInformation($"[BLOB OK] Payload saved for {city} as blob: {blobName}");
                }
                catch (Exception ex)
                {
                    log.LogError($"[BLOB ERROR] Failed to save payload to blob for {city}: {ex.Message}");
                }
            }

            // 3. Save log to table
            try
            {
                var logEntry = new WeatherLog
                {
                    City = city,
                    Success = success,
                    ErrorMessage = errorMessage,
                    PartitionKey = "Weather",
                    RowKey = rowKey
                };

                await _tableService.SaveLogAsync(logEntry);
                log.LogInformation($"[TABLE OK] Log saved for {city}, Success = {success}, RowKey = {rowKey}");
            }
            catch (Exception ex)
            {
                log.LogError($"[TABLE ERROR] Failed to save log entry for {city}: {ex.Message}");
            }

            log.LogInformation("[END] WeatherTimerFunction completed.");
        }
    }
}

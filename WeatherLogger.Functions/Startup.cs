using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WeatherLogger.Functions.Services;

[assembly: FunctionsStartup(typeof(WeatherLogger.Functions.Startup))]

namespace WeatherLogger.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient<IWeatherApiService, WeatherApiService>();
            builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
            builder.Services.AddSingleton<ITableStorageService, TableStorageService>();
        }
    }
}

using Microsoft.Extensions.Configuration;
using Shouldly;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherLogger.Functions.Services;
using Xunit;

namespace WeatherLogger.Functions.Tests.Services
{
    public class WeatherApiServiceIntegrationTests
    {
        [Fact]
        public async Task GetWeatherForCityAsync_Should_Return_Json_Response_From_Real_Api()
        {
            // Arrange
            var httpClient = new HttpClient();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "OpenWeatherApiKey", "YOUR_REAL_API_KEY" }
                })
                .Build();

            var service = new WeatherApiService(httpClient, config);

            // Act
            var response = await service.GetWeatherForCityAsync("London");

            // Assert
            response.ShouldNotBeNullOrWhiteSpace();
            response.ShouldContain("London", Case.Insensitive);
            response.ShouldContain("weather"); // sanity check, expected in payload
        }
    }
}

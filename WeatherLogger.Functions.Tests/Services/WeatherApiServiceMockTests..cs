using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Shouldly;
using WeatherLogger.Functions.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace WeatherLogger.Functions.Tests.Services
{
    public class WeatherApiServiceMockTests
    {
        [Fact]
        public async Task GetWeatherForCityAsync_Should_Return_Response_Content_When_Api_Success()
        {
            // Arrange
            var fakeResponse = "{\"weather\": [{\"main\": \"Clear\"}]}";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponse),
                });

            var httpClient = new HttpClient(handlerMock.Object);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["OpenWeatherApiKey"]).Returns("fake-api-key");

            var service = new WeatherApiService(httpClient, configurationMock.Object);

            // Act
            var result = await service.GetWeatherForCityAsync("London");

            // Assert
            result.ShouldBe(fakeResponse);
        }

        [Fact]
        public async Task GetWeatherForCityAsync_Should_Throw_If_ApiKey_Missing()
        {
            // Arrange
            var httpClient = new HttpClient();
            var config = new Mock<IConfiguration>();
            config.Setup(x => x["OpenWeatherApiKey"]).Returns((string)null);

            var service = new WeatherApiService(httpClient, config.Object);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await service.GetWeatherForCityAsync("London");
            });
        }
    }
}

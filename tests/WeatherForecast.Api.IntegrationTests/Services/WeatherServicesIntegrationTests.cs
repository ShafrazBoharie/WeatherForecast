using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using WeatherForecast.Api.Models;
using Microsoft.Extensions.Options;
using Moq;
using WeatherForecast.Api.Models.Entities;
using WeatherForecast.Api.Services;
using Xunit;

namespace WeatherForecast.Api.IntegrationTests.Services
{
    public class WeatherServicesIntegrationTests
    {
        private readonly ILogger<WeatherService> _logger;
        private readonly AccuWeatherSettings _accuWeatherSettings;
        private readonly WeatherService _weatherService;

        public WeatherServicesIntegrationTests()
        {
           
            _accuWeatherSettings = new AccuWeatherSettings{
                AccuWeatherHost = "https://dataservice.accuweather.com",
                AccuWeatherKey = "UwpUxO06Y9tYvwpLtLHuLZW56Nr8lFZY"
            };

            _logger = new Mock<ILogger<WeatherService>>().Object;
            _weatherService = new WeatherService(new DefaultHttpClientFactory(), _accuWeatherSettings, _logger);
        }

        [Fact]
        public async Task WhenMatchingLocationsDoesNotMatchWithStartStringResultShouldReturnEmptyLocation()
        {
            var response = await _weatherService.GetMatchingLocations("$£$$");

            response.Should().BeEquivalentTo(new List<Location>());
            response.Count.Should().Be(0);
        }

        [Fact]
        public async Task WhenMatchingLocationsDoesExistResultShouldReturnCollectionOfLocations()
        {
            var response = await _weatherService.GetMatchingLocations("Lon");
            
            response.Count.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task WhenMatchingForecastIsPassedResultShouldReturnForecast()
        {
            var londonKey = 328328;
            var response = await _weatherService.GetForecast(londonKey);

            response.Should().NotBeNull();
            response.DailyForecasts.Count.Should().Be(1);
        }

        [Fact]
        public async Task WhenNoMatchingKeyIsPassedResultShouldReturnForecast()
        {
            var invalidKey = 0;
            var response = await _weatherService.GetForecast(invalidKey);

            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenMatchingKeyIsPassedResultShouldReturnForecast()
        {
            var londonKey = 328328;
            var response = await _weatherService.GetForecast(londonKey);

            response.Should().NotBeNull();
            response.DailyForecasts.Count.Should().Be(1);
        }
    }
    
    public sealed class DefaultHttpClientFactory : IHttpClientFactory
    {
        private static readonly Lazy<HttpClient> _httpClientLazy =
            new Lazy<HttpClient>(() => new HttpClient());
                
        public HttpClient CreateClient(string name) => _httpClientLazy.Value;
    }
}
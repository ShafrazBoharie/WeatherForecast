using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Net;
using System.Text.Json;
using WeatherForecast.Api.Models;
using WeatherForecast.Api.Models.Entities;

namespace WeatherForecast.Api.Services
{
    public class WeatherService
    {
        private readonly ILogger<WeatherService> _logger;
        private readonly AsyncRetryPolicy<bool> _retryPolicy;
        private readonly HttpClient _httpClient;
        private readonly AccuWeather accueWeatherSettings;
        private readonly int MaxRetries = 3;
        public WeatherService(IHttpClientFactory httpClientFactory,IOptions<AccuWeather> accuWesatherOptions, ILogger<WeatherService> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("AccuWeather");
            accueWeatherSettings = accuWesatherOptions.Value;
            _retryPolicy = Policy<bool>.Handle<Exception>().RetryAsync(MaxRetries);
        }

        public async Task<List<Location>> GetMatchingLocations(string startString)
        {
            var status = false;
            var output = new List<Location>();
            try
            {
                status = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var url =
                        $"{accueWeatherSettings.AccuWeatherHost}/locations/v1/cities/autocomplete?apikey={accueWeatherSettings.AccuWeatherKey}&q={startString.Trim()}";
                    var response = await _httpClient.GetAsync(url);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                        {
                            return true;
                        }
                        case HttpStatusCode.OK:
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var locationsContent = JsonSerializer.Deserialize<List<Location>>(content);
                            output= locationsContent;
                            return true;
                        }
                        default:
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            throw new Exception(
                                $"Error retrieving locations for '{startString}', Response: '{responseContent}'");
                            return false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Locations");
                return new List<Location>();
            }

            return output;
        }
         
        public async Task<Forecast> GetForecast(int key)
        {
            var status = false;
            Forecast output = null;
            try
            {
                status = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var url =
                        $"{accueWeatherSettings.AccuWeatherHost}/forecasts/v1/daily/1day/{key}?apikey={accueWeatherSettings.AccuWeatherKey}";
                    var response = await _httpClient.GetAsync(url);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                        {
                            return true;
                        }
                        case HttpStatusCode.OK:
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var forecastsContent = JsonSerializer.Deserialize<Forecast>(content);
                            output = forecastsContent;
                            return true;
                        }
                        default:
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            throw new Exception(
                                $"Error retrieving forecast for '{key}', Response: '{responseContent}'");
                            return false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Locations");
            }

            return output;
        }
    }
}

using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Identity.Web;
using Polly;
using Polly.Retry;
using WeatherForecast.Api.Models.Dto;

namespace WeatherForecast.App.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<bool> _retryPolicy;
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly int MaxRetries = 3;


        public WeatherForecastService(IHttpClientFactory httpClientFactory, ILogger<WeatherForecastService> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClientFactory.CreateClient("api");
            _retryPolicy = Policy<bool>.Handle<Exception>().RetryAsync(MaxRetries);
        }
        public async Task<List<LocationDto>> GetMatchingLocations(string keyword)
        {
            var status = false;
            var output = new List<LocationDto>();

         //   string[] scopes = new string[] { "Api.Readonly" };
          //  string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
          //  _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                status = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var url = $"/api/Weather/Locations/{keyword}";
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
                            var locationsContent = JsonSerializer.Deserialize<List<LocationDto>>(content);
                            output = locationsContent;
                            return true;
                        }
                        default:
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            throw new Exception(
                                $"Error retrieving locations for '{keyword}', Response: '{responseContent}'");
                            return false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Locations");
                return new List<LocationDto>();
            }
            return output;
        }

        public async Task<ForecastDto> GetForecast(string key)
        {
            var status = false;
            ForecastDto output = null;
            try
            {
                status = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var url =
                        $"/api/Weather/Forecast/{key}";
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
                            var forecastsContent = JsonSerializer.Deserialize<ForecastDto>(content);
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
                _logger.LogError(ex, "Error retrieving Forecast");
            }

            return output;
        }
    }
}

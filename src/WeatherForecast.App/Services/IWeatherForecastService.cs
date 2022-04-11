using WeatherForecast.Api.Models.Dto;

namespace WeatherForecast.App.Services;

public interface IWeatherForecastService
{
    Task<List<LocationDto>> GetMatchingLocations(string keyword);
    Task<ForecastDto> GetForecast(string key);
}
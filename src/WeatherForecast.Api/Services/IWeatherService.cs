using WeatherForecast.Api.Models.Entities;

namespace WeatherForecast.Api.Services;

public interface IWeatherService
{
    Task<List<Location>> GetMatchingLocations(string startString);
    Task<Forecast> GetForecast(int key);
}
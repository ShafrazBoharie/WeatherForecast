using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Api.Mapper;
using WeatherForecast.Api.Services;

namespace WeatherForecast.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IWeatherService _weatherService;
        private readonly ForecastMapper _forecastMapper;
        private readonly LocationMapper _locationMapper;

        public WeatherController(ILogger<WeatherController> logger, IWeatherService weatherService, ForecastMapper forecastMapper, LocationMapper locationMapper)
        {
            _logger = logger;
            _weatherService = weatherService;
            _forecastMapper = forecastMapper;
            _locationMapper = locationMapper;
        }

        // [Authorize]
        [HttpGet("Locations/{keyword}")]
        public async Task<IActionResult> GetLocations(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return NotFound();

            var locations = await _weatherService.GetMatchingLocations(keyword);

            if (locations.Any())
            {
                var locationsDtos = _locationMapper.Map(locations);
                    return Ok(locationsDtos);
            }

            return NotFound();
        }

        // [Authorize]
        [HttpGet("Forecast/{key}")]
        public async Task<IActionResult> GetForecast(int key)
        {
            if (key<=0) return NotFound();

            var forecast = await _weatherService.GetForecast(key);

            if (forecast != null)
            {
                var forecastDto = _forecastMapper.Map(forecast);
                return Ok(forecastDto);
            }

            return NotFound();
        }
    }
}
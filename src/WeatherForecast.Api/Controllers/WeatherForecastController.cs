using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Api.Models.Entities;
using WeatherForecast.Api.Services;

namespace WeatherForecast.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly WeatherService _weatherService;

        public WeatherController(ILogger<WeatherController> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet("Locations/{startString}")]
        public async Task<IActionResult> Get(string startString)
        {
            if (string.IsNullOrEmpty(startString)) return NotFound();

            var locations = await _weatherService.GetMatchingLocations(startString);

            if (locations.Any())
            {
                //TODO Get Locations
              //  var locationsDtos = 
                    return Ok();
            }

            return NotFound();
        }

        [HttpGet("Forecast/{key}")]
        public async Task<IActionResult> Get(int key)
        {
            if (key==0) return NotFound();

            var locations = await _weatherService.GetForecast(key);

            if (locations.Any())
            {
                //TODO Get Locations
                //  var locationsDtos = 
                return Ok();
            }

            return NotFound();
        }
    }
}
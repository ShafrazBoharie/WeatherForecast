using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WeatherForecast.Api.Models.Dto;
using WeatherForecast.App.Models;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;


namespace WeatherForecast.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _logger.LogInformation("Hello Logger-Trace");
            _logger.LogInformation("Hello Logger-Information");
            _logger.LogError("Hello Logger-Exception");
            var cc= configuration["WeatherAPI"];
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MatchingLocations(string keyword)
        { if (string.IsNullOrEmpty(keyword) || keyword.Trim().Length <= 2) return Json("");
            var locations = GetLocations(keyword);
            return Json(locations);
        }


        public IActionResult GetForecast(string locationId)
        {
            if (string.IsNullOrEmpty(locationId)) return Json("");
            var forecast = GetForecastData(locationId);
            return Json(forecast);
        }

        private List<LocationDto> GetLocations(string keyword)
        {
            var location = new List<LocationDto>();
            location.Add(new LocationDto { Key = "123",Name = "London",Code = "Lo"});
            location.Add(new LocationDto { Key = "124", Name = "Bristol", Code = "Br" });
            location.Add(new LocationDto { Key = "125", Name = "Cardiff", Code = "Cr" });

            return location;
        }

        private ForecastDto GetForecastData(string locationId)
        {
            var forecast = new ForecastDto();
            forecast.Category = "Sunny";
            forecast.Description = "Today is sunny weather";
            forecast.MaxTemperature = 46;
            forecast.MinTemperature = 28;
            
            return forecast;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using WeatherForecast.Api.Models.Dto;
using WeatherForecast.App.Models;
using WeatherForecast.App.Services;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;


namespace WeatherForecast.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWeatherForecastService _weatherForecastService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
            _logger.LogInformation("Hello Logger-Trace");
            _logger.LogInformation("Hello Logger-Information");
            _logger.LogError("Hello Logger-Exception");
            var cc= configuration["WeatherAPI"];
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MatchingLocations(string keyword)
        {
            if (string.IsNullOrEmpty(keyword) || keyword.Trim().Length <= 2)
            {
                return Json("");
            }
            var locations = await _weatherForecastService.GetMatchingLocations(keyword);
            return Json(locations);
        }

        public async Task<IActionResult> GetForecast(string locationId)
        {
            if (string.IsNullOrEmpty(locationId)) return Json("");
            var forecast = await _weatherForecastService.GetForecast(locationId);
            return Json(forecast);
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
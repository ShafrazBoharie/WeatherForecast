using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WeatherForecast.App.Models;

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

        public async Task<IActionResult> GetLocations(string keyword)
        {
            if (keyword.Trim().Length <= 2) return View();

            var location = new List<LocationModel>();
            location.Add(new LocationModel{Key = 123,Location = "London",Code = "Lo".ToUpper()});
            location.Add(new LocationModel { Key = 124, Location = "Bristol", Code = "Br".ToUpper() });
            location.Add(new LocationModel { Key = 125, Location = "Cardiff", Code = "Cr".ToUpper() });
            return Json(location);
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
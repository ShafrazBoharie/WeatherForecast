
namespace WeatherForecast.Api.Models.Dto
{
    public class ForecastDto
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public double MaxTemperature { get; set; }
        public double MinTemperature { get; set; }
    }
}

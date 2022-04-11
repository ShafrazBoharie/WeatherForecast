
using System.Text.Json.Serialization;

namespace WeatherForecast.Api.Models.Dto
{
    public class ForecastDto
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("maxTemperature")]
        public double MaxTemperature { get; set; }
        [JsonPropertyName("minTemperature")]
        public double MinTemperature { get; set; }
    }
}

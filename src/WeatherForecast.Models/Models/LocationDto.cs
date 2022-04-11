using System.Text.Json.Serialization;

namespace WeatherForecast.Api.Models.Dto
{
    public class LocationDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{Name}, {Code.ToUpper()}";
            }
        }
    }
}

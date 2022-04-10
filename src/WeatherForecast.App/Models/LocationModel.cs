namespace WeatherForecast.App.Models
{
    public class LocationModel
    {
        public int Key { get; set; }
        public string Location { get; set; }
        public string Code { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{Location}, {Code}";
            }
        }
    }
}

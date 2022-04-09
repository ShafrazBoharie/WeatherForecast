using WeatherForecast.Api.Models.Entities;

namespace WeatherForecast.Api.Services
{
    public class WeatherService
    {
        public WeatherService()
        {
            
        }

        public async Task<List<Location>> GetMatchingLocations(string startString)
        {
            throw new NotImplementedException();
        }
         
        public async Task<List<Location>> GetForecast(int key)
        {
            throw new NotImplementedException();
        }
    }
}

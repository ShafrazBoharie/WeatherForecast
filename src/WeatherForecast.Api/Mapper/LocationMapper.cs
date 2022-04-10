using WeatherForecast.Api.Models.Dto;
using WeatherForecast.Api.Models.Entities;

namespace WeatherForecast.Api.Mapper
{
    public class LocationMapper
    {
        public IEnumerable<LocationDto> Map(List<Location> locations)
        {
            if (locations == null) return new List<LocationDto>();
            return locations.Select(x => new LocationDto
            {
                Key = x.Key,
                Code = x.AdministrativeArea.ID,
                Name = x.LocalizedName
            });
        }
    }
}

using WeatherForecast.Api.Models.Dto;
using WeatherForecast.Api.Models.Entities;

namespace WeatherForecast.Api.Mapper
{
    public class ForecastMapper
    {
        public virtual ForecastDto Map(Forecast forecast)
        {
            if (forecast is null || forecast.DailyForecasts is null || forecast.Headline is null) return new ForecastDto();
            return  new ForecastDto()
            {
                Category = forecast.Headline.Category,
                Description = forecast.Headline.Text,
                MaxTemperature = forecast.DailyForecasts.FirstOrDefault().Temperature.Maximum.Value,
                MinTemperature = forecast.DailyForecasts.FirstOrDefault().Temperature.Minimum.Value
            };
        }
    }
}

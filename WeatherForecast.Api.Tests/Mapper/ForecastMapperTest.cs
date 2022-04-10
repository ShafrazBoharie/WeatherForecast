using AutoBogus;
using FluentAssertions;
using System.Linq;
using WeatherForecast.Api.Mapper;
using WeatherForecast.Api.Models.Dto;
using WeatherForecast.Api.Models.Entities;
using Xunit;

namespace WeatherForecast.Api.Tests.Mapper
{
    public class ForecastMapperTest
    {
        [Fact]
        public void WhenForecastIsEmptyForecastMapperShouldReturnEmptyForecastDtoCollection()
        {
            var input = new Forecast();

            var sut = new ForecastMapper();

            var result = sut.Map(input);

            result.Should().BeEquivalentTo(new ForecastDto());
        }

        [Fact]
        public void WhenForecastLocationIsNullForecastMapperShouldReturnEmptyForecastDtoCollection()
        {
            Forecast input = null;

            var sut = new ForecastMapper();

            var result = sut.Map(input);

            result.Should().BeEquivalentTo(new ForecastDto());
        }

        [Fact]
        public void WhenForecastHasDataMapperShouldReturnForecastDto()
        {
            var input = AutoFaker.Generate<Forecast>();

            var sut = new ForecastMapper();

            var result = sut.Map(input);

            
            result.Should().BeEquivalentTo(new ForecastDto()
            {
                Category = input.Headline.Category,
                Description = input.Headline.Text,
                MaxTemperature = input.DailyForecasts.FirstOrDefault().Temperature.Maximum.Value,
                MinTemperature = input.DailyForecasts.FirstOrDefault().Temperature.Minimum.Value
            });
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBogus;
using FluentAssertions;
using WeatherForecast.Api.Mapper;
using WeatherForecast.Api.Models.Dto;
using WeatherForecast.Api.Models.Entities;
using Xunit;

namespace WeatherForecast.Api.Tests.Mapper
{
    public class LocationMapperTests
    {
        [Fact]
        public void WhenLocationIsEmptyItShouldReturnEmptyLocationDtoCollection()
        {
            var input = new List<Location>();

            var sut = new LocationMapper();

            var result = sut.Map(input);

            result.Count().Should().Be(0);
        }

        [Fact]
        public void WhenLocationIsNulltShouldReturnEmptyLocationDtoCollection()
        {
            List<Location> input = null;

            var sut = new LocationMapper();

            var result = sut.Map(input);

            result.Count().Should().Be(0);
        }

        [Fact]
        public void WhenLocationHasLocationObjectLocationMapperShouldMapperToLocationDtoCollection()
        {
            var input = AutoFaker.Generate<Location>(3);

            var sut = new LocationMapper();

            var result = sut.Map(input);

            result.Count().Should().Be(3);

            result.First().Should().BeEquivalentTo(new LocationDto
            {
                Key = input[0].Key,
                Code = input[0].AdministrativeArea.ID,
                Name = input[0].LocalizedName
            });
        }
    }
}

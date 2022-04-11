using System.Collections.Generic;
using System.Threading.Tasks;
using AutoBogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecast.Api.Controllers;
using WeatherForecast.Api.Mapper;
using WeatherForecast.Api.Models.Dto;
using WeatherForecast.Api.Models.Entities;
using WeatherForecast.Api.Services;
using Xunit;

namespace WeatherForecast.Api.Tests.Controllers
{
    public class WeatherControllerTests
    {
        private WeatherController _sut;
        private Mock<ILogger<WeatherController>> _logger;
        private Mock<IWeatherService> _weatherService;
        private Mock<ForecastMapper> _forecastMapper;
        private Mock<LocationMapper> _locationMapper;

        public WeatherControllerTests()
        {
            _logger = new Mock<ILogger<WeatherController>>();
            _weatherService = new Mock<IWeatherService>();
            _forecastMapper= new Mock<ForecastMapper>();
            _locationMapper= new Mock<LocationMapper>();
            ConfigureController();
        }


        [Fact]
        private async Task WhenMatchingLocationsFoundForGivenKeywordEndPointShouldReturn200()
        {

            _weatherService.Setup(x => x.GetMatchingLocations(It.IsAny<string>())).ReturnsAsync(AutoFaker.Generate<Location>(3));
            _locationMapper.Setup(x => x.Map(It.IsAny<List<Location>>())).Returns(AutoFaker.Generate<LocationDto>(3));
            
            var result = await _sut.GetLocations("lon") as OkObjectResult;
            result.StatusCode.Should().Be(200);
            
        }

        [Fact]
        private async Task WhenNoMatchingLocationsFoundForGivenKeywordEndPointShouldReturn404()
        {
            _weatherService.Setup(x => x.GetMatchingLocations(It.IsAny<string>())).ReturnsAsync(new List<Location>());
            var result = await _sut.GetLocations("111") as NotFoundResult;
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        private async Task WhenMatchingForecastFoundForGivenKeywordEndPointShouldReturn200()
        {

            _weatherService.Setup(x => x.GetForecast(It.IsAny<int>())).ReturnsAsync(AutoFaker.Generate<Forecast>());
            _forecastMapper.Setup(x => x.Map(It.IsAny<Forecast>())).Returns(AutoFaker.Generate<ForecastDto>());

            var result = await _sut.GetForecast(123) as OkObjectResult;
            result.StatusCode.Should().Be(200);

        }

        [Fact]
        private async Task WhenNoMatchingForecastNotFoundForGivenKeywordEndPointShouldReturnEmptyForecastWith200()
        {
            _weatherService.Setup(x => x.GetForecast(It.IsAny<int>())).ReturnsAsync((Forecast)null);
            _forecastMapper.Setup(x => x.Map(It.IsAny<Forecast>())).Returns(new ForecastDto());
            var result = await _sut.GetForecast(111) as NotFoundResult ;
            result.StatusCode.Should().Be(404);
        }


        [Fact]
        private async Task WhenNoMatchingForecastNotFoundForGivenKeyWordReturnEmptyForecastWith404()
        {
            var result = await _sut.GetForecast(-111) as NotFoundResult;
            result.StatusCode.Should().Be(404);
        }


        private void ConfigureController()
        {
            var httpContext = new DefaultHttpContext();
            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);

            _sut = new WeatherController(_logger.Object, _weatherService.Object, _forecastMapper.Object,
                _locationMapper.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<string>())).Returns("http://test/api");
        }
    }
}

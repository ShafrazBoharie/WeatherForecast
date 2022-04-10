using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoBogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Protected;
using Newtonsoft.Json;
using WeatherForecast.Api.Models;
using WeatherForecast.Api.Models.Entities;
using WeatherForecast.Api.Services;
using Xunit;

namespace WeatherForecast.Api.Tests.Services;

public class WeatherServicesTests
{
    private const string _testUrl = "http://testUrl/api";

    [Theory]
    [InlineData("Lon")]
    public async Task WhenMatchingStartStringHasValidLocationsGetLocationShouldReturnCollectionOfLocations(
        string keyword)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        var locationResponseContent = LocationResponseContent();
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/locations/v1/cities/autocomplete?apikey={moqOptions.Value.AccuWeatherKey}&q={keyword.Trim()}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(locationResponseContent))
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetMatchingLocations(keyword);

        result.Count.Should().Be(3);
        result[0].Key.Should().NotBeNull();
        result[1].Key.Should().NotBeNull();
        result[2].Key.Should().NotBeNull();
    }

    [Theory]
    [InlineData("xxx")]
    public async Task WhenKeywordDoesNotMatchWithAnyLocationsGetLocationShouldReturnEmptyCollections(string keyword)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        var emptyLocationResponseContent = new List<Location>();
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/locations/v1/cities/autocomplete?apikey={moqOptions.Value.AccuWeatherKey}&q={keyword.Trim()}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(emptyLocationResponseContent))
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetMatchingLocations(keyword);

        result.Should().BeEquivalentTo(new List<Location>());
    }

    [Theory]
    [InlineData("lon")]
    public async Task WhenThereIsAnExceptionGetLocationShouldReturnException(string locationKey)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        var emptyLocationResponseContent = new List<Location>();
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/locations/v1/cities/autocomplete?apikey={moqOptions.Value.AccuWeatherKey}&q={locationKey.Trim()}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetMatchingLocations(locationKey);

        result.Should().BeEquivalentTo(new List<Location>());

        _logger.Verify(
            m => m.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Error retrieving Locations"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }

    [Theory]
    [InlineData("lon")]
    public async Task WhenThereIsAnExceptionGetLocationShouldReturnHttpRequestTries(string keyword)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        var emptyLocationResponseContent = new List<Location>();
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/locations/v1/cities/autocomplete?apikey={moqOptions.Value.AccuWeatherKey}&q={keyword.Trim()}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetMatchingLocations(keyword);

        result.Should().BeEquivalentTo(new List<Location>());

        _logger.Verify(
            m => m.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Error retrieving Locations"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

        handler.Protected().As<IHttpMessageHandler>()
            .Verify(x =>
                    x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()),
                Times.Exactly(4)); //TODO: Investigate this. Should return 3 times 
    }

    [Theory]
    [InlineData(1234)]
    public async Task WhenLocationKeyIsValidGetLocationShouldReturnForecast(int locationKey)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        var forecastResponseContent = ForecastResponseContent();
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/forecasts/v1/daily/1day/{locationKey}?apikey={moqOptions.Value.AccuWeatherKey}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(forecastResponseContent))
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetForecast(locationKey);

        result.Should().NotBeNull();
        result.DailyForecasts.Should().NotBeNull();
        result.Headline.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-1)]
    public async Task WhenLocationKeyDoesNotMatchWithAnyLocationsGetForecastShouldReturnEmptyForecast(int locationKey)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        Forecast nullForecastResponseContent = null;
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/forecasts/v1/daily/1day/{locationKey}?apikey={moqOptions.Value.AccuWeatherKey}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(nullForecastResponseContent))
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetForecast(locationKey);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData(1234)]
    public async Task WhenThereIsAnExceptionGetForecastShouldReturnNullObject(int locationKey)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        Forecast nullForecastResponseContent = null;
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/forecasts/v1/daily/1day/{locationKey}?apikey={moqOptions.Value.AccuWeatherKey}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetForecast(locationKey);

        result.Should().BeNull();

        _logger.Verify(
            m => m.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Error retrieving Forecast"),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }

    [Theory]
    [InlineData(1234)]
    public async Task WhenThereIsAnExceptionGetForecastShouldTryHttpRequestTries(int locationKey)
    {
        var _logger = new Mock<ILogger<WeatherService>>();
        Forecast nullForecastResponseContent = null;
        var moqOptions = MoqOptions(_testUrl);

        var handler = new Mock<HttpMessageHandler>();
        var moqFactory = mockHttpClientFactory(handler, _testUrl);

        handler.Protected().As<IHttpMessageHandler>()
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri ==
                    new Uri(
                        $"{_testUrl}/forecasts/v1/daily/1day/{locationKey}?apikey={moqOptions.Value.AccuWeatherKey}")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var sut = new WeatherService(moqFactory, moqOptions, _logger.Object);

        var result = await sut.GetForecast(locationKey);

        result.Should().BeNull();


        handler.Protected().As<IHttpMessageHandler>()
            .Verify(x =>
                    x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()),
                Times.Exactly(4)); //TODO: Investigate this. Should return 3 times 
    }


    private static IHttpClientFactory mockHttpClientFactory(Mock<HttpMessageHandler> handler, string testUrl)
    {
        var moqFactory = handler.CreateClientFactory();
        Mock.Get(moqFactory).Setup(x => x.CreateClient("AccuWeather"))
            .Returns(() =>
            {
                var client = handler.CreateClient();
                client.BaseAddress = new Uri(testUrl);
                return client;
            });
        return moqFactory;
    }

    private static IOptions<AccuWeather> MoqOptions(string testUrl)
    {
        var moqOptions = Options.Create(new AccuWeather
        {
            AccuWeatherKey = "12345678",
            AccuWeatherHost = testUrl
        });
        return moqOptions;
    }


    private List<Location> LocationResponseContent()
    {
        return AutoFaker.Generate<List<Location>>();
    }


    private Forecast ForecastResponseContent()
    {
        return AutoFaker.Generate<Forecast>();
    }
}
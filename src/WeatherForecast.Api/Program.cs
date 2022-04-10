using Azure.Identity;
using Microsoft.Extensions.Options;
using WeatherForecast.Api.Mapper;
using WeatherForecast.Api.Models;
using WeatherForecast.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var configRoot = config.Build();

    var keyVaultName = configRoot["KeyVault:Name"];
    config.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
    config.AddUserSecrets<Program>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ForecastMapper>();
builder.Services.AddSingleton<LocationMapper>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

var accueWeatherHost = builder.Configuration.GetSection("AccuWeatherHost").Value.ToString();
var accueWeatherKey = builder.Configuration.GetSection("AccuWeatherKey").Value.ToString();
builder.Services.AddSingleton<AccuWeatherSettings>((_) =>
    {
        return new AccuWeatherSettings
        {
            AccuWeatherHost = accueWeatherHost,
            AccuWeatherKey = accueWeatherKey
        };
    }
);

builder.Services.AddHttpClient("AccuWeatherSettings", (context,client) =>
{
    client.BaseAddress = new Uri(accueWeatherHost);
    client.DefaultRequestHeaders.Add("Accept","application/json");
});

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


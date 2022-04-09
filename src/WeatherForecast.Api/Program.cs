using WeatherForecast.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AccuWeather>(builder.Configuration.GetSection("AccuWeather"));

builder.Services.AddHttpClient("AccuWeather", client =>
{
    client.BaseAddress = new Uri("https://dataservice.accuweather.com");
    client.DefaultRequestHeaders.Add("Accept","application/json");
});

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

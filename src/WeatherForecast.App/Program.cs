using System.Diagnostics;
using System.Reflection;
using System.Security.Policy;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using WeatherForecast.App.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.ApplicationInsights;
using WeatherForecast.App.Services;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("WeatherForecastAppContextConnection");builder.Services.AddDbContext<WeatherForecastAppContext>(options =>
    options.UseSqlServer(connectionString));builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<WeatherForecastAppContext>();

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var configRoot = config.Build();
            var keyVaultName = configRoot["KeyVault:Name"];
            config.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
            config.AddUserSecrets<Program>();
        });


// Add services to the container.

builder.Services.AddApplicationInsightsTelemetry();
builder.Host.ConfigureLogging((context, builder) =>
{
    
    builder.AddFilter<ApplicationInsightsLoggerProvider>(
        typeof(Program).FullName, LogLevel.Trace);
    builder.AddConsole();
    builder.AddApplicationInsights(context.Configuration["ApplicationInsights:InstrumentationKey"]);
});

builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddMvc(option =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    option.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

builder.Services.AddHttpClient("api", (context, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7146");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

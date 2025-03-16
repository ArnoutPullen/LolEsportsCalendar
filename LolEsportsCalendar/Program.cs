using GoogleCalendarApiClient.Services;
using LolEsportsApiClient.Options;
using LolEsportsCalendar;
using LolEsportsCalendar.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("appsettings.json");

// Configure logging
builder.Services.AddLogging(config => {
	config.AddConsole().AddConfiguration(builder.Configuration.GetSection("Logging"));
});

builder.Services.AddOptions<LolEsportsOptions>().BindConfiguration("LolEsports").ValidateOnStart();

// Configure services
builder.Services.AddGoogleCalendarService();
builder.Services.AddHostedService<BackgroundHostedService>();
builder.Services.AddLeagueEsportService(builder.Configuration.GetSection("LolEsports"));

// Run
await builder.Build().RunAsync();
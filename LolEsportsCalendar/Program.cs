using GoogleCalendarApiClient.Services;
using LolEsportsApiClient.Options;
using LolEsportsCalendar;
using LolEsportsCalendar.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddLogging(config => {
	config.ClearProviders();
    config.AddSerilog(new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration) // Read from appsettings.json
        .CreateLogger());
});

builder.Services.AddOptions<LolEsportsOptions>().BindConfiguration(LolEsportsOptions.SectionName).ValidateOnStart();

// Configure services
builder.Services.AddGoogleCalendarService();
builder.Services.AddHostedService<HourlyBackgroundService>();
builder.Services.AddLeagueEsportService(builder.Configuration.GetSection(LolEsportsOptions.SectionName));

// Run
await builder.Build().RunAsync();
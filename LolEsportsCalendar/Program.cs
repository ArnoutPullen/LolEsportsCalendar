using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleCalendarApiClient.Services;
using LolEsportsApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LolEsportsCalendar
{
	class Program
	{
		static async Task Main(string[] args)
		{
			// Get app configuration
			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
			var configuration = configurationBuilder.AddJsonFile("appsettings.json").Build();

			// Collect app services
			var serviceCollection = new ServiceCollection();

			// Register console app
			serviceCollection.AddSingleton<ConsoleApp>();
			// Register logging
			serviceCollection.AddLogging(config => {
				config.AddConsole().AddConfiguration(configuration);
			});

			await ConfigureServices(serviceCollection, configuration);

			// Run
			var serviceProvider = serviceCollection.BuildServiceProvider();
			var consoleApp = serviceProvider.GetRequiredService<ConsoleApp>();
			await consoleApp.RunAsync();
		}

		public async static Task ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			// Google Calendar API
			var userCredential = await GetUserCredentialAsync();

			services.AddSingleton<GoogleCalendarService>();
			services.AddSingleton<CalendarListService>();
			services.AddSingleton<CalendarsService>();
			services.AddSingleton<EventsService>();
			services.AddSingleton(_ => new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = userCredential,
				ApplicationName = "LolEsportsCalendar"
			}));

			// LolEsports API
			services.AddLeagueEsportService(configuration.GetSection("LolEsports"));
		}

		static async Task<UserCredential> GetUserCredentialAsync()
		{
			using var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read);

			// The file token.json stores the user's access and refresh tokens, and is created
			// automatically when the authorization flow completes for the first time.
			GoogleClientSecrets googleClientSecrets = await GoogleClientSecrets.FromStreamAsync(stream);

			return await GoogleWebAuthorizationBroker.AuthorizeAsync(
				googleClientSecrets.Secrets,
				new string[] { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents },
				"user",
				CancellationToken.None,
				new FileDataStore("token.json", true));
		}
	}

	public static class LeagueServiceCollection
	{
		public static IServiceCollection AddLeagueEsportService(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton<LolEsportsService>();
			services.AddHttpClient<LolEsportsClient>((serviceProvider, httpClient) => {
				httpClient.BaseAddress = new Uri(configuration.GetValue<string>("BaseUrl"));
				httpClient.DefaultRequestHeaders.Add("x-api-key", configuration.GetValue<string>("ApiKey"));
			});

			return services;
		}
	}
}

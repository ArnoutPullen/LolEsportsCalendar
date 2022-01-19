using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleCalendarApiClient.Services;
using LolEsportsApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LolEsportsCalendar
{
	class Config
	{
		
	}

	class Program
	{
		static async Task Main(string[] args)
		{
			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
			var configuration = configurationBuilder.AddJsonFile("appsettings.json").Build();
			
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<ConsoleApp>();

			await ConfigureServices(serviceCollection, configuration);

			var serviceProvider = serviceCollection.BuildServiceProvider();

			var consoleApp = serviceProvider.GetRequiredService<ConsoleApp>();
			await consoleApp.RunAsync();
		}

		public async static Task ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			var userCredential = await GetUserCredentialAsync();

			services.AddSingleton(_ => new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = userCredential,
				ApplicationName = "LolEsportsCalendar"
			}));

			services.AddSingleton<GoogleCalendarService>();
			services.AddSingleton<LolEsportsService>();
			services.AddHttpClient<LolEsportsClient>((httpClient) => {
				var lolEsportsConfig = configuration.GetSection("LolEsports");

				httpClient.BaseAddress = new Uri(lolEsportsConfig.GetValue<string>("BaseUrl"));
				httpClient.DefaultRequestHeaders.Add("x-api-key", lolEsportsConfig.GetValue<string>("ApiKey"));
			});

			services.AddSingleton<CalendarListService>();
			services.AddSingleton<CalendarsService>();
			services.AddSingleton<EventsService>();
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
}

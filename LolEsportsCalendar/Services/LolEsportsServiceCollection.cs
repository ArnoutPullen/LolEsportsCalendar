using LolEsportsApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LolEsportsCalendar.Services
{
	public static class LolEsportsServiceCollection
	{
		public static IServiceCollection AddLeagueEsportService(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<LolEsportsOptions>(configuration);
			services.AddSingleton<LolEsportsService>();
			services.AddHttpClient<LolEsportsClient>((serviceProvider, httpClient) => {
				LolEsportsOptions lolEsportOptions = configuration.Get<LolEsportsOptions>();

				httpClient.BaseAddress = new Uri(lolEsportOptions.BaseUrl);
				httpClient.DefaultRequestHeaders.Add("x-api-key", lolEsportOptions.ApiKey);
			});

			return services;
		}
	}
}

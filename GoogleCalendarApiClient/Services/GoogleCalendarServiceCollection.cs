using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCalendarApiClient.Services
{
    public static class GoogleCalendarServiceCollection
    {
        public static IServiceCollection AddGoogleCalendarService(this IServiceCollection services)
        {
            // Google Calendar API
            var userCredential = GetUserCredentialAsync().GetAwaiter().GetResult();

            services.AddSingleton<GoogleCalendarService>();
            services.AddSingleton<CalendarListService>();
            services.AddSingleton<CalendarsService>();
            services.AddSingleton<EventsService>();
            services.AddSingleton(_ => new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential,
                ApplicationName = "LolEsportsCalendar"
            }));

            return services;
        }

        public static async Task<UserCredential> GetUserCredentialAsync()
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

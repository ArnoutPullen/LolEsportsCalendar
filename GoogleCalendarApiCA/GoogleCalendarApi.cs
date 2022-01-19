using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleCalendarApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Calendar.v3.CalendarListResource;
using static Google.Apis.Calendar.v3.CalendarsResource;

namespace GoogleCalendarApi
{
    public class GoogleCalendarApi
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents };
        static string ApplicationName = "LolEsportsCalendar";
        static CalendarService calendarService;
        static Dictionary<string, string> calendarLookup = new Dictionary<string, string>();

        public static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // View
            CalendarListService calendarListService = new CalendarListService(calendarService);
            CalendarList calendarList = calendarListService.ViewCalendarList();

            // Add calendars to lookup
            foreach (var c in calendarList.Items)
            {
                if (c.Summary == "LEC") {

			    }
                calendarLookup.Add(c.Id, c.Summary);
                Console.WriteLine("{0} - {1}", c.Id, c.Summary);
            }

            var calendarsService = new CalendarsService(calendarService);

            // Check if calendar exist
            foreach (var c in calendarList.Items)
            {
                var exists = calendarLookup.FirstOrDefault(x => x.Key == c.Summary);
                if (false) {
                    Calendar calendar = new Calendar
                    {
                        Summary = "LEC",
                        Description = "LEC / EUROPE",
                        // ETag = "Test",
                        // Kind = "Test",
                        // Location = "Test",
                        // TimeZone = "Europe/Amsterdam"
                    };
                    var newCalendar = calendarsService.InsertCalendar(calendar);
                }
                Console.WriteLine("{0} - {1}", c.Id, c.Summary);
            }

            // Testing();

            Console.Read();
        }

        public static void Testing()
        {
            /**
              * Calendar
              */
            var calendarsService = new CalendarsService(calendarService);

            Calendar calendar = new Calendar
            {
                Id = "98767991302996019",
                Summary = "LEC",
                Description = "LEC / EUROPE",
                // ETag = "Test",
                // Kind = "Test",
                // Location = "Test",
                // TimeZone = "Europe/Amsterdam"
            };

            var singleCalendar = calendarsService.ViewCalendar(calendar.Id);
            var newCalendar = calendarsService.InsertCalendar(calendar);

            /**
             * CalendarList
             */
            CalendarListService calendarListService = new CalendarListService(calendarService);
            CalendarListEntry calendarListEntry = new CalendarListEntry
            {
                // Id = "98767991302996019",
                Summary = "LEC"
            };

            // View
            CalendarList calendarList = calendarListService.ViewCalendarList();
            foreach (var c in calendarList.Items)
            {
                Console.WriteLine("{0} - {1}", c.Id, c.Summary);
            }

            // Insert
            CalendarListEntry newCalendarListEntry = calendarListService.InsertCalendarList(calendarListEntry);
            Console.WriteLine(newCalendarListEntry);

            // PrintUpcomingEvents();
        }

        public static void PrintUpcomingEvents()
        {
            // Define parameters of request.
            EventsResource.ListRequest request = calendarService.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }
        }
    }
}

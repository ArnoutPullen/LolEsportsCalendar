# Setup
Follow [.NET quickstart](https://developers.google.com/calendar/api/quickstart/dotnet#step_2_set_up_the_sample) for GoogleCalendarAPI.
Go to [LolEsports](https://lolesports.com/) and find your api-key, paste it inside lolesports-credentials.json

# Todo
## GoogleCalendarService.InsertOrUpdateEvent
- Usecase tested: Requires event.Id and evenId to be the same.
- Usecase untested: Change event.Id
## GoogleCalendarService
- Calendar.InsertOrUpdate

## Caching
- Leagues caching LeagueId=>LeagueName
- Calendars caching Name=>Id
- https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-6.0

# Disclaimer
LolEsports.com does not have any public information about their API.
Please be carefull and use at your own risk.

# References
- https://developers.google.com/calendar/api/v3/reference/
- https://developers.google.com/resources/api-libraries/documentation/calendar/v3/csharp/latest/classGoogle_1_1Apis_1_1Calendar_1_1v3_1_1Data_1_1Calendar.html
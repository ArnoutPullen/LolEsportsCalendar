#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["LolEsportsCalendar/LolEsportsCalendar.csproj", "LolEsportsCalendar/"]
COPY ["GoogleCalendarApiClient/GoogleCalendarApiClient.csproj", "GoogleCalendarApiClient/"]
COPY ["LolEsportsApiClient/LolEsportsApiClient.csproj", "LolEsportsApiClient/"]
RUN dotnet restore "LolEsportsCalendar/LolEsportsCalendar.csproj"
COPY . .
WORKDIR "/src/LolEsportsCalendar"
RUN dotnet build "LolEsportsCalendar.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LolEsportsCalendar.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LolEsportsCalendar.dll"]
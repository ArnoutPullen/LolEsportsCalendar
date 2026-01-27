FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["LolEsportsCalendar/LolEsportsCalendar.csproj", "LolEsportsCalendar/"]
COPY ["GoogleCalendarApiClient/GoogleCalendarApiClient.csproj", "GoogleCalendarApiClient/"]
COPY ["LolEsportsApiClient/LolEsportsApiClient.csproj", "LolEsportsApiClient/"]
RUN dotnet restore "./LolEsportsCalendar/LolEsportsCalendar.csproj"
COPY . .
WORKDIR "/src/LolEsportsCalendar"
RUN dotnet build "./LolEsportsCalendar.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LolEsportsCalendar.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LolEsportsCalendar.dll"]
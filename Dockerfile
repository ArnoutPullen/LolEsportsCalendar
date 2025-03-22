# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app
# Optionally, if you need to set a user, you can uncomment and define the user variable.
# USER $APP_UID

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["LolEsportsCalendar/LolEsportsCalendar.csproj", "LolEsportsCalendar/"]
COPY ["GoogleCalendarApiClient/GoogleCalendarApiClient.csproj", "GoogleCalendarApiClient/"]
COPY ["LolEsportsApiClient/LolEsportsApiClient.csproj", "LolEsportsApiClient/"]

# Restore dependencies
RUN dotnet restore "./LolEsportsCalendar/LolEsportsCalendar.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
WORKDIR "/src/LolEsportsCalendar"
RUN dotnet build "./LolEsportsCalendar.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "./LolEsportsCalendar.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy the published output from the previous stage
COPY --from=publish /app/publish .

# Set entry point for the container to run the app
ENTRYPOINT ["dotnet", "LolEsportsCalendar.dll"]
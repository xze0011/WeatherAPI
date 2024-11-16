# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies only when needed
COPY ["WeatherApi/WeatherApi.csproj", "WeatherApi/"]
RUN dotnet restore "WeatherApi/WeatherApi.csproj"

# Copy the entire project and build it
COPY ["WeatherApi/", "WeatherApi/"]
WORKDIR "/src/WeatherApi"
RUN dotnet build "WeatherApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "WeatherApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 5000

# Copy the published files from the publish stage
COPY --from=publish /app/publish .

# Start the application
ENTRYPOINT ["dotnet", "WeatherApi.dll"]

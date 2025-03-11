# Use .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the project files first (to optimize Docker caching)
COPY src/search-api/search-api.csproj search-api/
COPY src/SharedModels/SharedModels.csproj SharedModels/
COPY src/Monitoring/MonitoringService.csproj Monitoring/

ARG BUILD_CONFIGURATION=Release
# Restore dependencies (single restore is sufficient)
RUN dotnet restore search-api/search-api.csproj

# Copy the full source code AFTER restore (optimizes Docker caching)
COPY src/search-api/ search-api/
COPY src/SharedModels/ SharedModels/
COPY src/Monitoring/ Monitoring/

# Set working directory before building
WORKDIR /src/search-api

# Build and publish with production optimizations
RUN dotnet publish -c Release -o /app/publish 

# Use a lightweight .NET runtime for final deployment
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /app
COPY --from=build /app/publish .


# Set the entry point
ENTRYPOINT ["dotnet", "search-api.dll"]
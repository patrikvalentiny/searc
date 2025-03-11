# Use .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the project files first (to optimize Docker caching)
COPY src/CleanerService/CleanerService.csproj CleanerService/
COPY src/SharedModels/SharedModels.csproj SharedModels/
COPY src/Monitoring/MonitoringService.csproj Monitoring/

ARG BUILD_CONFIGURATION=Release
# **Force restore all dependencies with detailed logging**
RUN dotnet restore CleanerService/CleanerService.csproj

# Copy the full source code AFTER restore (optimizes Docker caching)
COPY src/CleanerService/ CleanerService/
COPY src/SharedModels/ SharedModels/
COPY src/Monitoring/ Monitoring/

# Set working directory before building
WORKDIR /src/CleanerService

# **Ensure all dependencies are installed**
RUN dotnet restore


# **Final build & publish**
RUN dotnet publish -c Release -o /app/publish

# Use a lightweight .NET runtime for final deployment
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /app
COPY --from=build /app/publish .


# Set the entry point
ENTRYPOINT ["dotnet", "CleanerService.dll"]
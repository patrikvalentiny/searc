# Use .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the project files first (to optimize Docker caching)
COPY src/IndexerService/IndexerService.csproj IndexerService/
COPY src/SharedModels/SharedModels.csproj SharedModels/
COPY src/Monitoring/MonitoringService.csproj Monitoring/

# **Force restore all dependencies with detailed logging**
RUN dotnet restore IndexerService/IndexerService.csproj

# Copy the full source code AFTER restore (optimizes Docker caching)
COPY src/IndexerService/ IndexerService/
COPY src/SharedModels/ SharedModels/
COPY src/Monitoring/ Monitoring/

# Set working directory before building
WORKDIR /src/IndexerService

# **Ensure all dependencies are installed**
RUN dotnet add package Microsoft.Extensions.Configuration.Binder --version 9.0.2
RUN dotnet restore --force --no-cache --verbosity detailed

# **Final build & publish**
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use a lightweight .NET runtime for final deployment
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "IndexerService.dll"]
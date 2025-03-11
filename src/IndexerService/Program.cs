using dotenv.net;
using EasyNetQ;
using Handlers;
using IndexerService.Application.Services;
using IndexerService.Infrastructure;
using IndexerService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitoring;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Load environment variables from .env file
DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 7, envFilePaths: [".env"]));

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
Console.WriteLine($"Environment: {env}");

// Configure logging and tracing
var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var zipkinUrl = Environment.GetEnvironmentVariable("ZIPKIN_URL") ?? "http://localhost:9411/api/v2/spans";
MonitoringService.SetupSerilog(seqUrl);
MonitoringService.SetupTracing(zipkinUrl);

// Get RabbitMQ connection from environment or use default for development
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
Console.WriteLine($"Connecting to RabbitMQ at: {rabbitHost}");
var bus = RabbitHutch.CreateBus($"host={rabbitHost}");
builder.Services.AddSingleton(bus);

// Configure PostgreSQL connection
var connectionString = @$"
    Host={Environment.GetEnvironmentVariable("INDEXER_DB_HOST") ?? "indexer-db"};
    Port={Environment.GetEnvironmentVariable("INDEXER_DB_PORT") ?? "5432"};
    Database={Environment.GetEnvironmentVariable("INDEXER_DB_NAME")};
    Username={Environment.GetEnvironmentVariable("INDEXER_DB_USER")};
    Password={Environment.GetEnvironmentVariable("INDEXER_DB_PASSWORD")}";
builder.Services.AddNpgsqlDataSource(connectionString);

// Register services
builder.Services.AddSingleton<IIndexerRepository, IndexerRepository>();
builder.Services.AddSingleton<IIndexerService, IndexerService.Application.Services.IndexerService>();
builder.Services.AddSingleton<IndexedMessagePublisher>();

// Register message handler for processing cleaned files
builder.Services.AddHostedService<IndexedFileHandler>();

using IHost host = builder.Build();

host.Start();
using (MonitoringService.ActivitySource.StartActivity("IndexerService"))
{
    Console.WriteLine("IndexerService is running and waiting for messages...");
}

// Wait for shutdown signal
await host.WaitForShutdownAsync();
Console.WriteLine("Shutting down IndexerService...");
await MonitoringService.Dispose();
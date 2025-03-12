using EasyNetQ;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitoring;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
Console.WriteLine($"Environment: {env}");
var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var zipkinUrl = Environment.GetEnvironmentVariable("ZIPKIN_URL") ?? "http://localhost:9411/api/v2/spans";
if (env == "Development")
{
    seqUrl = "http://localhost:5341";
    zipkinUrl = "http://localhost:9411/api/v2/spans";
}

MonitoringService.SetupSerilog(seqUrl);
MonitoringService.SetupTracing(zipkinUrl);

// Get RabbitMQ connection from environment or default to localhost for development
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
Console.WriteLine($"Connecting to RabbitMQ at: {rabbitHost}");
var bus = RabbitHutch.CreateBus($"host={rabbitHost}");
builder.Services.AddSingleton(bus);

builder.Services.AddSingleton<CleanedMessagePublisher>();
var connectionString = @$"
    Host={Environment.GetEnvironmentVariable("DB_CLEANER_HOST")};
    Port={Environment.GetEnvironmentVariable("DB_CLEANER_PORT")};
    Database={Environment.GetEnvironmentVariable("DB_CLEANER_NAME")};
    Username={Environment.GetEnvironmentVariable("DB_CLEANER_USER")};
    Password={Environment.GetEnvironmentVariable("DB_CLEANER_PASSWORD")}";
builder.Services.AddNpgsqlDataSource(connectionString);
using IHost host = builder.Build();

host.Start();
using (MonitoringService.ActivitySource.StartActivity("CleaningFiles"))
{
    var messagePublisher = host.Services.GetRequiredService<CleanedMessagePublisher>();
    var cleanerService = new CleanerService.Application.Services.CleanerService(messagePublisher);
    
    var relativePath = Environment.GetEnvironmentVariable("APP_DATA_PATH") ?? "../../data";
    Console.WriteLine($"Using data path: {relativePath}");
    var cleanedFiles = await cleanerService.CleanFilesAsync(relativePath);
    // await cleanerService.PublishCleanedFilesAsync(cleanedFiles);
}
await host.WaitForShutdownAsync();
Console.WriteLine("Shutting down cleanly...");
await MonitoringService.Dispose();
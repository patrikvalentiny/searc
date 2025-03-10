using EasyNetQ;
using Handlers;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitoring;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var zipkinUrl = Environment.GetEnvironmentVariable("ZIPKIN_URL") ?? "http://localhost:9411/api/v2/spans";
if (env == "Development")
{
    seqUrl = "http://localhost:5341";
    zipkinUrl = "http://localhost:9411/api/v2/spans";
}

MonitoringService.SetupSerilog(seqUrl);
MonitoringService.SetupTracing(zipkinUrl);

var bus = RabbitHutch.CreateBus("host=localhost");
builder.Services.AddSingleton(bus);
builder.Services.AddHostedService<CleanedFileHandler>();
builder.Services.AddSingleton<CleanedMessagePublisher>();

using IHost host = builder.Build();

host.Start();
using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService");
var messagePublisher = host.Services.GetRequiredService<CleanedMessagePublisher>();
var cleanerService = new CleanerService.Application.Services.CleanerService(messagePublisher);
var cleanedFiles = await cleanerService.CleanFilesAsync();
await cleanerService.PublishCleanedFilesAsync(cleanedFiles);
Console.ReadLine();
await MonitoringService.Dispose();


// Add monitoring and tracing
// builder.Host.AddMonitoring();
// builder.Services.AddTracing(builder.Configuration);
using Monitoring;
using Serilog;
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

using var activity = MonitoringService.ActivitySource.StartActivity("CleanerService");
var cleanerService = new CleanerService.Application.Services.CleanerService();
cleanerService.CleanFilesAsync().Wait();

await MonitoringService.Dispose();
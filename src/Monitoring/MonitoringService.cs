using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using Serilog;

using OpenTelemetry.Trace;
using System.Reflection;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;

namespace Monitoring;

public static class MonitoringService
{

    public static readonly ActivitySource ActivitySource = new("searc", "1.0.0");

    public static void AddMonitoring(this IHostBuilder builder)
    {
        builder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                .WriteTo.Seq(context.Configuration["SEQ_URL"] ?? "http://seq:5341")
                .ReadFrom.Configuration(context.Configuration);
        });
    }

    public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration)
    {

        var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "search-api";
        var version = "1.0.0";

        services.AddSingleton<MyResourceDetector>();

        services.AddOpenTelemetry().WithTracing(tracerBuilder =>
        {
            tracerBuilder
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(
                // serviceName: serviceName, serviceNamespace: "search-api", serviceVersion: version))
                configuration["ServiceName"] ?? "search-api", serviceVersion: version))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource(ActivitySource.Name)
                .AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["ZIPKIN_URL"] ?? "http://zipkin:9411/api/v2/spans");
                });
        });

        return services;
    }
}

public class MyResourceDetector : IResourceDetector
{
    private readonly IWebHostEnvironment webHostEnvironment;

    public MyResourceDetector(IWebHostEnvironment webHostEnvironment)
    {
        this.webHostEnvironment = webHostEnvironment;
    }

    public Resource Detect()
    {
        return ResourceBuilder.CreateEmpty()
            .AddService(serviceName: this.webHostEnvironment.ApplicationName)
            .AddAttributes(new Dictionary<string, object> { ["host.environment"] = this.webHostEnvironment.EnvironmentName })
            .Build();
    }
}
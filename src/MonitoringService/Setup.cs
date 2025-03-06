using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using Serilog;

using OpenTelemetry.Trace;

namespace MonitoringService;

public static class Setup
{
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
        services.AddOpenTelemetry().WithTracing(tracerBuilder =>
        {
            tracerBuilder
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(configuration["ServiceName"] ?? "search-api"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["ZIPKIN_URL"] ?? "http://zipkin:9411/api/v2/spans");
                });
        });

        return services;
    }
}
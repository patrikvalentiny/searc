using System.Diagnostics;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Monitoring;
using OpenTelemetry.Context.Propagation;
using Serilog;
using SharedModels;

namespace Handlers;
public class CleanedFileHandler(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Debug("Starting CleanedFileHandler");
        await bus.PubSub.SubscribeAsync<CleanedFileDTO>("CleanedFile", async message =>
        {
            Log.Logger.Information("Received message from RabbitMQ");
            var propagator = new TraceContextPropagator();
            var headers = message.PropagationHeaders;
            var context = propagator.Extract(default, headers,  (r, key) => [r.ContainsKey(key) ? r[key].ToString() : string.Empty]).ActivityContext;
            using var activity = MonitoringService.ActivitySource.StartActivity("CleanedFileHandler", ActivityKind.Consumer, context);
            await Task.CompletedTask;
        }, cancellationToken: stoppingToken).AsTask();

        Log.Debug("Subscribed to RabbitMQ");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }


    }
}
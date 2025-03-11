using System.Diagnostics;
using EasyNetQ;
using IndexerService.Application.Services;
using Microsoft.Extensions.Hosting;
using Monitoring;
using OpenTelemetry.Context.Propagation;
using Serilog;
using SharedModels;

namespace Handlers;

public class IndexedFileHandler(IBus bus, IIndexerService indexerService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Debug("Starting IndexedFileHandler");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await bus.PubSub.SubscribeAsync<CleanedFileDTO>("CleanedFile", async message =>
                {
                    Log.Logger.Information("Received CleanedFileDTO from RabbitMQ");
                    
                    var propagator = new TraceContextPropagator();
                    var headers = message.PropagationHeaders;
                    var context = propagator.Extract(default, headers, (r, key) =>
                        [r.ContainsKey(key) ? r[key].ToString() : string.Empty]).ActivityContext;

                    using var activity = MonitoringService.ActivitySource.StartActivity(
                        "IndexerService.IndexedFileHandler", ActivityKind.Consumer, context);

                    await indexerService.ProcessFileAsync(message);
                    
                }, c => c.WithTopic("CleanedFile"), cancellationToken: stoppingToken).AsTask();

                Log.Debug("Subscribed to RabbitMQ topic 'CleanedFile'");
                break; 
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error subscribing to RabbitMQ. Retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
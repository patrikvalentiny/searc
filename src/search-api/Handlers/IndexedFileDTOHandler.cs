using System.Diagnostics;
using EasyNetQ;
using Monitoring;
using OpenTelemetry.Context.Propagation;
using Searc.SearchApi.Services;
using Serilog;
using SharedModels;

namespace SearchApi.Handlers;

public class IndexedFileDTOHandler(IBus bus, ISearchService service) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await bus.PubSub.SubscribeAsync<IndexedFileDTO>("search-api", async message =>
        {
            var propagator = new TraceContextPropagator();
            var headers = message.PropagationHeaders;
            var context = propagator.Extract(default, headers,  (r, key) => [r.ContainsKey(key) ? r[key].ToString() : string.Empty]).ActivityContext;
            using var activity = MonitoringService.ActivitySource.StartActivity("CleanedFileHandler", ActivityKind.Consumer, context);
            Log.Logger.Information("Received IndexedFileDTO from RabbitMQ");
            await Task.CompletedTask;
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
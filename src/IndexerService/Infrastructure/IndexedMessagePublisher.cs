using System.Diagnostics;
using EasyNetQ;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Serilog;
using SharedModels;

namespace IndexerService.Infrastructure;

public class IndexedMessagePublisher(IBus bus)
{
    public async Task PublishIndexedMessage(IndexedFileDTO message)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("IndexerService.PublishIndexedMessage");
        var context = activity?.Context ?? Activity.Current?.Context ?? default;
        var propagator = new TraceContextPropagator();
        var propagationContext = new PropagationContext(context, Baggage.Current);
        propagator.Inject(propagationContext, message.PropagationHeaders, (c, k, v) => c[k] = v);

        Log.Logger.Information("Publishing indexed file message to RabbitMQ with topic 'IndexedFile'");
        await bus.PubSub.PublishAsync(message, c => c.WithTopic("IndexedFile"));
    }
}
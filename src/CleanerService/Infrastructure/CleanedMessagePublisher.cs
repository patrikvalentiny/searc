using System.Diagnostics;
using EasyNetQ;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Serilog;
using SharedModels;

namespace Infrastructure;
public class CleanedMessagePublisher (IBus bus){
    public async Task PublishCleanedMessage(CleanedFileDTO message){
        using var activity = MonitoringService.ActivitySource.StartActivity("PublishCleanedMessage");
        var context = activity?.Context ?? Activity.Current?.Context ?? default;
        var propagator = new TraceContextPropagator();
        var propagationContext = new PropagationContext(context, Baggage.Current);
        propagator.Inject(propagationContext, message.PropagationHeaders , (c, k, v) => c[k] = v);
        Log.Logger.Information("Publishing message to RabbitMQ");
        await bus.PubSub.PublishAsync(message);
    }
}
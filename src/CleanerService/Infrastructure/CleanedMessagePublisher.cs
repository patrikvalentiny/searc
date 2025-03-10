using EasyNetQ;
using Serilog;
using SharedModels;

namespace Infrastructure;
public class CleanedMessagePublisher (IBus bus){
    public async Task PublishCleanedMessage(CleanedFileDTO message){
        Log.Logger.Information("Publishing message to RabbitMQ");
        await bus.PubSub.PublishAsync(message);
    }
}
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharedModels;

namespace Handlers;
public class CleanedFileHandler(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriptionResult = await bus.PubSub.SubscribeAsync<CleanedFileDTO>("CleanedFile", async message =>
        {
            Log.Logger.Information("Received message from RabbitMQ");
            await Task.CompletedTask;
        }, cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }


    }
}
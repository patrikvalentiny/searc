using EasyNetQ;
using Searc.SearchApi.Models;
using Searc.SearchApi.Services;

public class FileDetailsHandler(ISearchService service, IBus bus) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bus.PubSub.Subscribe<FileContent>("file-content", service.ProcessFileDetails);
        return Task.CompletedTask;
    }
}
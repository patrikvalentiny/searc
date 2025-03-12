using IndexerService.Infrastructure;
using IndexerService.Infrastructure.Repositories;
using Monitoring;
using Serilog;
using SharedModels;

namespace IndexerService.Application.Services;

public interface IIndexerService
{
    Task ProcessFileAsync(CleanedFileDTO cleanedFile);
}

public class IndexerService(IIndexerRepository repository, IndexedMessagePublisher publisher) : IIndexerService
{
    public async Task ProcessFileAsync(CleanedFileDTO cleanedFile)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("IndexerService.ProcessFile");
        Log.Information("Processing file: {Filename}", cleanedFile.Filename);

        var fileId = await repository.InsertFileAsync(cleanedFile.Filename, System.Text.Encoding.UTF8.GetBytes(cleanedFile.Content));

        var wordCounts = cleanedFile.Content
            .Split([' ', '\n', '\r', '.', ',', '!', '?'], StringSplitOptions.RemoveEmptyEntries)
            .GroupBy(word => word.ToLower())
            .ToDictionary(group => group.Key, group => group.Count());

        var wordIds = new Dictionary<string, int>();

        foreach (var word in wordCounts.Keys)
        {
            var wordId = await repository.InsertWordAsync(word);
            wordIds[word] = wordId;
        }

        var insertTasks = wordCounts.Select(entry =>
            repository.InsertOccurrenceAsync(wordIds[entry.Key], fileId, entry.Value));

        await Task.WhenAll(insertTasks);
        Log.Information("Finished indexing {Filename}", cleanedFile.Filename);

        var indexedFileMessage = new IndexedFileDTO
        {
            Filename = cleanedFile.Filename,
            WordCount = wordCounts,
            Content = System.Text.Encoding.UTF8.GetBytes(cleanedFile.Content)
        };

        await publisher.PublishIndexedMessage(indexedFileMessage);
    }
}
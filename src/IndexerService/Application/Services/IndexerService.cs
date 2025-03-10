using IndexerService.Infrastructure.Repositories;
using SharedModels;

namespace IndexerService.Application.Services;

public interface IIndexerService
{
    Task ProcessFileAsync(CleanedFileDTO cleanedFile);
}

public class IndexerService(IIndexerRepository repository) : IIndexerService
{
    public async Task ProcessFileAsync(CleanedFileDTO cleanedFile)
    {
        var fileId = await repository.InsertFileAsync(cleanedFile.Filename, System.Text.Encoding.UTF8.GetBytes(cleanedFile.Content));

        var wordCounts = cleanedFile.Content
            .Split(new[] { ' ', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .GroupBy(word => word.ToLower())
            .ToDictionary(group => group.Key, group => group.Count());

        foreach (var (word, count) in wordCounts)
        {
            var wordId = await repository.InsertWordAsync(word);
            await repository.InsertOccurrenceAsync(wordId, fileId, count);
        }
        
        Console.WriteLine($"Finished indexing {cleanedFile.Filename}");
    }
}
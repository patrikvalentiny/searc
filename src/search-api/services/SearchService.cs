using Monitoring;
using Searc.SearchApi.Models;
using Searc.SearchApi.Repositories;
using Serilog;
using SharedModels;

namespace Searc.SearchApi.Services;

public class SearchService(ISearchRepository repository) : ISearchService
{
    public  async Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("SearchService.SearchFilesAsync");
        return await repository.SearchAsync(query);
    }

    public async Task AddIndexFile(IndexedFileDTO file)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("SearchService.IndexFileAsync");
        var insertedFileId = await repository.InsertFileAsync(new Models.File { Name = file.Filename, Content = file.Content });
        var words = file.WordCount;
        foreach (var word in words)
        {
            var insertedWord = await repository.InsertWordAsync(new Word { Value = word.Key });
            await repository.InsertOccurrenceAsync(new Occurrence { WordId = insertedWord.Id, FileId = insertedFileId.Id, Count = word.Value });
        }
    }

    public Task<byte[]?> GetFileAsync(int fileId)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("SearchService.GetFileAsync");
        return repository.GetFileAsync(fileId);
    }
}
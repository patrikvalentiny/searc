using Monitoring;
using Searc.SearchApi.Models;
using Searc.SearchApi.Repositories;

namespace Searc.SearchApi.Services;

public class SearchService(ISearchRepository repository) : ISearchService
{
    public  async Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("SearchService.SearchFilesAsync");
        return await repository.SearchAsync(query);
    }
}
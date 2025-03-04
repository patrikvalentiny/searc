using System.Threading.Tasks;
using Searc.SearchApi.Models;
using Searc.SearchApi.Repositories;

namespace Searc.SearchApi.Services;

public class SearchService(ISearchRepository repository) : ISearchService
{
    public  async Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query)
    {
        return await repository.SearchAsync(query);
    }
}
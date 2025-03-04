using System.Threading.Tasks;
using Searc.SearchApi.Models;
using Searc.SearchApi.Repositories;

namespace Searc.SearchApi.Services;

public class SearchService(ISearchRepository repository) : ISearchService
{
    public async Task<FileDetailsDTO> ProcessFileDetails(FileContent msg)
    {
        Console.WriteLine($"Processing file details for {msg.Filename}");
        return await repository.AddAsync(msg);
    }

    public  async Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query)
    {
        return await repository.SearchAsync(query);
    }
}
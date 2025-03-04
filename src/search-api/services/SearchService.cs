using Searc.SearchApi.Models;

namespace Searc.SearchApi.Services;

public class SearchService : ISearchService
{
    public IEnumerable<FileDetailsDTO> SearchFiles(string query)
    {
        return new List<FileDetailsDTO>
        {
            new() { Filename = $"{query}.txt" },
            new() { Filename = $"file1{query}.txt" },
            new() { Filename = $"file2{query}.txt" }
        };
    }
}
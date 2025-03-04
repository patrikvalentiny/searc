using Searc.SearchApi.Models;

namespace Searc.SearchApi.Services;
public interface ISearchService
{
    Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query);
}
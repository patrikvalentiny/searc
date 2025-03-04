using Searc.SearchApi.Models;

namespace Searc.SearchApi.Services;
public interface ISearchService
{
    IEnumerable<FileDetailsDTO> SearchFiles(string query);
}
using Searc.SearchApi.Models;

namespace Searc.SearchApi.Services;
public interface ISearchService
{
    Task<FileDetailsDTO> ProcessFileDetails(FileContent msg);
    Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query);
}
using Searc.SearchApi.Models;

namespace Searc.SearchApi.Services;
public interface ISearchService
{
    Task<FileContent> GetFileContentAsync(int id);
    Task<FileDetailsDTO> ProcessFileDetails(FileContent msg);
    Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query);
}
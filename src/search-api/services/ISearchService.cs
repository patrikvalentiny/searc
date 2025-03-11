using Searc.SearchApi.Models;
using SharedModels;

namespace Searc.SearchApi.Services;
public interface ISearchService
{
    Task<IEnumerable<FileDetailsDTO>> SearchFilesAsync(string query);
    Task AddIndexFile(IndexedFileDTO file);
}
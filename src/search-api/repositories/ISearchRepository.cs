using Searc.SearchApi.Models;

namespace Searc.SearchApi.Repositories;
public interface ISearchRepository
{
    Task<FileDetailsDTO> AddAsync(FileContent msg);
    Task<IEnumerable<FileDetailsDTO>> SearchAsync(string query);
}

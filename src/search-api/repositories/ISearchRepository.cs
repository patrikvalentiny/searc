using Searc.SearchApi.Models;

namespace Searc.SearchApi.Repositories;
public interface ISearchRepository
{
    Task<IEnumerable<FileDetailsDTO>> SearchAsync(string query);
}

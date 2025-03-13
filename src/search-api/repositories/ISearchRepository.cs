using Searc.SearchApi.Models;

namespace Searc.SearchApi.Repositories;
public interface ISearchRepository
{
    Task<IEnumerable<FileDetailsDTO>> SearchAsync(string query);
    Task<Word> InsertWordAsync(Word word);
    Task<Models.File> InsertFileAsync(Models.File file);
    Task<Occurrence> InsertOccurrenceAsync(Occurrence occurrence);
    Task<byte[]?> GetFileAsync(int fileId);
}

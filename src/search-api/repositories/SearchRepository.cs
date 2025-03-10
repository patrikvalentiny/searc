using System.Data.Common;
using Searc.SearchApi.Models;
using Dapper;

namespace Searc.SearchApi.Repositories;

public class SearchRepository(DbDataSource dataSource) : ISearchRepository
{
    public async Task<IEnumerable<FileDetailsDTO>> SearchAsync(string query)
    {
        using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("SearchRepository.SearchAsync");
        var sql = @$"SELECT f.name as {nameof(FileDetailsDTO.Filename)} FROM words w
        INNER JOIN occurrences o ON w.id = o.word_id
        INNER JOIN files f ON o.file_id = f.id
        WHERE w.word LIKE @query";
        using var conn = await dataSource.OpenConnectionAsync();
        return await conn.QueryAsync<FileDetailsDTO>(sql, new { query });
    }
}
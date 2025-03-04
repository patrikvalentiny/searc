using System.Data;
using System.Data.Common;
using Searc.SearchApi.Models;
using Dapper;

namespace Searc.SearchApi.Repositories;

public class SearchRepository(DbDataSource dataSource) : ISearchRepository
{
    public Task<FileDetailsDTO> AddAsync(FileContent msg)
    {
        var sql = @$"INSERT INTO files (name, content) VALUES (@Filename, @Content) 
        RETURNING id as {nameof(FileDetailsDTO.Id)}, name as {nameof(FileDetailsDTO.Filename)}";
        using var conn = dataSource.OpenConnection();
        return conn.QuerySingleAsync<FileDetailsDTO>(sql, msg);
    }

    public async Task<IEnumerable<FileDetailsDTO>> SearchAsync(string query)
    {
        var sql = @$"SELECT f.name as {nameof(FileDetailsDTO.Filename)} FROM words w
        INNER JOIN occurrences o ON w.id = o.word_id
        INNER JOIN files f ON o.file_id = f.id
        WHERE w.word LIKE @query";
        using var conn = await dataSource.OpenConnectionAsync();
        return await conn.QueryAsync<FileDetailsDTO>(sql, new { query });
    }
}
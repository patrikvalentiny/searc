using System.Data.Common;
using Searc.SearchApi.Models;
using Dapper;
using Serilog;

namespace Searc.SearchApi.Repositories;

public class SearchRepository(DbDataSource dataSource) : ISearchRepository
{
    public async Task<byte[]?> GetFileAsync(int fileId)
    {
        // using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("SearchRepository.GetFileAsync");
        var sql = "SELECT content FROM files WHERE id = @fileId";
        using var conn = await dataSource.OpenConnectionAsync();
        
        return await conn.ExecuteScalarAsync<byte[]>(sql, new { fileId });
    }

    public async Task<Models.File> InsertFileAsync(Models.File file)
    {
        // using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("SearchRepository.InsertFileAsync");
        var sql = "INSERT INTO files (name, content) VALUES (@Name, @Content) RETURNING id";
        using var conn = await dataSource.OpenConnectionAsync();
        var id = await conn.ExecuteScalarAsync<int>(sql, file);
        file.Id = id;
        Log.Logger.Information("Inserted file with id {Id}", id);
        return file;
    }

    public async Task<Occurrence> InsertOccurrenceAsync(Occurrence occurrence)
    {
        // using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("SearchRepository.InsertOccurrenceAsync");
        var sql = "INSERT INTO occurrences (word_id, file_id, count) VALUES (@WordId, @FileId, @Count) ON CONFLICT (word_id, file_id) DO UPDATE SET count = @Count";
        using var conn = await dataSource.OpenConnectionAsync();
        await conn.ExecuteAsync(sql, occurrence);
        Log.Logger.Information("Inserted occurrence for word {WordId} in file {FileId}", occurrence.WordId, occurrence.FileId);
        return occurrence;
    }

    public async Task<Word> InsertWordAsync(Word word)
    {
        // using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("SearchRepository.InsertWordAsync");
        var sql = "WITH ins AS (INSERT INTO words (word) VALUES (@Value) ON CONFLICT (word) DO NOTHING RETURNING id) SELECT id FROM ins UNION SELECT id FROM words WHERE word = @Value";
        using var conn = await dataSource.OpenConnectionAsync();
        var id = await conn.ExecuteScalarAsync<int>(sql, word);
        word.Id = id;
        Log.Logger.Information("Inserted word {Word} with id {Id}", word.Value, id);
        return word;
    }

    public async Task<IEnumerable<FileDetailsDTO>> SearchAsync(string query)
    {
        using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("SearchRepository.SearchAsync");
        var sql = @$"SELECT f.id as {nameof(FileDetailsDTO.Id)}, f.name as {nameof(FileDetailsDTO.Filename)} FROM words w
        INNER JOIN occurrences o ON w.id = o.word_id
        INNER JOIN files f ON o.file_id = f.id
        WHERE w.word LIKE @query";
        using var conn = await dataSource.OpenConnectionAsync();
        return await conn.QueryAsync<FileDetailsDTO>(sql, new { query });
    }
}
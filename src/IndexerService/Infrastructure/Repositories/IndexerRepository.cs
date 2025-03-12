using Dapper;
using Npgsql;
using Serilog;
using System.Threading;

namespace IndexerService.Infrastructure.Repositories;

public interface IIndexerRepository
{
    Task<int> InsertFileAsync(string filename, byte[] content);
    Task<int> InsertWordAsync(string word);
    Task InsertOccurrenceAsync(int wordId, int fileId, int count);
}

public class IndexerRepository(NpgsqlDataSource dataSource) : IIndexerRepository
{
    private readonly SemaphoreSlim _semaphore = new(20); // Limit to 100 concurrent operations

    public async Task<int> InsertFileAsync(string filename, byte[] content)
    {
        await _semaphore.WaitAsync();
        try
        {
            const string sql = "INSERT INTO files (name, content) VALUES (@Filename, @Content) RETURNING id";
            using var conn = await dataSource.OpenConnectionAsync();
            
            Log.Information("Inserting file {Filename} into database", filename);
            return await conn.ExecuteScalarAsync<int>(sql, new { Filename = filename, Content = content });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<int> InsertWordAsync(string word)
    {
        await _semaphore.WaitAsync();
        try
        {
            const string sql = "INSERT INTO words (word) VALUES (@Word) ON CONFLICT (word) DO UPDATE SET word=EXCLUDED.word RETURNING id";
            using var conn = await dataSource.OpenConnectionAsync();
            
            Log.Information("Inserting word {Word} into database", word);
            return await conn.ExecuteScalarAsync<int>(sql, new { Word = word });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task InsertOccurrenceAsync(int wordId, int fileId, int count)
    {
        await _semaphore.WaitAsync();
        try
        {
            const string sql = "INSERT INTO occurrences (word_id, file_id, count) VALUES (@WordId, @FileId, @Count)";
            using var conn = await dataSource.OpenConnectionAsync();
            
            Log.Information("Inserting occurrence: WordId={WordId}, FileId={FileId}, Count={Count}", wordId, fileId, count);
            await conn.ExecuteAsync(sql, new { WordId = wordId, FileId = fileId, Count = count });
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
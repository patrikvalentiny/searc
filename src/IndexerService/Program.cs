using dotenv.net;
using IndexerService.Application.Services;
using IndexerService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using SharedModels;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Load environment variables
        DotEnv.Load(options: new DotEnvOptions(
            probeForEnv: true,
            probeLevelsToSearch: 7,
            envFilePaths: [".env"]
        ));

        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var config = context.Configuration;

                var connectionString = $"Host={Environment.GetEnvironmentVariable("INDEXER_DB_HOST")};" +
                                       $"Port={Environment.GetEnvironmentVariable("INDEXER_DB_PORT")};" +
                                       $"Database={Environment.GetEnvironmentVariable("INDEXER_DB_NAME")};" +
                                       $"Username={Environment.GetEnvironmentVariable("INDEXER_DB_USER")};" +
                                       $"Password={Environment.GetEnvironmentVariable("INDEXER_DB_PASSWORD")}";

                services.AddSingleton<NpgsqlDataSource>(_ => NpgsqlDataSource.Create(connectionString));
                services.AddSingleton<IIndexerRepository, IndexerRepository>();
                services.AddSingleton<IIndexerService, IndexerService.Application.Services.IndexerService>();
            })
            .Build();

        var service = host.Services.GetRequiredService<IIndexerService>();

        // Test file for processing
        var testFile = new CleanedFileDTO
        {
            Filename = "example.txt",
            Content = "hello world hello"
        };

        await service.ProcessFileAsync(testFile);
    }
}
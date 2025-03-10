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

        string filePath = "testfile.txt";
        if (File.Exists(filePath))
        {
            string content = await File.ReadAllTextAsync(filePath);
            var testFile = new CleanedFileDTO
            {
                Filename = "testfile.txt",
                Content = content
            };

            Console.WriteLine($"Processing file: {testFile.Filename}");
            await service.ProcessFileAsync(testFile);
            Console.WriteLine("File processing completed!");
        }
        else
        {
            Console.WriteLine($"File '{filePath}' not found. Please create it first.");
        }
    }
}
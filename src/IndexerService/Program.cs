using dotenv.net;
using IndexerService.Application.Services;
using IndexerService.Infrastructure.Repositories;
using Monitoring;

// Load environment variables from .env file in development
// Docker Compose will provide environment variables in production
DotEnv.Load(options: new DotEnvOptions(
    probeForEnv: true,
    probeLevelsToSearch: 7,
    envFilePaths: [".env"]
));

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddMonitoring();
builder.Services.AddTracing(builder.Configuration);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Add Postgres configuration
var connectionString = $"" +
                       $"Host={Environment.GetEnvironmentVariable("INDEXER_DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("INDEXER_DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("INDEXER_DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("INDEXER_DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("INDEXER_DB_PASSWORD")}";
builder.Services.AddNpgsqlDataSource(connectionString);

builder.Services.AddSingleton<IIndexerService, IIndexerService>();
builder.Services.AddSingleton<IIndexerRepository, IndexerRepository>();

builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();

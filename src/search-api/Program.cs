using System.Data;
using dotenv.net;
using Npgsql;
using Scalar.AspNetCore;
using Searc.SearchApi.Repositories;
using Searc.SearchApi.Services;
using EasyNetQ;

// Load environment variables from .env file in development
// Docker Compose will provide environment variables in production
DotEnv.Load(options: new DotEnvOptions(
    probeForEnv: true,
    probeLevelsToSearch: 7,
    envFilePaths: [".env"]
));

Console.WriteLine($"App Port: {Environment.GetEnvironmentVariable("APP_PORT")}");

var builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();
// Add Postgres configuration
var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("DB_PORT")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
builder.Services.AddNpgsqlDataSource(connectionString);

// Add RabbitMQ configuration
var rabbitMqConnectionString = $"host={Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"}";
builder.Services.AddSingleton(RabbitHutch.CreateBus(rabbitMqConnectionString));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddSingleton<ISearchService, SearchService>();
builder.Services.AddSingleton<ISearchRepository, SearchRepository>();
builder.Services.AddHostedService<FileDetailsHandler>();
// Add controllers to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCorsPolicy");
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

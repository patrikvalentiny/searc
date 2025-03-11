using dotenv.net;
using Monitoring;
using Scalar.AspNetCore;
using Searc.SearchApi.Repositories;
using Searc.SearchApi.Services;

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
var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("DB_PORT")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
builder.Services.AddNpgsqlDataSource(connectionString);

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

// Add monitoring and tracing


// Add services to the container.
builder.Services.AddSingleton<ISearchService, SearchService>();
builder.Services.AddSingleton<ISearchRepository, SearchRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

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

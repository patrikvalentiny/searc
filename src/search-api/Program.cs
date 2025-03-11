using dotenv.net;
using Monitoring;
using Scalar.AspNetCore;
using Searc.SearchApi.Repositories;
using Searc.SearchApi.Services;


var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
Console.WriteLine($"Environment: {env}");
var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var zipkinUrl = Environment.GetEnvironmentVariable("ZIPKIN_URL") ?? "http://localhost:9411/api/v2/spans";
if (env == "Development")
{
    seqUrl = "http://localhost:5341";
    zipkinUrl = "http://localhost:9411/api/v2/spans";
}

Console.WriteLine($"Connecting to Seq at: {seqUrl}");
Console.WriteLine($"Connecting to Zipkin at: {zipkinUrl}");
MonitoringService.SetupSerilog(seqUrl);
MonitoringService.SetupTracing(zipkinUrl);

// Load environment variables from .env file in development
// Docker Compose will provide environment variables in production
DotEnv.Load(options: new DotEnvOptions(
    probeForEnv: true,
    probeLevelsToSearch: 7,
    envFilePaths: [".env"]
));

var builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();


// Add Postgres configuration
var connectionString = @$"
    Host={Environment.GetEnvironmentVariable("DB_HOST")};
    Port={Environment.GetEnvironmentVariable("DB_PORT")};
    Database={Environment.GetEnvironmentVariable("DB_NAME")};
    Username={Environment.GetEnvironmentVariable("DB_USER")};
    Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}
";
builder.Services.AddNpgsqlDataSource(connectionString);

builder.Services.AddControllers();

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
await MonitoringService.Dispose();

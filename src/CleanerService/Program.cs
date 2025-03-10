using Monitoring;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add monitoring and tracing
builder.Host.AddMonitoring();
builder.Services.AddTracing(builder.Configuration);


app.UseHttpsRedirection();
app.Run();
using Microsoft.EntityFrameworkCore;
using SenCloud.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Register the database context with dependency injection
builder.Services.AddDbContext<SenCloudDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SenCloud")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
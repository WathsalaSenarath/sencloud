using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SenCloud.Api.Data;
using SenCloud.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<SenCloudDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SenCloud")));

// Send enums as text in JSON ("Pending") instead of numbers (0)
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapAppEndpoints();

app.Run();
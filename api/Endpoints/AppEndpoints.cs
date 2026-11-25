using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SenCloud.Api.Data;
using SenCloud.Api.Models;

namespace SenCloud.Api.Endpoints;

// What the client sends when creating an app
public record CreateAppRequest(string Name, string Image, int? Port);

public static partial class AppEndpoints
{
    // Kubernetes "DNS label": lowercase letters, digits, dashes; starts/ends with a letter or digit
    [GeneratedRegex("^[a-z0-9]([-a-z0-9]*[a-z0-9])?$")]
    private static partial Regex DnsLabel();

    public static void MapAppEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/apps");

        // GET /apps — list all apps, newest first
        group.MapGet("/", async (SenCloudDbContext db) =>
            await db.Apps.OrderByDescending(a => a.CreatedAt).ToListAsync());

        // GET /apps/{name} — one app, or 404
        group.MapGet("/{name}", async (string name, SenCloudDbContext db) =>
            await db.Apps.FirstOrDefaultAsync(a => a.Name == name) is { } found
                ? Results.Ok(found)
                : Results.NotFound());

        // POST /apps — validate, save, return 201 Created
        group.MapPost("/", async (CreateAppRequest request, SenCloudDbContext db) =>
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 63 || !DnsLabel().IsMatch(request.Name))
                errors["name"] = ["Use 1-63 lowercase letters, digits or '-', starting and ending with a letter or digit."];

            if (string.IsNullOrWhiteSpace(request.Image) || request.Image.Length > 255)
                errors["image"] = ["Image is required (max 255 characters), e.g. 'nginx' or 'nginx:1.27'."];

            var port = request.Port ?? 80;
            if (port is < 1 or > 65535)
                errors["port"] = ["Port must be between 1 and 65535."];

            if (errors.Count > 0)
                return Results.ValidationProblem(errors);

            if (await db.Apps.AnyAsync(a => a.Name == request.Name))
                return Results.Conflict(new { error = $"An app named '{request.Name}' already exists." });

            var app = new App { Name = request.Name, Image = request.Image.Trim(), Port = port };
            db.Apps.Add(app);
            await db.SaveChangesAsync();

            return Results.Created($"/apps/{app.Name}", app);
        });
    }
}
using AemenersolSync.Data;
using AemenersolSync.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("AEM_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A database connection string is required.");
var apiBaseUrl = Environment.GetEnvironmentVariable("AEM_API_BASE_URL")
    ?? builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("The external API base URL is required.");

builder.Services.AddDbContext<SyncDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddHttpClient<PlatformWellApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddScoped<PlatformWellSyncService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<SyncDbContext>().Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/sync", async (
    bool useDummy,
    PlatformWellApiClient apiClient,
    PlatformWellSyncService syncService,
    CancellationToken cancellationToken) =>
{
    var username = Environment.GetEnvironmentVariable("AEM_API_USERNAME");
    var password = Environment.GetEnvironmentVariable("AEM_API_PASSWORD");
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.Problem("Set AEM_API_USERNAME and AEM_API_PASSWORD environment variables.", statusCode: 500);

    var source = await apiClient.GetPlatformsAsync(username, password, useDummy, cancellationToken);
    var result = await syncService.SyncAsync(source, cancellationToken);
    return Results.Ok(result);
})
.WithName("SyncPlatformWells")
.WithSummary("Download and upsert platform/well data")
.WithDescription("Set useDummy=true to test compatibility with missing and additional JSON fields.")
.Produces<SyncResult>()
.ProducesProblem(500);

app.MapGet("/api/platforms", async (SyncDbContext database, CancellationToken cancellationToken) =>
    Results.Ok(await database.Platforms.AsNoTracking()
        .OrderBy(platform => platform.Id)
        .Select(platform => new
        {
            platform.Id,
            platform.UniqueName,
            platform.Latitude,
            platform.Longitude,
            platform.CreatedAt,
            platform.UpdatedAt,
            Wells = platform.Wells.OrderBy(well => well.Id).Select(well => new
            {
                well.Id,
                well.PlatformId,
                well.UniqueName,
                well.Latitude,
                well.Longitude,
                well.CreatedAt,
                well.UpdatedAt
            })
        })
        .ToListAsync(cancellationToken)))
    .WithName("GetPlatforms")
    .WithSummary("Read stored platforms and their wells");

app.MapGet("/api/wells", async (SyncDbContext database, CancellationToken cancellationToken) =>
    Results.Ok(await database.Wells.AsNoTracking()
        .OrderBy(well => well.Id)
        .ToListAsync(cancellationToken)))
    .WithName("GetWells")
    .WithSummary("Read all stored wells");

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.Run();

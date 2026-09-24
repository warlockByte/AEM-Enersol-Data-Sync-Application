using AemenersolSync.Configuration;
using AemenersolSync.Data;
using AemenersolSync.Services;
using Microsoft.EntityFrameworkCore;

try
{
    var settings = ApplicationSettings.Load(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
    var useDummy = args.Contains("--dummy", StringComparer.OrdinalIgnoreCase);
    var options = new DbContextOptionsBuilder<SyncDbContext>().UseSqlServer(settings.ConnectionString).Options;
    await using var dbContext = new SyncDbContext(options);
    await dbContext.Database.MigrateAsync();

    using var httpClient = new HttpClient { BaseAddress = settings.BaseUrl };
    var source = await new PlatformWellApiClient(httpClient).GetPlatformsAsync(
        settings.Username, settings.Password, useDummy, CancellationToken.None);
    var result = await new PlatformWellSyncService(dbContext).SyncAsync(source, CancellationToken.None);
    Console.WriteLine($"Sync complete ({(useDummy ? "dummy" : "actual")}): platforms " +
        $"{result.PlatformsInserted} inserted/{result.PlatformsUpdated} updated; wells " +
        $"{result.WellsInserted} inserted/{result.WellsUpdated} updated.");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Sync failed: {exception.Message}");
    return 1;
}

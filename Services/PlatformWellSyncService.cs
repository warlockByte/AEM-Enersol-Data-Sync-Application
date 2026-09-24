using AemenersolSync.Contracts;
using AemenersolSync.Data;
using AemenersolSync.Models;
using Microsoft.EntityFrameworkCore;

namespace AemenersolSync.Services;

public sealed record SyncResult(int PlatformsInserted, int PlatformsUpdated, int WellsInserted, int WellsUpdated);

public sealed class PlatformWellSyncService(SyncDbContext dbContext)
{
    public async Task<SyncResult> SyncAsync(IReadOnlyList<PlatformDto> source, CancellationToken cancellationToken)
    {
        var platformIds = source.Select(x => x.Id).Distinct().ToArray();
        var wellIds = source.SelectMany(x => x.Wells ?? []).Select(x => x.Id).Distinct().ToArray();
        var platforms = await dbContext.Platforms.Where(x => platformIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
        var wells = await dbContext.Wells.Where(x => wellIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
        var platformsInserted = 0; var platformsUpdated = 0; var wellsInserted = 0; var wellsUpdated = 0;
        foreach (var item in source)
        {
            if (!platforms.TryGetValue(item.Id, out var platform))
            {
                platform = new Platform { Id = item.Id };
                dbContext.Platforms.Add(platform); platforms.Add(item.Id, platform); platformsInserted++;
            }
            else platformsUpdated++;
            if (item.UniqueName is not null) platform.UniqueName = item.UniqueName;
            if (item.Latitude.HasValue) platform.Latitude = item.Latitude;
            if (item.Longitude.HasValue) platform.Longitude = item.Longitude;
            if (item.CreatedAt.HasValue) platform.CreatedAt = item.CreatedAt;
            if (item.UpdatedAt.HasValue) platform.UpdatedAt = item.UpdatedAt;

            foreach (var wellItem in item.Wells ?? [])
            {
                if (!wells.TryGetValue(wellItem.Id, out var well))
                {
                    well = new Well { Id = wellItem.Id };
                    dbContext.Wells.Add(well); wells.Add(wellItem.Id, well); wellsInserted++;
                }
                else wellsUpdated++;
                well.PlatformId = item.Id;
                well.Platform = platform;
                if (wellItem.UniqueName is not null) well.UniqueName = wellItem.UniqueName;
                if (wellItem.Latitude.HasValue) well.Latitude = wellItem.Latitude;
                if (wellItem.Longitude.HasValue) well.Longitude = wellItem.Longitude;
                if (wellItem.CreatedAt.HasValue) well.CreatedAt = wellItem.CreatedAt;
                if (wellItem.UpdatedAt.HasValue) well.UpdatedAt = wellItem.UpdatedAt;
            }
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return new(platformsInserted, platformsUpdated, wellsInserted, wellsUpdated);
    }
}

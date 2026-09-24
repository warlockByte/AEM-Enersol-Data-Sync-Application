using AemenersolSync.Models;
using Microsoft.EntityFrameworkCore;

namespace AemenersolSync.Data;

public sealed class SyncDbContext(DbContextOptions<SyncDbContext> options) : DbContext(options)
{
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<Well> Wells => Set<Well>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>(entity =>
        {
            entity.ToTable("Platform");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UniqueName).HasMaxLength(200);
        });
        modelBuilder.Entity<Well>(entity =>
        {
            entity.ToTable("Well");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.UniqueName).HasMaxLength(200);
            entity.HasOne(x => x.Platform).WithMany(x => x.Wells)
                .HasForeignKey(x => x.PlatformId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}

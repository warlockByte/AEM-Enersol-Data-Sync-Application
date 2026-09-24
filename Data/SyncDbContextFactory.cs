using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AemenersolSync.Data;

public sealed class SyncDbContextFactory : IDesignTimeDbContextFactory<SyncDbContext>
{
    public SyncDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("AEM_CONNECTION_STRING")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=AemenersolDb;Trusted_Connection=True;TrustServerCertificate=True;";
        var options = new DbContextOptionsBuilder<SyncDbContext>().UseSqlServer(connectionString).Options;
        return new SyncDbContext(options);
    }
}

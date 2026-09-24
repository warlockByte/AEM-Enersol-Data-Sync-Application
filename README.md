

# Aemenersol Platform/Well Sync

.NET 8 application that authenticates against the assessment API, downloads platform/well data, and upserts it into SQL Server LocalDB using Entity Framework Core Code First. The solution provides two ways to run the sync:

- **Console application** for a one-off or scheduled synchronization.
- **ASP.NET Core Web API** with Swagger for manually triggering synchronization and viewing stored data.

## Prerequisites

- .NET 8 SDK
- SQL Server Express LocalDB (`MSSQLLocalDB`)

## Choose how to run

### Option 1: Web API and Swagger

From the repository directory, start the API project:

PowerShell:

```powershell
$env:AEM_API_USERNAME = 'user@aemenersol.com'
$env:AEM_API_PASSWORD = 'Test@123'
dotnet run --project .\AemenersolSync.Api\AemenersolSync.Api.csproj --launch-profile http
```

The API applies the included EF migration during startup. Open [http://localhost:5097/swagger](http://localhost:5097/swagger) in a browser and use Swagger to:

1. Call `POST /api/sync?useDummy=false` to download and upsert data from the assessment API.
2. Call `POST /api/sync?useDummy=true` to test the dummy response with missing and additional JSON fields.
3. Call `GET /api/platforms` or `GET /api/wells` to verify the records stored in LocalDB.



The API remains running until stopped with `Ctrl+C`.




###

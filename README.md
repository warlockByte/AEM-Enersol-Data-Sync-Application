# Aemenersol Platform/Well Sync

.NET 8 console application that authenticates against the assessment API, downloads platform/well data, and upserts it into SQL Server LocalDB using Entity Framework Core Code First.

## Prerequisites

- .NET 8 SDK
- SQL Server Express LocalDB (`MSSQLLocalDB`)

## Run

PowerShell:

```powershell
$env:AEM_API_USERNAME = '<supplied username>'
$env:AEM_API_PASSWORD = '<supplied password>'
dotnet tool restore
dotnet restore
dotnet run
```

The app applies the included EF migration automatically. Run it again to exercise the update path. To verify tolerance of missing/changed fields with the supplied dummy endpoint:

```powershell
dotnet run -- --dummy
```

Optional overrides are `AEM_CONNECTION_STRING` and `AEM_API_BASE_URL`.

## Design notes

- API credentials are environment variables and are never committed.
- Database IDs use `ValueGeneratedNever`; they are the API IDs used for insert/update decisions.
- Platforms and wells are loaded in batches before applying changes, avoiding a query per record.
- DTO properties are nullable where an API field may be absent. Unknown JSON fields are ignored, and a missing `well` collection is treated as empty.
- The dummy response omits `createdAt`/`updatedAt` and adds `lastUpdate`; it therefore validates both missing-field and unknown-field behavior.
- Records absent from a response are retained because the requirement specifies upserts, not snapshot deletion.

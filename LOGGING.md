# Football Betting Application - Logging Setup

## 📝 Overview

The Football Betting application uses **Serilog** for structured logging with both console and file output. Logs are automatically created for all services when running through .NET Aspire.

## 📂 Log File Locations

Log files are written to each service's local `Logs/` directory:

```
C:\Dev\repos\Football Betting\
├── FootballBetting.Web\Logs\
│   ├── footballbetting-web-20251008.log          # Human-readable logs
│   └── footballbetting-web-json-20251008.log     # Structured JSON logs
├── FootballBetting.ApiService\Logs\
│   ├── footballbetting-apiservice-20251008.log
│   └── footballbetting-apiservice-json-20251008.log
└── FootballBetting.AppHost\Logs\
    ├── footballbetting-apphost-20251008.log
    └── footballbetting-apphost-json-20251008.log
```

## 📋 Log File Format

### Human-Readable Logs
```
2025-10-08 00:10:15.265 +01:00 [INF] [footballbetting-web] Now listening on: https://localhost:49997
2025-10-08 00:10:15.299 +01:00 [INF] [footballbetting-web] Application started. Press Ctrl+C to shut down.
```

### Structured JSON Logs
```json
{
  "@t": "2025-10-07T23:10:15.2656652Z",
  "@mt": "Now listening on: {address}",
  "address": "https://localhost:49997",
  "ServiceName": "footballbetting-web",
  "Environment": "Development"
}
```

## ⚙️ Configuration

### Log Levels

**Development Environment:**
- Default: `Debug`
- FootballBetting.*: `Debug`
- FootballAPIWrapper: `Debug`
- Microsoft.*: `Information/Warning`
- EntityFrameworkCore: `Information`

**Production Environment:**
- Default: `Information`
- Microsoft.*: `Warning`
- EntityFrameworkCore: `Warning`

### File Rotation
- **Daily rotation**: New log files created each day
- **Retention**: 7 days of log files kept
- **Format**: `servicename-YYYYMMDD.log`

## 🔍 Viewing Logs

### Real-time Console Logs
When running applications directly:
```bash
dotnet run --project FootballBetting.Web
```

### Aspire Dashboard
When running through Aspire:
1. Start: `dotnet run --project FootballBetting.AppHost`
2. Open: `https://localhost:17185`
3. Navigate to individual services for structured log viewing

### File-based Logs
```bash
# View latest Web logs
Get-Content "FootballBetting.Web\Logs\footballbetting-web-*.log" -Wait

# View latest API logs
Get-Content "FootballBetting.ApiService\Logs\footballbetting-apiservice-*.log" -Wait

# Parse JSON logs with PowerShell
Get-Content "FootballBetting.Web\Logs\footballbetting-web-json-*.log" | ConvertFrom-Json
```

## 🛠️ Customization

### Changing Log Levels
Update `appsettings.json` or `appsettings.Development.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "FootballBetting": "Debug",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  }
}
```

### Custom Log Paths
Modify `ServiceDefaults/Extensions.cs` in the `ConfigureSerilog` method to change log directory paths.

## 📊 Log Analysis

### Common Log Patterns
- **API Calls**: Look for `FootballAPIWrapper` namespace logs
- **Database Operations**: EF Core logs show SQL queries and performance
- **HTTP Requests**: ASP.NET Core request/response logs
- **Application Errors**: Exception logs with full stack traces

### Performance Monitoring
JSON logs can be imported into tools like:
- **Azure Monitor** / Application Insights
- **Elastic Stack** (ELK)
- **Seq** for structured log analysis
- **Splunk**

## 🚨 Troubleshooting

### No Log Files Created
1. Check permissions on the application directory
2. Verify Serilog packages are installed
3. Ensure `ConfigureSerilog()` is called in ServiceDefaults

### Log Files Too Large
1. Adjust retention policy in `ConfigureSerilog`
2. Increase log level filters in appsettings
3. Consider log shipping to external systems

### Missing Logs
1. Check service-specific log levels
2. Verify application startup (logs start when service initializes)
3. Check console output for Serilog initialization errors

---
*Last updated: October 2025*
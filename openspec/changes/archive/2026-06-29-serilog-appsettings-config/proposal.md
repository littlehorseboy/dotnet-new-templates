## Why

目前 `appsettings.json` 中的 `Logging:LogLevel` 設定對 Serilog 完全無效——Serilog 接管後不讀取 `Microsoft.Extensions.Logging` 的格式，log level 實際上寫死在 `Program.cs` 與 `SerilogHelper.cs` 的 `.MinimumLevel.Information()` 呼叫中。這讓模板使用者誤以為可以透過設定檔調整 log level，造成誤導，也使不同環境（開發 / 正式）的 log 粒度無法透過設定檔分開控制。

## What Changes

- 移除 `appsettings.json` 與 `appsettings.Development.json` 中無效的 `Logging:LogLevel` 區段
- 在兩個 appsettings 檔案中新增 Serilog 原生格式的 `Serilog:MinimumLevel` 區段（含 `Default` 與 `Override`）
- 加入 NuGet 套件 `Serilog.Settings.Configuration`
- `Program.cs` 的 system logger 加上 `.ReadFrom.Configuration(builder.Configuration)`，讓 appsettings 的設定真正生效
- `SerilogHelper.Initialize()`（bootstrap logger）的 `.MinimumLevel.Information()` 維持 hardcode，因為 DI 建立前無法讀取設定檔——此為已知且正確的設計

## Capabilities

### New Capabilities

無

### Modified Capabilities

- `vue-app-admin-dotnet8/logging`：新增「Serilog log level 可透過 appsettings.json 設定」的 requirement，並移除 `Logging:LogLevel` 殭屍設定的容許

## Impact

- **檔案異動**：`appsettings.json`、`appsettings.Development.json`、`Program.cs`、`VueAppAdmin.Server.csproj`
- **不影響**：`SerilogHelper.cs` bootstrap logger 行為不變
- **相依性**：新增 NuGet 套件 `Serilog.Settings.Configuration`
- **行為變化**：開發環境 log level 預設改為 `Debug`（原為 hardcode `Information`），正式環境維持 `Information`

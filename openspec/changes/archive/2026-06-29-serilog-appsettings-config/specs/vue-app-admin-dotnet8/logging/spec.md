## ADDED Requirements

### Requirement: Serilog MinimumLevel 可透過 appsettings.json 設定

系統 SHALL 使用 `Serilog.Settings.Configuration` 套件，讓 Serilog 的 MinimumLevel 從 `appsettings.json` 的 `Serilog:MinimumLevel` 區段讀取，取代原先 hardcode 的 `.MinimumLevel.Information()`。

#### Scenario: 正式環境 log level 讀取

- **WHEN** 應用程式在正式環境啟動
- **THEN** Serilog system logger SHALL 套用 `appsettings.json` 中 `Serilog:MinimumLevel:Default` 的設定值（預設為 `Information`）

#### Scenario: 開發環境 log level 讀取

- **WHEN** 應用程式在開發環境（`ASPNETCORE_ENVIRONMENT=Development`）啟動
- **THEN** Serilog system logger SHALL 套用 `appsettings.Development.json` 中 `Serilog:MinimumLevel:Default` 的設定值（預設為 `Debug`）

#### Scenario: Override 設定生效

- **WHEN** `appsettings.json` 的 `Serilog:MinimumLevel:Override` 包含 `Microsoft.AspNetCore: Warning`
- **THEN** 來自 `Microsoft.AspNetCore` namespace 的 log SHALL 僅記錄 `Warning` 以上等級，`Information` 以下丟棄

#### Scenario: bootstrap logger 不受 appsettings 控制

- **WHEN** 應用程式啟動，`IConfiguration` 尚未建立
- **THEN** `SerilogHelper.Initialize()` 的 bootstrap logger SHALL 固定使用 `Information` 等級，不讀取 appsettings.json

---

## REMOVED Requirements

### Requirement: Logging:LogLevel（Microsoft.Extensions.Logging 殭屍設定）

**Reason**：`appsettings.json` 原有的 `Logging:LogLevel` 屬於 Microsoft.Extensions.Logging 格式，Serilog 接管後不讀取此設定，對實際 log 行為無任何影響，應予移除以避免使用者誤解。

**Migration**：改用 `Serilog:MinimumLevel` 與 `Serilog:MinimumLevel:Override` 區段控制 Serilog log level。

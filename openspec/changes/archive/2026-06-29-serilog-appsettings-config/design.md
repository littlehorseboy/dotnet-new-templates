## Context

目前 `vue-app-admin-dotnet8` 使用 Serilog 作為 logging 框架，但 log level 以 `.MinimumLevel.Information()` hardcode 於 `Program.cs` 與 `SerilogHelper.cs`。`appsettings.json` 雖保留了 `Logging:LogLevel` 區段，但該格式屬於 `Microsoft.Extensions.Logging`，Serilog 接管後完全不讀取，形成殭屍設定。

加入 `Serilog.Settings.Configuration` 套件後，Serilog 可透過 `.ReadFrom.Configuration()` 讀取 appsettings 中 `Serilog:MinimumLevel` 的格式，使 log level 真正可在設定檔中控制。

## Goals / Non-Goals

**Goals:**
- Log level 可透過 `appsettings.json` / `appsettings.Development.json` 設定，不需重新編譯
- 移除無效的 `Logging:LogLevel` 殭屍設定，避免使用者誤解
- 開發環境預設 `Debug`，正式環境預設 `Information`
- 更新 README.md，說明設定方式

**Non-Goals:**
- 不改變 sink 設定（Console + File 維持不變）
- 不改變 `SerilogHelper.GetLogger<T>()` 分類 logger 的行為
- 不改變 `SerilogHelper.Initialize()`（bootstrap logger）——DI 建立前無法讀取設定檔，hardcode 為已知且正確的設計
- 不支援 Serilog 的 structured output（JSON sink）

## Decisions

### 決策一：使用 `Serilog:MinimumLevel` 格式，而非保留 `Logging:LogLevel`

**選擇**：改用 `Serilog:MinimumLevel` + `Serilog:MinimumLevel:Override`。

**理由**：`Logging:LogLevel` 是 Microsoft.Extensions.Logging 的格式，Serilog 原生不讀取；若強行橋接（`builder.Logging.SetMinimumLevel()`）會讓設定分散在兩個體系，反而更難維護。改用 Serilog 原生格式，設定與框架完全一致，無混用問題。

**捨棄方案**：維持 `Logging:LogLevel` 並手動橋接——額外代碼且易造成混淆。

---

### 決策二：bootstrap logger 維持 hardcode

**選擇**：`SerilogHelper.Initialize()` 的 `.MinimumLevel.Information()` 不改動。

**理由**：bootstrap logger 在 `WebApplication.CreateBuilder()` 之前執行，此時 `IConfiguration` 尚未建立，無法讀取 appsettings.json。這是 Serilog 兩階段初始化的已知限制，hardcode 為正確做法。

---

### 決策三：appsettings.Development.json 預設 `Debug`

**選擇**：開發環境 `Default` 改為 `Debug`。

**理由**：開發時需要看到更細的 log（如 Service 層的查詢細節），`Debug` 符合開發慣例；正式環境維持 `Information` 避免日誌量過大。

## Risks / Trade-offs

- **bootstrap logger 等級不一致**：應用啟動期間（DI 建立前）log level 固定為 `Information`，無法讀設定——此為 Serilog 兩階段初始化的固有限制，可接受，應在 README 中說明。
- **新增套件相依**：`Serilog.Settings.Configuration` 是 Serilog 官方套件，版本穩定，風險低。

## Open Questions

無。

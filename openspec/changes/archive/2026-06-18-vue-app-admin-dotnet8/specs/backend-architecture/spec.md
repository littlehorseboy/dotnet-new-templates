## ADDED Requirements

### Requirement: 服務以擴充方法統一註冊
`ServiceRegistrationExtensions.cs` SHALL 提供 `AddCustomServices(this IServiceCollection)` 擴充方法，集中註冊所有應用程式服務，`Program.cs` 只需呼叫 `builder.Services.AddCustomServices()`。

#### Scenario: 新增服務不需修改 Program.cs
- **WHEN** 開發者新增一個 Service 類別並在 `AddCustomServices` 內註冊
- **THEN** `Program.cs` 無需任何修改即可使用該服務

### Requirement: Serilog 透過 SerilogHelper 初始化
`SerilogHelper.cs` SHALL 提供 `Initialize()` 靜態方法，設定 Serilog 輸出至 Console 與 rolling daily log 檔案（純文字與 JSON 各一）。`Program.cs` SHALL 在 `WebApplication.CreateBuilder` 之前呼叫 `SerilogHelper.Initialize()`。

#### Scenario: 啟動期日誌被捕捉
- **WHEN** 應用程式在 builder 初始化前發生例外
- **THEN** 例外訊息被 Serilog 捕捉並寫入 log 檔案

#### Scenario: 日誌依日期 rolling
- **WHEN** 應用程式跨日運行
- **THEN** `logs/` 目錄下產生以日期命名的新 log 檔案

### Requirement: PasswordHasherHelper 提供密碼雜湊工具
`PasswordHasherHelper.cs` SHALL 提供 `HashPassword(plainText)` 與 `VerifyPassword(plainText, hash)` 靜態方法，使用 BCrypt 或 ASP.NET Core 內建 `PasswordHasher<T>`。

#### Scenario: 雜湊後可驗證
- **WHEN** `HashPassword("myPassword")` 產生 hash，再以 `VerifyPassword("myPassword", hash)` 驗證
- **THEN** 回傳 `true`

### Requirement: DTO 以 Request/Response 分層組織
`DTO/` 目錄 SHALL 按 `Request/` 與 `Response/` 兩個子目錄分層，每個功能模組各有子目錄。範本 SHALL 包含 `DTO/Request/Auth/LoginRequest.cs`、`DTO/Response/Auth/LoginResponse.cs` 作為示範。

#### Scenario: DTO 目錄結構清晰
- **WHEN** 開發者新增一個新模組的 DTO
- **THEN** 可依 `DTO/Request/<Module>/` 與 `DTO/Response/<Module>/` 的既有模式放置

### Requirement: ExampleItems 展示 Controller/Service 分層
`ExampleItemsController.cs` 與 `ExampleItemsService.cs` SHALL 作為示範用分層架構範例，使用 hardcoded dummy data（無 DB），提供 `GET /api/ExampleItems`（列表）與 `GET /api/ExampleItems/{id}`（單筆）。

#### Scenario: 取得 ExampleItems 列表
- **WHEN** 呼叫 `GET /api/ExampleItems`（已登入）
- **THEN** 回應 HTTP 200，body 為 ExampleItem 陣列（至少 3 筆 hardcoded 資料）

#### Scenario: 取得單筆 ExampleItem
- **WHEN** 呼叫 `GET /api/ExampleItems/1`（已登入）
- **THEN** 回應 HTTP 200，body 為對應的單筆 ExampleItem

#### Scenario: 不存在的 id
- **WHEN** 呼叫 `GET /api/ExampleItems/999`（已登入）
- **THEN** 回應 HTTP 404

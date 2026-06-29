## Why

目前後端 API 的 logging 僅涵蓋 HTTP 層的基本資訊（method、path、status、耗時），以及例外攔截與驗證失敗的局部記錄。當需要排查「Client 傳了什麼資料」或「Server 回了什麼結果」時，現有 log 無法提供足夠資訊。此外，各處 log 格式不一致，難以建立統一的 log 查閱習慣。

## What Changes

- 新增全域 `ApiLogFilter`（`Shared/Logging/ApiLogFilter.cs`），實作 `IActionFilter` + `IAlwaysRunResultFilter`，涵蓋三個案例：
  - **正常流程**：log request payload（已遮罩敏感欄位）+ response 內容 + elapsed time
  - **401 被擋**：log path、method、user（空）、狀態碼（Authorization 短路後仍觸發）
  - **400 驗證失敗**：log request payload（已遮罩）+ validation errors
- 新增 `[LogMask]` attribute（`Shared/Logging/LogMaskAttribute.cs`），標記 Request 物件中的敏感欄位（如 Password），filter 序列化前以 `"***"` 替換
- `LoginRequest.Password` 加上 `[LogMask]`
- 統一 log template：`"[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms"`
- Response 目前 log 完整 `ApiResponse` 內容（`Success`、`Message`、`Result`/`Results`）供觀察；`ApiPagedResponse` 額外記錄 `Total`
- `ApiResponse<T>` 加入 `ToLogSummary()` 方法，目前回傳完整內容；預留 TODO 供未來切換為「寫 log 存表」機制
- `Program.cs` 全域套用 `ApiLogFilter`，取代現有 validation log 邏輯（`InvalidModelStateResponseFactory` 中的 `LogWarning` 移至 filter 統一處理）

### TODO（未來規劃，本次不實作）

- **TODO: 登入紀錄存表** — `AuthService.ValidateCredentials` 完成後，將登入嘗試（帳號、結果、IP、時間）寫入資料庫 `LoginLogs` 表，供稽核查閱
- **TODO: API Log 存表** — `ApiLogFilter` 記錄完整 request/response payload 至資料庫 `ApiLogs` 表，支援依時間、user、path 查詢；屆時 `ToLogSummary()` 可改為精簡格式，完整內容改由資料庫保存

## Capabilities

### New Capabilities

- 無新增獨立 capability（本次為 `logging` 的需求延伸）

### Modified Capabilities

- `vue-app-admin-dotnet8/logging`：新增 API 全域 log filter 需求，包含統一格式、敏感欄位遮罩、request/response 記錄、三案例覆蓋規範

## Impact

- **新增檔案**：`Shared/Logging/LogMaskAttribute.cs`、`Shared/Logging/ApiLogFilter.cs`
- **修改檔案**：`Shared/ApiResponse.cs`、`Features/Auth/Requests/LoginRequest.cs`、`Program.cs`
- **無 breaking change**：filter 全域套用，現有 controller/service 不需修改
- **相依性**：Serilog 已安裝，無需新增套件
- **非目標（本次）**：登入紀錄存表、API Log 存表（均已標記 TODO）

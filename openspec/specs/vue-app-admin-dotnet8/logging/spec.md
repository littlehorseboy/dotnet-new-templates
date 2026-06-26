## ADDED Requirements

### Requirement: Serilog 基礎設定
應用程式 SHALL 使用 Serilog 作為 logging 框架，在 `Shared/Logging/SerilogHelper.cs` 中集中管理初始化設定。

#### Scenario: Console 與檔案 sink
- **WHEN** 應用程式啟動
- **THEN** Serilog SHALL 同時輸出至 Console 與每日 rolling txt 檔案（`logs/log-.txt`），保留天數 SHALL 為 365 天，不得使用 JSON sink

#### Scenario: 每日 rolling 檔名
- **WHEN** 跨越午夜
- **THEN** 系統 SHALL 自動建立新的 log 檔案，舊檔案保留，超過 365 個檔案時 SHALL 刪除最舊的

---

### Requirement: HTTP Request Logging
每一次 HTTP request/response SHALL 被自動記錄，包含 HTTP method、路徑、狀態碼、耗時。

#### Scenario: 正常請求記錄
- **WHEN** 任何 HTTP 請求進入並完成
- **THEN** Serilog SHALL 記錄一行 Information 等級的 log，包含 method、path、status code、duration（毫秒）

#### Scenario: `UseSerilogRequestLogging` 位置
- **WHEN** 設定 middleware pipeline
- **THEN** `app.UseSerilogRequestLogging()` SHALL 在 `UseAuthentication()` 之前加入 pipeline

---

### Requirement: Service 層結構化 Logging
Service 類別 SHALL 透過建構子注入 `ILogger<T>` 進行 logging，不得使用靜態 `Log.*` 方法。

#### Scenario: ILogger 注入
- **WHEN** 建立任何 Service 類別
- **THEN** SHALL 透過 primary constructor 注入 `ILogger<T>`（T 為該 Service 型別）

#### Scenario: 結構化 log 參數
- **WHEN** 記錄含有變數的 log
- **THEN** SHALL 使用具名佔位符（`{Username}`），不得使用字串插值（`$"{username}"`）

---

### Requirement: 驗證失敗 Logging
DataAnnotations 驗證失敗時，系統 SHALL 主動記錄 Warning 等級 log，包含路徑、HTTP 狀態碼與驗證錯誤詳情。

#### Scenario: 驗證失敗記錄格式
- **WHEN** request 的 ModelState 驗證失敗（400）
- **THEN** 系統 SHALL 記錄 `LogWarning`，包含 `{Path}`、`{StatusCode}`（值為 400）、`{@ValidationErrors}`（各欄位錯誤訊息的 Dictionary）

---

### Requirement: 全域 Exception 集中 Logging
所有未被捕捉的 exception SHALL 由 `ExceptionHandlingMiddleware` 攔截並記錄，位於 `Shared/Middleware/ExceptionHandlingMiddleware.cs`。

#### Scenario: Unhandled exception 記錄
- **WHEN** 任何未捕捉的 exception 發生
- **THEN** middleware SHALL 記錄 `LogError` 包含 exception 物件、HTTP method、路徑，並回傳 HTTP 500

#### Scenario: Exception detail 不洩漏
- **WHEN** 回傳 500 錯誤給前端
- **THEN** response body SHALL 只包含通用錯誤訊息，不得包含 exception message 或 stack trace

#### Scenario: Middleware 在 pipeline 的位置
- **WHEN** 設定 middleware pipeline
- **THEN** `ExceptionHandlingMiddleware` SHALL 是 pipeline 中最外層的 middleware（第一個 `app.Use*` 呼叫）

---

## ADDED Requirements

### Requirement: Serilog Log 保留天數可透過設定檔控制

系統 SHALL 從 `appsettings.json` 的 `Logging:RetentionDays` 讀取 Log 檔案保留天數，而非 hardcode。若設定檔未包含該值，SHALL fallback 至預設值 `365`，確保行為不變。

#### Scenario: 設定檔包含 RetentionDays

- **WHEN** `appsettings.json` 的 `Logging:RetentionDays` 設為 `30`
- **THEN** Serilog 每日 rolling log 檔案最多保留 30 天，超過自動刪除

#### Scenario: 設定檔未包含 RetentionDays

- **WHEN** `appsettings.json` 的 `Logging` 區段未包含 `RetentionDays`
- **THEN** Log 保留天數預設為 365 天，行為與修改前相同

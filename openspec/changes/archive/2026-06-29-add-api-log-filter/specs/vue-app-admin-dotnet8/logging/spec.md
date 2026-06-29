## ADDED Requirements

### Requirement: 全域 API Log Filter
系統 SHALL 透過全域 `ApiLogFilter`（`Shared/Logging/ApiLogFilter.cs`）統一記錄所有 API 端點的 request 與 response，涵蓋正常流程、401 授權短路、400 驗證失敗三個案例。

`ApiLogFilter` SHALL 實作 `IActionFilter` 與 `IAlwaysRunResultFilter`，並在 `Program.cs` 以 `options.Filters.Add()` 全域套用。

#### Scenario: 正常流程記錄
- **WHEN** 一個通過授權與驗證的 API 請求完成執行
- **THEN** `ApiLogFilter` SHALL 以 `LogInformation` 記錄一筆 log，包含 HTTP method、path、user identity、status code、masked request payload、response 內容、elapsed time（毫秒），格式為 `[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms`

#### Scenario: 401 授權短路記錄
- **WHEN** 請求因 `[Authorize]` 全域 filter 未通過授權而短路（HTTP 401）
- **THEN** `ApiLogFilter` SHALL 以 `LogWarning` 記錄一筆 log，包含 method、path、user（空值以 `-` 表示）、status code；request body 因 model binding 尚未執行，`req` 欄位記錄為 `null`

#### Scenario: 400 驗證失敗記錄
- **WHEN** request 的 DataAnnotations 驗證失敗（HTTP 400）
- **THEN** `ApiLogFilter` SHALL 以 `LogWarning` 記錄一筆 log，包含 method、path、user、status code、masked request payload；validation errors 以 `{@Response}` 欄位呈現

#### Scenario: Filter 執行時機
- **WHEN** 設定 MVC filter pipeline
- **THEN** `ApiLogFilter` SHALL 透過 `Program.cs` 的 `options.Filters.Add(new ApiLogFilter(...))` 全域套用，不需在各 Controller/Action 個別標記

---

### Requirement: 敏感欄位遮罩（LogMask）
系統 SHALL 提供 `[LogMask]` attribute（`Shared/Logging/LogMaskAttribute.cs`），用於標記 Request 物件中的敏感欄位。`ApiLogFilter` 序列化 request 之前，SHALL 以 reflection 將所有標記欄位的值替換為 `"***"`。

#### Scenario: Password 欄位遮罩
- **WHEN** `LoginRequest.Password` 標記有 `[LogMask]`，且 Login API 被呼叫
- **THEN** log 中的 `req` 欄位 SHALL 顯示 `"password":"***"`，不得出現原始密碼值

#### Scenario: 未標記欄位不受影響
- **WHEN** Request 物件的欄位未標記 `[LogMask]`
- **THEN** 該欄位值 SHALL 原樣序列化進入 log

---

### Requirement: ApiResponse Log 摘要
`ApiResponse<T>` SHALL 提供 `ToLogSummary()` 方法，供 `ApiLogFilter` 呼叫以取得 response 的 log 表示。

`ApiPagedResponse<T>` SHALL 覆寫 `ToLogSummary()`，回傳包含 `Success`、`Total`、`Count`（items 筆數）的精簡物件，避免分頁資料全量寫入 log。

#### Scenario: 一般 ApiResponse log 內容
- **WHEN** `ApiLogFilter` 記錄正常流程的 response
- **THEN** log 的 `res` 欄位 SHALL 包含完整 `ApiResponse` 序列化內容（`Success`、`Message`、`Result`/`Results`）

#### Scenario: ApiPagedResponse log 內容
- **WHEN** `ApiLogFilter` 記錄分頁查詢的 response
- **THEN** log 的 `res` 欄位 SHALL 只包含 `{ success, total, count }`，不含完整 items 清單

#### Scenario: 預留存表擴充點（TODO）
- **WHEN** 未來「API Log 存表」功能實作
- **THEN** `ToLogSummary()` SHALL 可改為精簡格式，完整 payload 由 `ApiLogs` 資料庫表保存；目前以 TODO 註解標記於 `ToLogSummary()` 實作處

---

### Requirement: 登入紀錄存表（TODO 預留）
系統 SHALL 預留登入紀錄寫入資料庫的擴充點，本次不實作。

#### Scenario: TODO 標記位置
- **WHEN** `AuthService.ValidateCredentials` 完成登入驗證
- **THEN** 程式碼 SHALL 包含 TODO 註解，說明未來應將登入嘗試（帳號、結果、IP、時間）寫入 `LoginLogs` 資料表，供稽核查閱

---

## MODIFIED Requirements

### Requirement: 驗證失敗 Logging
DataAnnotations 驗證失敗時，系統 SHALL 記錄 Warning 等級 log，包含路徑、HTTP 狀態碼與驗證錯誤詳情。

記錄行為 SHALL 由 `ApiLogFilter` 的 `IAlwaysRunResultFilter` 路徑統一處理，`InvalidModelStateResponseFactory` 中的 `LogWarning` 呼叫 SHALL 移除，以維持單一 log 來源。

#### Scenario: 驗證失敗記錄格式
- **WHEN** request 的 ModelState 驗證失敗（400）
- **THEN** `ApiLogFilter` SHALL 記錄 `LogWarning`，格式為 `[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms`，其中 `res` 包含 validation errors

#### Scenario: 不重複記錄
- **WHEN** 驗證失敗發生
- **THEN** 整個請求生命週期中 SHALL 只出現一筆驗證失敗 log，`InvalidModelStateResponseFactory` 不得再單獨呼叫 `LogWarning`

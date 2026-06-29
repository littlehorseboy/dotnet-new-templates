## 1. 基礎建設：LogMask attribute

- [x] 1.1 新增 `Shared/Logging/LogMaskAttribute.cs`（空的 `[AttributeUsage(AttributeTargets.Property)]` attribute class）
- [x] 1.2 在 `Features/Auth/Requests/LoginRequest.cs` 的 `Password` 欄位加上 `[LogMask]`

## 2. ApiResponse 擴充

- [x] 2.1 在 `Shared/ApiResponse.cs` 的 `ApiResponse<T>` 加入 `ToLogSummary()` 方法，目前回傳 `this`（完整內容）；加上 TODO 註解說明未來改為精簡格式並搭配 `ApiLogs` 存表
- [x] 2.2 在 `Shared/ApiResponse.cs` 的 `ApiPagedResponse<T>` 加入 `ToLogSummary()` 方法，回傳 `{ success, total, count }` 精簡物件

## 3. ApiLogFilter 實作

- [x] 3.1 新增 `Shared/Logging/ApiLogFilter.cs`，宣告 class 實作 `IActionFilter`、`IAlwaysRunResultFilter`，注入 `ILogger<ApiLogFilter>`
- [x] 3.2 實作 private helper `MaskRequest(object? requestObj)`：用 reflection 找出所有標記 `[LogMask]` 的 property，複製物件後將其值替換為 `"***"`，回傳匿名物件或 Dictionary 供序列化
- [x] 3.3 實作 `OnActionExecuting`：取出第一個 action argument（request payload），呼叫 `MaskRequest`，存入 `HttpContext.Items["ApiLog.Request"]`；記錄起始時間至 `HttpContext.Items["ApiLog.StartTime"]`
- [x] 3.4 實作 `OnResultExecuted`（IAlwaysRunResultFilter）：從 `HttpContext.Items` 取出 request、start time；計算 elapsed；判斷 status code 決定 log level（401/403 → Warning，400 → Warning，其餘 → Information / Error）；取得 response（若 `context.Result` 為 `ObjectResult` 且 Value 為 `ApiResponse` 則呼叫 `ToLogSummary()`，否則記錄 status code）；寫入統一 log template `"[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms"`

## 4. Program.cs 整合

- [x] 4.1 在 `Program.cs` 的 `AddControllers(options => ...)` 中加入 `options.Filters.Add<ApiLogFilter>()`（或透過 DI 注入方式加入）
- [x] 4.2 移除 `InvalidModelStateResponseFactory` 中原有的 `logger.LogWarning("Validation failed ...")` 呼叫（保留 response 格式邏輯，只刪 log 那行）

## 5. AuthService TODO 標記

- [x] 5.1 在 `Features/Auth/AuthService.cs` 的 `ValidateCredentials` 方法內，於驗證完成後加上 TODO 註解：說明未來應將登入嘗試（帳號、結果、IP、時間）寫入 `LoginLogs` 資料表

## 6. 驗證

- [x] 6.1 啟動應用程式，呼叫正常 API（需帶 JWT token），確認 log 輸出符合 `[API] POST /api/... | user:admin | 200 | req:{...} | res:{...} | Xms` 格式
- [x] 6.2 呼叫任一 API 不帶 token，確認 log 出現 401 Warning，且 `req` 為 `null`
- [x] 6.3 呼叫 Login API 帶空白 username，確認 log 出現 400 Warning，`password` 欄位顯示 `"***"`
- [x] 6.4 呼叫 Login API 帶正確帳密，確認 log 中 `password` 顯示 `"***"`，`username` 原樣顯示
- [x] 6.5 呼叫分頁查詢 API（ExampleItems/Search），確認 `res` 只包含 `{ success, total, count }`，不含完整 items 清單

## 1. Serilog 端：注入 correlation id

- [x] 1.1 修改 `VueAppAdmin.Server/Shared/Logging/ApiLogFilter.cs`：於 `OnResultExecuted` 讀取 `httpContext.TraceIdentifier`，加入現有的 log message template（新增 `reqId:{RequestId}` 欄位），同時套用至 400/401/403 的 Warning 分支與其餘的 Information 分支
- [x] 1.2 修改 `VueAppAdmin.Server/Shared/Middleware/ExceptionHandlingMiddleware.cs`：於 catch 區塊讀取 `context.TraceIdentifier`，加入 `logger.LogError` 的訊息樣板，欄位名稱與 1.1 保持一致
- [x] 1.3 本機啟動專案，分別觸發一次正常請求與一次會拋出未攔截例外的請求，確認 `logs/log-.txt` 中兩種情境的 log 行都包含 correlation id 欄位
- [x] 1.4 在 `VueAppAdmin.Server/Shared/Logging/ApiLogFilter.cs` 加上程式碼註解（放在未來會設定 `ModulesNames` 的位置，例如 `OnActionExecuting` 附近）：註明日後串接 `Basic_Api_Log.ModulesNames` 時，該值須透過 `ActionDescriptor`（`ControllerActionDescriptor.ControllerTypeInfo` + `ActionName`）自動取得，格式為 `Namespace.Class/Method`，禁止在各 Controller/Action 手動硬編碼字串

## 2. Schema 異動：Basic_Api_Log

- [x] 2.1 於 `db/schema.sql` 的 `Basic_Api_Log` 定義中新增欄位：`RequestId varchar(64) NULL`、`Method varchar(10) NULL`、`Path nvarchar(200) NULL`、`StatusCode int NULL`、`ElapsedMs int NULL`
- [x] 2.2 於 `db/schema.sql` 中移除 `Basic_Api_Log.GroupGuid` 欄位定義
- [x] 2.3 新增 CHECK constraint：`ALTER TABLE Basic_Api_Log ADD CONSTRAINT CK_Basic_Api_Log_Datas_IsJson CHECK (Datas IS NULL OR ISJSON(Datas) = 1)`
- [x] 2.4 以 `sys.sp_addextendedproperty` 補齊 `RequestId`、`Method`、`Path`、`StatusCode`、`ElapsedMs` 五個新欄位的繁體中文 MS_Description（內容依 design.md 的欄位說明草稿表）
- [x] 2.5 更新 `ModulesNames` 現有的 MS_Description，補上資料格式說明（例如「功能名稱（格式：Namespace.Class/Method）」），只描述資料語意，不寫入 `ActionDescriptor` 這類實作細節（該細節放在 1.4 的程式碼註解）
- [x] 2.6 移除 `Basic_Api_Log.GroupGuid` 對應的既有 `sp_addextendedproperty` 說明陳述式

## 3. Schema 異動：Basic_Api_Change_Log

- [x] 3.1 於 `db/schema.sql` 的 `Basic_Api_Change_Log` 定義中新增欄位：`RequestId varchar(64) NULL`
- [x] 3.2 以 `sys.sp_addextendedproperty` 補齊 `RequestId` 欄位的繁體中文 MS_Description（內容依 design.md 的欄位說明草稿表）

## 4. 驗證

- [x] 4.1 以人工檢視方式確認 `db/schema.sql` 語法正確（欄位定義、`CHECK` constraint 語法、`GO` 分隔皆與既有慣例一致），不實際對資料庫執行建表
- [x] 4.2 語法審閱確認 `ISJSON(Datas) = 1` 為 SQL Server 2016+ 原生函式，合法 JSON 通過、非 JSON 字串會被此 constraint 拒絕；未實際執行 INSERT 測試
- [x] 4.3 以 `git diff db/schema.sql` 確認差異僅落在 `Basic_Api_Log`／`Basic_Api_Change_Log` 兩個區塊，`Basic_LoginLog`、`Basic_OperationLog` 未被觸碰
- [x] 4.4 檢查 `openspec validate api-log-traceability --strict` 通過（已知限制：此 CLI 版本對兩層巢狀 capability 路徑 `specs/vue-app-admin-dotnet8/logging/` 回報 "No deltas found"，與既有已完成的 `add-db-schema-and-template-ux` change 相同限制，非本次新增問題）

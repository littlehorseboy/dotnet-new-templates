## Context

現況組成：

- `Program.cs` 以 Serilog 建立 `systemLogger`（輸出至 console 與 `logs/log-.txt`）。
- `Shared/Logging/ApiLogFilter.cs`：`IActionFilter` + `IAlwaysRunResultFilter`，於 `OnResultExecuted` 寫出一行結構化 log（`Method/Path/User/StatusCode/req/res/ElapsedMs`）。此攔截點只在 MVC action pipeline **正常完成**（含 200/400/401/403）時才會執行。
- `Shared/Middleware/ExceptionHandlingMiddleware.cs`：掛載於 `app.UseMiddleware<ExceptionHandlingMiddleware>()`，位於 MVC routing/pipeline 之外層。當 action 執行中拋出未攔截例外時，例外會直接傳播到此 middleware 被捕捉，**不會**經過 `ApiLogFilter.OnResultExecuted`，因此不會產生 `ApiLogFilter` 那行 log。
- `db/schema.sql` 內既有 `Basic_Api_Log`、`Basic_Api_Change_Log` 兩張表，目前範本產生的專案程式碼（`*.cs`）完全沒有寫入或讀取這兩張表（已用 `grep -rn` 確認）。
- `Basic_Api_Log` 現有欄位：`id, Actions(nvarchar 50), ModulesNames(nvarchar 200), Datas(nvarchar max), GroupGuid(uniqueidentifier), CreateUserRowId(uniqueidentifier), CreateDate(datetime)`。`GroupGuid` 型別為 `uniqueidentifier`，但範本 `Basic_Groups` 的主鍵 `GroupId` 為 `int IDENTITY`，兩者型別不符，且 `Basic_Api_Log` 未對 `GroupGuid` 設定任何 FK constraint（相對地，`Basic_OperationLog.CreatedUserGuid` 有明確 `FOREIGN KEY REFERENCES Basic_Users`）。可判定 `GroupGuid` 為範本沿用過程留下、與目前結構無關聯的孤兒欄位。
- 範本產生的專案一開始就是空白骨架（例如 `UserRepository` 的 DB 查詢方法多為 TODO），尚未有任何真實 SQL 執行於生產路徑上，是在缺陷被實際踩到之前先把 log 可追溯性基礎補齊的合適時機。

## Goals / Non-Goals

**Goals:**

- 讓同一個 HTTP request 不論正常結束或未攔截例外結束，都能以單一 correlation id 互相對應，並讓此 id 同時出現在 Serilog 文字 log 與資料庫 log 表中。
- 讓 `Basic_Api_Log` 具備可直接以 SQL 查詢的關鍵欄位（`Method`、`Path`、`StatusCode`、`ElapsedMs`），不必解析 `Datas` 內的 JSON blob。
- 讓 `Basic_Api_Change_Log`（資料異動前後對照）能串回是哪一次 API 呼叫造成的異動。
- 確保 `Datas` 欄位只會存放合法 JSON，避免未來寫入邏輯不一致造成的格式髒污。
- 移除已確認無關聯、命名具誤導性的 `GroupGuid` 欄位。

**Non-Goals:**

- 不設計新的 id 產生機制（不自行生成 GUID／序號，不引入新套件）。
- 不處理單一 SQL 查詢層級的耗時追蹤（例如 Dapper 呼叫的 Stopwatch 包裝，或 OpenTelemetry `SqlClient` instrumentation）。`Basic_Api_Log.ElapsedMs` 只反映「整支 API 呼叫」的總耗時，與 `ApiLogFilter` 現有計算邏輯同一顆粒度。
- 不導入 OpenTelemetry 或任何分散式追蹤系統；本次設計僅確保未來若導入 OTel，`HttpContext.TraceIdentifier` 可平滑升級為標準 W3C trace id，不需要重新設計 correlation 機制。
- 不異動 `Basic_LoginLog`、`Basic_OperationLog` 兩張表。
- 不實作 `Basic_Api_Log` / `Basic_Api_Change_Log` 實際寫入的 Repository/Service 程式碼；本次僅完成 schema 異動與 Serilog 端 correlation id 注入，資料庫寫入邏輯待後續由使用此範本的專案依當下需求另行規劃。

## Decisions

### 1. Correlation id 採用 `HttpContext.TraceIdentifier`，不自建機制

**決策**：直接讀取 ASP.NET Core 內建的 `HttpContext.TraceIdentifier`，作為貫穿 Serilog log 與 `Basic_Api_Log.RequestId` / `Basic_Api_Change_Log.RequestId` 的唯一 correlation id。

**考慮過的替代方案**：
- 自行生成 GUID 並存入 `HttpContext.Items`：需要額外程式碼在最外層 middleware 產生並傳遞，且與框架已提供的機制重複。
- 導入 OpenTelemetry 取得標準 W3C trace id：屬於更大的架構決策（見 Non-Goals），且範本目前無明確痛點需要分散式追蹤能力。

**為何選擇 `TraceIdentifier`**：框架自動生成、每個 request 唯一、零依賴、零額外程式碼即可取得；且此屬性在有 `Activity`（例如未來接上 OTel）存在時會自動升級為 W3C trace id 格式，具備前向相容性。

### 2. 例外路徑與正常路徑共用同一 correlation id 欄位

**決策**：`ApiLogFilter.OnResultExecuted`（正常路徑）與 `ExceptionHandlingMiddleware` 的 catch 區塊（例外路徑）各自獨立讀取 `HttpContext.TraceIdentifier`，並以相同的 log 欄位名稱（`RequestId`）輸出。由於兩者讀的是同一個 `HttpContext` 上的同一個屬性，不需要額外的傳遞機制即可保證一致。

**為何不重構例外攔截架構**：另一個可能方案是把 `ExceptionHandlingMiddleware` 改為 MVC 層的 `IExceptionFilter`，讓例外也走完 MVC pipeline、觸發 `IAlwaysRunResultFilter`。此方案能讓「只寫一行 log」的目標更徹底達成，但屬於現有例外處理架構的重構，風險與影響範圍較大（例如例外攔截的位置、對非 MVC 請求的涵蓋範圍都會改變）。本次選擇風險較低的做法：兩條路徑各自寫各自的 log，但透過共用 correlation id 讓查詢時能夠對應，不變動既有的攔截架構。

### 3. `Basic_Api_Log` 新增 `RequestId / Method / Path / StatusCode / ElapsedMs`，移除 `GroupGuid`

**決策**：
- `RequestId varchar(64) NULL`：對應 `HttpContext.TraceIdentifier`（型別長度以框架實際格式為準，預留足夠長度）。
- `Method varchar(10) NULL`：HTTP method。
- `Path nvarchar(200) NULL`：HTTP 請求路徑。
- `StatusCode int NULL`：僅在 `Actions = RESPONSE` 或 `EXCEPTION` 的列有值。
- `ElapsedMs int NULL`：整支 API 呼叫總耗時，僅在 `Actions = RESPONSE` 或 `EXCEPTION` 的列有值，與 `ApiLogFilter` 現有的 `elapsedMs` 計算邏輯一致（同一顆粒度，非 SQL 層級耗時）。
- 移除 `GroupGuid`：型別（`uniqueidentifier`）與範本 `Basic_Groups.GroupId`（`int`）不符，無 FK constraint，程式碼中無任何引用，確認為孤兒欄位。

**為何拉出獨立欄位而非全部塞進 `Datas`**：`Datas` 現況是一包未結構化的 JSON，若要查詢「哪些 request 的狀態碼是 500」或「哪些 request 耗時超過 1 秒」，必須在 SQL 中解析 JSON（`OPENJSON` 等），效率與可讀性都差。將高頻查詢用的欄位獨立出來，符合這幾張表原始設計「可被稽核/報表工具直接查詢」的定位。

### 4. `ModulesNames` 與 `Path` 並存，各自代表不同視角

**決策**：保留現有 `ModulesNames` 欄位，沿用既有格式 `Namespace.Class/Method`，與新增的 `Path` 欄位並存，不互相取代。

- `ModulesNames`（code 視角）：代表實際處理該請求的類別與方法，不隨路由/API 版本調整而改變，適合在原始碼中全文搜尋比對。
- `Path`（HTTP 視角）：代表使用者實際呼叫的網址，適合與前端呼叫紀錄、Swagger 文件、API Gateway 存取紀錄對照。

**技術實作提示（供未來實作階段參考，本次不展開實作）**：`ModulesNames` 的值可透過 ASP.NET Core 的 `ActionDescriptor`（例如 `ControllerActionDescriptor` 的 `ControllerTypeInfo` 與 `ActionName`）在 `ApiLogFilter` 攔截點自動取得，不需要在各 Controller/Action 手動填寫此欄位。實作時務必記得使用此機制，避免退化為每個 Action 手動硬編碼字串。

### 5. `Datas` 欄位加上 `ISJSON` CHECK constraint

**決策**：
```sql
ALTER TABLE Basic_Api_Log
ADD CONSTRAINT CK_Basic_Api_Log_Datas_IsJson
CHECK (Datas IS NULL OR ISJSON(Datas) = 1);
```

**為何用 CHECK constraint 而非只靠程式規範**：`ISJSON()` 為 SQL Server 2016 以後原生支援的函式，不需要額外套件或觸發器。以資料庫層級強制格式，可避免任何未來寫入路徑（無論是範本產生的專案程式碼、手動維運腳本、或其他工具）意外寫入非 JSON 字串，比僅靠程式碼慣例更可靠。

### 6. `Basic_Api_Change_Log` 新增 `RequestId`

**決策**：新增 `RequestId varchar(64) NULL`，與 `Basic_Api_Log.RequestId` 使用同一個 `HttpContext.TraceIdentifier` 值，使資料異動紀錄（`BeforeDatas`/`AfterDatas`/`SQLStr`）可以串回是哪一次 API 呼叫造成的。既有欄位（`Actions`、`ModulesNames`、`BeforeDatas`、`AfterDatas`、`SQLStr`、`CreateUserRowId`、`CreateDate`）不變動。

### 7. 欄位中文說明（MS_Description）比照既有慣例補齊

**決策**：所有新增欄位（`RequestId`、`Method`、`Path`、`StatusCode`、`ElapsedMs`）皆以 `sys.sp_addextendedproperty` 補上繁體中文欄位說明，格式比照 `db/schema.sql` 中既有欄位的寫法。

| 欄位 | MS_Description 草稿 |
|---|---|
| `Basic_Api_Log.RequestId` | 請求關聯序號，對應 HttpContext.TraceIdentifier，用於串接同一次呼叫的 REQUEST/RESPONSE/EXCEPTION 紀錄 |
| `Basic_Api_Log.Method` | HTTP 方法（GET/POST/PUT/DELETE 等） |
| `Basic_Api_Log.Path` | HTTP 請求路徑 |
| `Basic_Api_Log.StatusCode` | HTTP 回應狀態碼，僅 RESPONSE/EXCEPTION 列有值 |
| `Basic_Api_Log.ElapsedMs` | 整支 API 呼叫耗時（毫秒），僅 RESPONSE/EXCEPTION 列有值 |
| `Basic_Api_Change_Log.RequestId` | 請求關聯序號，對應造成此筆資料異動的 API 呼叫（Basic_Api_Log.RequestId） |

## Risks / Trade-offs

- **[風險] 兩條 log 路徑（`ApiLogFilter` / `ExceptionHandlingMiddleware`）仍是各自獨立寫 log，架構上未真正合併** → **緩解**：透過共用 correlation id 達到「可對應查詢」的實務效果，且改動風險遠低於重構例外攔截架構；若未來確有需求，可再評估改用 `IExceptionFilter` 的方案。
- **[風險] `ElapsedMs` 只到「整支 API」顆粒度，可能被誤解為涵蓋 SQL 執行時間** → **緩解**：於本文件與欄位說明中明確標註顆粒度範圍，並列為 Non-Goal，避免後續維護者誤用此欄位做 SQL 效能分析。
- **[風險] 新增 `CHECK (ISJSON(Datas)=1)` 屬於 BREAKING 變更** → **緩解**：目前無任何程式碼寫入 `Basic_Api_Log`，此 constraint 於範本層級上線時不會影響現有資料或現有寫入路徑；若未來寫入邏輯不慎序列化錯誤，會在 INSERT 當下直接失敗並曝露問題，屬於預期內的防呆行為。使用此範本產生的既有專案若已有資料，套用前需自行確認 `Datas` 現有資料是否皆為合法 JSON。
- **[取捨] 移除 `GroupGuid` 而非保留觀察** → 已確認型別不符、無 FK、無程式碼引用，保留只會持續造成誤導，故直接移除；若未來真的需要「群組」維度做分析，屆時可用正確型別（`int`，對應 `Basic_Groups.GroupId`）重新設計，不沿用舊欄位語意。

## Migration Plan

1. 於 `db/schema.sql` 新增對應的 `ALTER TABLE` 陳述式（新增欄位、移除 `GroupGuid`、新增 CHECK constraint、補齊 `sp_addextendedproperty`），比照現有檔案風格與順序。
2. 修改 `ApiLogFilter.cs` 與 `ExceptionHandlingMiddleware.cs`，於既有 log 樣板加入 `HttpContext.TraceIdentifier` 欄位。
3. 因目前範本無任何程式碼實際寫入 `Basic_Api_Log` / `Basic_Api_Change_Log`，本次 schema 異動不影響現有資料，無需資料遷移或回填腳本。
4. 若使用此範本產生的專案已進入正式環境且這兩張表已有資料，實際套用前需另行確認 `Datas` 現有資料是否皆為合法 JSON，避免 CHECK constraint 建立失敗；本設計假設範本層級套用時為全新/無資料狀態。

## Open Questions

- `Basic_Api_Log` / `Basic_Api_Change_Log` 實際寫入邏輯（何時、何處呼叫 INSERT）不在本次範圍內，需在使用此範本的專案中依當時的 Repository/Service 慣例決定（例如是否透過 `ApiLogFilter` 直接寫 DB，或改用非同步佇列避免拖慢主要請求）。
- `RequestId varchar(64)` 的長度是否足夠，需在實作時依 `HttpContext.TraceIdentifier` 實際輸出格式（一般路徑或已啟用 `Activity` 時的 W3C 格式）確認，必要時調整長度。

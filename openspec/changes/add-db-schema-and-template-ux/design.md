## Context

範本後端採 Dapper + SQL Server(見 `openspec/specs/vue-app-admin-dotnet8/data-access/spec.md`),但範本內沒有任何 `.sql` 檔案。`Features/Auth/UserRepository.cs` 是 TODO 骨架(註解內有示意 SQL 但沒有對應資料表);`Menu` 與 `FeatureList` 只有 Service(in-memory),沒有 Repository;群組權限資料放在 `Features/Auth/GroupFeatureStore.cs`(in-memory)。

`template.json` 已有 5 組 `generated port` symbols 與 sources 排除清單,但沒有 `postActions`;`appsettings.json` 的 `Jwt:SignKey` 是固定佔位字串 `REPLACE_WITH_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS`,所有產出專案共用。

使用者已提供其慣用的 `Basic_*` / `Para_*` schema(來自實際專案),本次需通用化後納入範本。

## Goals / Non-Goals

**Goals:**

- 範本使用者產生專案後,能依 `db/schema.sql` + `db/seed.sql` 一次建好資料庫與初始資料
- schema 完全去除領域專屬痕跡,符合「範本不得洩漏公司/客戶資訊」原則
- 每個產出專案的 JWT secret 自動唯一化
- 產生專案後顯示明確的下一步指引

**Non-Goals:**

- 不引入 migration 工具(DbUp、FluentMigrator、EF Migrations)——單檔 schema.sql 已滿足範本需求
- 不改變任何 runtime 行為:Auth/Menu/FeatureList 維持 in-memory dummy,不新增群組管理等後台頁面
- 不在本次決定 `IdNumber` 欄位去留(標記 TODO)
- 不處理範本 repo 本身(開發用)的 JWT secret 隨機化——僅產出的專案

## Decisions

### D1. schema 以單檔 `db/schema.sql` 交付,`GO` 批次分隔

依使用者貼附的 SSMS 匯出風格:`CREATE TABLE` + `ALTER TABLE ... ADD CONSTRAINT`(default/FK)+ `sp_addextendedproperty`(MS_Description),每批 `GO` 分隔。考慮過 per-table 分檔,但單檔對「新專案一次建庫」的場景更直接,也符合使用者現有工作流(SSMS 直接執行)。

資料表共 11 張:`Basic_Users`、`Basic_Groups`、`Basic_Users_Groups`、`Basic_Modules`、`Basic_Group_Modules`、`Basic_LoginLog`、`Basic_OperationLog`、`Basic_Api_Log`、`Basic_Api_Change_Log`、`Para_Categories`、`Para_Info`。保留使用者慣例:稽核四欄位、`MS_Description` 全欄位覆蓋、狀態 int + 列舉寫在描述、log 表 `PRIMARY KEY CLUSTERED (id DESC)`。

### D2. 通用化調整清單(相對使用者原始 schema)

| 項目 | 處理 | 理由 |
|------|------|------|
| `EOfficeAccount`、`EGovernmentAccount`、`UnitCode` | 刪除 | 政府專案領域痕跡 |
| `Department` | 刪除 | 原註解已標示棄用 |
| `Basic_LoginLog.LoginMethod` 列舉註解 | 改通用文字(如「1:SSO、2:一般帳號,依專案自行擴充」) | 移除「公務入口/自然人憑證」等字樣 |
| `IdNumber` | 保留 + `-- TODO:` 註記(敏感個資,通用範本是否保留待議) | 使用者決定暫不處理 |
| `NormalPassword varchar(30)` | 改 `PasswordHash varchar(72)` | BCrypt 雜湊固定 60 字元,對齊 `UserRepository.cs` 既有 BCrypt 註解 |
| `FK_Basic_LoginLog_Basic_LoginLog`(Id→Id 自我參照) | 移除 | 原 schema 誤建 |
| `Basic_Users.Title` default `getdate()` | 移除 default | 職稱預設日期為誤植 |
| `Basic_Users_Groups` 無 PK | 補 `(GroupId, UserGuid)` 複合主鍵 | 防重複關聯 |

### D3. `db/seed.sql`:一組 admin + 範例選單/群組

- `Basic_Users`:一筆 admin,`PasswordHash` 為預設密碼(文件化,如 `Admin@123`)的 BCrypt 雜湊;`CreatedUserGuid` 自我參照(系統初始帳號慣例)
- `Basic_Modules`:對齊前端現有路由的選單樹(首頁、Example Items 等)
- `Basic_Groups` + `Basic_Group_Modules` + `Basic_Users_Groups`:一個 Administrators 群組,admin 加入,擁有全部功能的五種權限旗標
- `Para_Categories` / `Para_Info`:塞入 schema 註解中引用的列舉(如狀態、裝置)作為示範

### D4. JWT secret =「專案名稱 + 隨機 GUID」的實作方式

`appsettings.json` 的 SignKey 改為 `"VueAppAdmin-REPLACE_WITH_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS"`:

- 前段 `VueAppAdmin` 由既有 `sourceName` 機制自動換成專案名稱
- 後段由新 symbol `JwtSecret`(`"type": "generated", "generator": "guid"`,format `N`)以 `replaces: "REPLACE_WITH_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS"` 置換為 32 字元隨機 hex

產出結果形如 `MyApp-9f8a3c...`(> 32 字元,滿足 HMAC 金鑰長度)。範本 repo 本身的值仍為固定字串且長度足夠,開發測試行為不變。考慮過只用 GUID 不加專案名稱前綴,但使用者偏好含專案名稱,且前綴可讀性有助於辨識設定來源。

`appsettings.Development.json` 原本有獨立的 dev 用 SignKey(不同佔位字串),已同步改為與 `appsettings.json` 相同的佔位字串,使同一個 `JwtSecret` symbol 能一併置換兩個檔案,避免 Development 環境仍殘留舊的固定金鑰。

### D5. postActions 只用「顯示指引」action,不執行指令

`template.json` 增加 `postActions`,採用 instructions 顯示類 action(`actionId: "AC1156F7-BB77-4DB8-B28F-24EEBCCA1E5C"`,實作時以官方文件確認 id),`manualInstructions` 內容涵蓋:

1. 建立 SQL Server 資料庫並執行 `db/schema.sql`、`db/seed.sql`
2. 確認 `appsettings.json` 的 `ConnectionStrings:Default`
3. 前端 `cd <name>.client && npm install`
4. 預設 admin 帳密與「請立即更改」提醒

不採自動執行(run script / restore)——使用者明確要求僅提示;自動執行在無 SQL Server 環境會直接失敗,提示則永遠安全。

### D6. Dapper SQL 註解放在既有檔案,不新增 Repository 骨架

- `Features/Auth/UserRepository.cs`:既有 TODO 註解升級為對應 `Basic_Users` 的完整 SQL(登入驗證含 `Status = 1`、鎖定判斷欄位,查 DisplayName 對應 `UserName`)
- `Features/Auth/GroupFeatureStore.cs`:註解附上 `Basic_Users_Groups` JOIN `Basic_Group_Modules` JOIN `Basic_Modules` 的權限查詢 SQL
- `Features/Menu/MenuService.cs`:註解附上 `Basic_Modules` 樹狀選單查詢 SQL(含 `FatherModuleId` 組樹說明)

考慮過為 Menu/FeatureList 新增 Repository 介面骨架,但那會產生無人呼叫的死程式碼;等真正接 DB 時再依 data-access spec 建立即可,註解中註明此慣例。

### D7. `db/` 資料夾納入範本輸出

現有 `sources` 排除清單不影響 `db/**`,無需修改;但 tasks 需包含「產生專案後驗證 `db/` 存在」的檢查步驟。

## Risks / Trade-offs

- [postActions 的 actionId 記錯或 host 不支援] → 實作時以 Microsoft 官方 template.json 文件確認;`dotnet new` CLI 與 Visual Studio 對 instructions 顯示行為不同,驗收以 CLI 為準
- [guid generator 參數格式寫錯導致置換失敗] → 驗收步驟必含:實際 `dotnet new` 產生專案,檢查 `appsettings.json` 的 SignKey 已變為「專案名 + 32 hex」且兩次產生值不同
- [seed 的 admin 預設密碼是公開已知值] → postActions 與 README 明示「上線前必須更改」;此為所有範本 seed 的共同取捨
- [schema 與未來真正接 DB 的程式碼可能漂移] → SQL 註解直接寫在程式碼旁,與 schema.sql 同 change 交付、同 change 維護;data-access spec 增加對應需求以約束後續變更
- [`IdNumber` 留在通用範本有個資疑慮] → 已標 TODO 並記錄於 proposal,由後續 change 決定

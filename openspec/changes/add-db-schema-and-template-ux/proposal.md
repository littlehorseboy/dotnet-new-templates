## Why

`vue-app-admin-dotnet8` 範本使用 Dapper + SQL Server,但範本內完全沒有任何資料庫 schema 檔案——使用者以 `dotnet new` 產生新專案後,不知道資料庫該長什麼樣子,`UserRepository` 等 TODO 骨架也沒有對應的資料表可參照。同時,範本產生後仍需手動處理 JWT secret(目前為固定值,所有產出專案共用同一把金鑰是安全隱患),且產生完成後沒有任何「下一步該做什麼」的指引。

## What Changes

- 新增 `db/` 資料夾,內含 `schema.sql` 與 `seed.sql`:
  - `schema.sql` 以既有 `Basic_*` / `Para_*` 命名慣例建立 11 張系統基礎資料表(`Basic_Users`、`Basic_Groups`、`Basic_Users_Groups`、`Basic_Modules`、`Basic_Group_Modules`、`Basic_LoginLog`、`Basic_OperationLog`、`Basic_Api_Log`、`Basic_Api_Change_Log`、`Para_Categories`、`Para_Info`),每張表與每個欄位掛 `MS_Description` 擴充屬性,稽核欄位採 `CreatedUserGuid` / `CreatedTime` / `ModifiedUserGuid` / `ModifiedTime` 四欄位慣例
  - schema 相對原始版本做通用化調整:移除領域專屬欄位(`EOfficeAccount`、`EGovernmentAccount`、`UnitCode`、已棄用的 `Department`);`Basic_LoginLog.LoginMethod` 的列舉註解改為通用文字;`IdNumber` 欄位暫時保留並加 `TODO` 註記待後續決定去留;`NormalPassword varchar(30)` 改為 `PasswordHash varchar(72)` 以配合 BCrypt 雜湊;修正 `Basic_LoginLog` 自我參照 FK 誤建、`Basic_Users.Title` 誤植的 `getdate()` default;`Basic_Users_Groups` 補上 `(GroupId, UserGuid)` 複合主鍵
  - `seed.sql` 提供一組 admin 帳號與範例選單/群組權限資料,使 schema 建好後有可登入、可展示的初始資料
- 後端 `Auth` / `Menu` / `FeatureList` Feature **維持 in-memory dummy 實作不變**,但 Repository 的 TODO 註解升級為對應 `Basic_*` 資料表的完整 Dapper SQL,使用者要接真 DB 時直接解開註解即可
- `template.json` 新增 `postActions`:產生專案後**僅顯示文字指引**(instructions),提示使用者建立資料庫、執行 `db/schema.sql` 與 `db/seed.sql`、`npm install` 等後續步驟;不自動執行任何指令
- `template.json` 新增 `generated guid` symbol:JWT secret 改為「專案名稱 + 隨機 GUID」組合,於 `dotnet new` 產生專案時自動置換 `appsettings.json` 中的 secret,使每個產出專案的金鑰皆不相同

## Capabilities

### New Capabilities

- `vue-app-admin-dotnet8/db-schema`: 範本隨附的 SQL Server 資料庫 schema 與 seed 資料——資料表命名慣例、稽核欄位、MS_Description 擴充屬性、通用化調整規則,以及 seed 資料的最低內容要求

### Modified Capabilities

- `vue-app-admin-dotnet8/template-config`: 新增兩項需求——(1) `postActions` 顯示產生後的下一步文字指引;(2) `JwtSecret` generated guid symbol 於產生時自動置換 `appsettings.json` 的 JWT secret
- `vue-app-admin-dotnet8/data-access`: 新增一項需求——in-memory dummy 的 Repository(Auth/Menu/FeatureList)其 TODO 註解 SHALL 包含對應 `db/schema.sql` 資料表的完整可用 Dapper SQL

## Impact

- **新增檔案**:`vue-app-admin-dotnet8/db/schema.sql`、`vue-app-admin-dotnet8/db/seed.sql`
- **修改檔案**:
  - `vue-app-admin-dotnet8/.template.config/template.json`(postActions、JwtSecret symbol、sources 需確認 `db/` 有被包進範本)
  - `VueAppAdmin.Server/appsettings.json`(JWT secret 佔位字串改為可被 symbol 置換的固定值)
  - `VueAppAdmin.Server/Features/Auth/UserRepository.cs`、`Features/Menu/*`、`Features/FeatureList/*` 的 TODO 註解(僅註解,不改行為)
  - 範本 README(補「資料庫建置」章節)
- **不受影響**:所有 runtime 行為——Auth/Menu/FeatureList 維持 in-memory dummy,現有測試不需改動;`ExampleItems` / `ExampleCategories` 維持 in-memory(依既有 data-access spec)
- **風險/取捨**:
  - JWT secret 改由 symbol 置換後,範本目錄本身(開發用)的 `appsettings.json` 仍是固定字串,僅產出的專案獲得隨機金鑰——範本 repo 內的開發測試行為不變
  - `IdNumber` 欄位含個資敏感性,本次僅標記 TODO 不做決定,留待後續 change 處理

## 1. 資料庫 schema

- [x] 1.1 建立 `vue-app-admin-dotnet8/db/schema.sql`:11 張資料表(`Basic_Users`、`Basic_Groups`、`Basic_Users_Groups`、`Basic_Modules`、`Basic_Group_Modules`、`Basic_LoginLog`、`Basic_OperationLog`、`Basic_Api_Log`、`Basic_Api_Change_Log`、`Para_Categories`、`Para_Info`),含 default constraints、外鍵與全欄位 `MS_Description`,依 design D2 完成通用化調整(刪 `EOfficeAccount`/`EGovernmentAccount`/`UnitCode`/`Department`、`LoginMethod` 描述改通用文字、`IdNumber` 加 TODO 註解、`PasswordHash varchar(72)`、移除 LoginLog 自我參照 FK、移除 `Title` 的 getdate() default、`Basic_Users_Groups` 補複合 PK)。資料表依相依順序排列(Basic_Users 先建立)以確保單檔可一次執行成功
- [x] 1.2 (調整為靜態檢視,不使用 sqlcmd)仔細審閱 `schema.sql` 的 T-SQL 語法與相依順序正確性:確認 11 張表皆存在、`Basic_Users` 排在被參照的 FK 之前、已移除的欄位/自我參照 FK/誤植 default 皆無殘留;實際資料庫執行請使用者以 SSMS 等工具自行驗證
- [x] 1.3 建立 `vue-app-admin-dotnet8/db/seed.sql`:admin 使用者(密碼 `Admin@123`,以 bcryptjs 實際產生並驗證通過的 BCrypt 雜湊,cost=11)、對齊前端 `router/index.ts` 現有路由與 `MenuService.cs` 選單結構的 6 筆 `Basic_Modules`、Administrators 群組(GroupId=1)+ 全模組五種權限旗標 + admin 使用者關聯、`Para_Categories`/`Para_Info` 示範列舉(對應 Basic_LoginLog.Device/ResponseMessage 的列舉說明)
- [x] 1.4 (調整為靜態檢視,不使用 sqlcmd)逐筆比對 `seed.sql` 的 INSERT 欄位與 `schema.sql` 定義一致、`IDENTITY_INSERT` 開關成對出現、FK 參照值(GroupId=1、ModuleId=1-6、UserGuid)在同檔案中皆有對應的 INSERT;實際資料庫執行由使用者以 SSMS 自行驗證

## 2. Repository TODO SQL 註解

- [x] 2.1 更新 `VueAppAdmin.Server/Features/Auth/UserRepository.cs` TODO 註解:對應 `Basic_Users` 的登入驗證 SQL(含 `Status = 1`、鎖定欄位判斷)與 DisplayName 查詢(對應 `UserName`),並修正原註解中「SQL 直接比對 BCrypt 雜湊」的技術謬誤(改為先查出 PasswordHash 再於程式端 BCrypt.Verify)
- [x] 2.2 更新 `VueAppAdmin.Server/Features/Auth/GroupFeatureStore.cs` 註解:附上 `Basic_Users_Groups` JOIN `Basic_Group_Modules` JOIN `Basic_Modules` 的權限查詢 SQL,並註明功能識別字命名與 `ModuleLink` 對應規則待正式接 DB 時決定
- [x] 2.3 更新 `VueAppAdmin.Server/Features/Menu/MenuService.cs` 註解:附上 `Basic_Modules` 選單查詢 SQL 與 `FatherModuleId` 組樹說明
- [x] 2.4 比對三處註解 SQL 與 `db/schema.sql` 的資料表/欄位名稱一致(Basic_Users/Basic_Users_Groups/Basic_Group_Modules/Basic_Modules 皆吻合);`dotnet build` 成功(0 錯誤 0 警告),`VueAppAdmin.Server.Tests` 23 項測試全數通過

## 3. template.json:postActions 與 JwtSecret

- [x] 3.1 修改 `VueAppAdmin.Server/appsettings.json` 與 `appsettings.Development.json`:`Jwt:SignKey` 統一改為 `VueAppAdmin-REPLACE_WITH_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS` 形式(兩檔案佔位字串一致,同一個 symbol 可一併置換);以 Production 環境直接執行編譯後 DLL 驗證啟動正常(`Hosting environment: Production`,無設定繫結錯誤),驗證後已 Stop-Process
- [x] 3.2 修改 `.template.config/template.json`:新增 `JwtSecret` generated guid symbol(`generator: guid`,`parameters.defaultFormat: "n"`,`replaces` 對準佔位字串)——實際 `dotnet new` 驗證產出 32 位小寫十六進位無底線格式,語法正確
- [x] 3.3 修改 `.template.config/template.json`:新增 `postActions`(`actionId: AC1156F7-BB77-4DB8-B28F-24EEBCCA1E5C`,顯示指引文字,不執行指令);**實測發現**此 SDK 版本的 `manualInstructions` 陣列只會顯示第一項,已改為單一 `text` 項目合併全部 4 個步驟

## 4. 範本產生驗證

- [x] 4.1 `dotnet new install` 本範本後於暫存目錄執行 `dotnet new vue-app-admin-dotnet8 -n MyApp`,驗證通過:產出含 `db/schema.sql`、`db/seed.sql`;CLI 完整顯示合併後的 postActions 指引文字
- [x] 4.2 檢查產出的 `appsettings.json`:`Jwt:SignKey` 為 `MyApp-e60bf939aa2f4b9b9b1af1ef4bff3f3c`(`MyApp-` 開頭 + 32 hex,不含佔位字串);`appsettings.Development.json` 同步置換;第二次於不同目錄產生,`SignKey` 為 `MyApp-b96adb66a8f6456cae3a1c448b2d3a97`,兩次不同,驗證通過
- [x] 4.3 產出的 `MyApp.Server` 執行 `dotnet build` 成功(exit code 0);in-memory dummy 不需 DB 故不另外啟動測試;已清理暫存測試目錄,`dotnet new uninstall` 待 Section 5 文件與整體收尾後一併執行

## 5. 文件

- [x] 5.1 更新 `VueAppAdmin.Server/README.md`:`appsettings.json` 範例同步新 SignKey 格式並說明 symbol 自動置換機制;新增「資料庫建置」章節(schema/seed 執行步驟、admin 預設帳密與更改警語、`IdNumber` TODO 提醒、接真 DB 時解開 TODO SQL 的指引)
- [x] 5.2 更新 repo 根目錄 `README.md`:`vue-app-admin-dotnet8` 區塊新增「資料庫」小節(schema/seed/in-memory dummy 現況說明),「開發體驗」補充 SignKey 自動置換與 postActions 指引提示

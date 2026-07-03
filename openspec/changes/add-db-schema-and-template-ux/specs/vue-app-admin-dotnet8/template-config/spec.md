## ADDED Requirements

### Requirement: 產生專案後顯示下一步指引
`template.json` SHALL 定義 `postActions`,於 `dotnet new` 產生專案完成後顯示文字指引(instructions 類 action),SHALL NOT 自動執行任何指令(不使用 run script、restore 等會實際執行動作的 action)。指引內容 SHALL 至少涵蓋:建立資料庫並執行 `db/schema.sql` 與 `db/seed.sql`、確認 `appsettings.json` 的 `ConnectionStrings:Default`、前端 `npm install`、admin 預設帳密與更改提醒。

#### Scenario: 產生專案後顯示指引
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet8 -n MyApp`
- **THEN** CLI 輸出包含建立資料庫、執行 schema/seed、npm install 等後續步驟的文字指引

#### Scenario: 不自動執行指令
- **WHEN** 使用者於無 SQL Server、無 Node.js 的環境產生專案
- **THEN** 專案產生成功,不因 postActions 嘗試執行外部指令而失敗

### Requirement: JWT secret 於產生專案時自動唯一化
`template.json` SHALL 定義 `JwtSecret` symbol(`generated` guid),置換 `appsettings.json` 中 `Jwt:SignKey` 的佔位字串;`Jwt:SignKey` 的範本值 SHALL 為「`VueAppAdmin` + 分隔符 + 佔位字串」形式,使產出專案的 SignKey 成為「專案名稱 + 隨機 GUID」且總長度大於 32 字元。

#### Scenario: 產出專案的 SignKey 含專案名稱與隨機值
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet8 -n MyApp`
- **THEN** 產出的 `appsettings.json` 中 `Jwt:SignKey` 以 `MyApp` 開頭、後接隨機 GUID 字串,且不含原佔位字串

#### Scenario: 不同次產生的 SignKey 不同
- **WHEN** 使用者兩次執行 `dotnet new vue-app-admin-dotnet8 -n MyApp` 於不同目錄
- **THEN** 兩個產出目錄的 `Jwt:SignKey` 值不同

### Requirement: db 資料夾納入範本輸出
範本 `sources` 設定 SHALL 使 `db/schema.sql` 與 `db/seed.sql` 包含於產出專案中。

#### Scenario: 產出專案含 db 資料夾
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet8 -n MyApp`
- **THEN** 產出目錄包含 `db/schema.sql` 與 `db/seed.sql`

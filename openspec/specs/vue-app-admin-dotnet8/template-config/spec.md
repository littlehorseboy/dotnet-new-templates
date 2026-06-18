## ADDED Requirements

### Requirement: 範本可透過 dotnet new 指令安裝與使用
`vue-app-admin-dotnet8` 目錄 SHALL 包含有效的 `.template.config/template.json`，使 .NET SDK 可識別並安裝此範本。

#### Scenario: 安裝範本
- **WHEN** 使用者執行 `dotnet new install .\vue-app-admin-dotnet8`
- **THEN** 安裝成功且無錯誤，`dotnet new list` 顯示 short name `vue-app-admin-dotnet8`

#### Scenario: 產生新專案
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet8 -n MyApp`
- **THEN** 在目前目錄下產生完整的方案目錄 `MyApp/`

### Requirement: sourceName 替換覆蓋所有檔案
`template.json` 的 `sourceName` SHALL 設為 `VueAppAdmin`，SDK 範本引擎 SHALL 將所有出現 `VueAppAdmin` 的地方（檔名、資料夾名、命名空間、檔案內容）替換為使用者指定的 `-n` 名稱。

#### Scenario: 後端命名空間替換
- **WHEN** 使用者以 `-n MyApp` 產生專案
- **THEN** 所有 `.cs` 檔案中的命名空間由 `VueAppAdmin` 替換為 `MyApp`

#### Scenario: 方案與專案檔名替換
- **WHEN** 使用者以 `-n MyApp` 產生專案
- **THEN** `.sln`、`.csproj`、`.esproj` 檔名均包含 `MyApp` 而非 `VueAppAdmin`

### Requirement: 前端目錄名稱以小寫替換
`template.json` SHALL 設定 `nameLower` derived symbol（`lowerCase` transform）加上 `fileRename: "vueappadmin"`，使前端目錄名稱正確替換為全小寫形式。

#### Scenario: 前端目錄名稱小寫
- **WHEN** 使用者以 `-n MyApp` 產生專案
- **THEN** 前端目錄名稱為 `myapp.client`（全小寫），而非 `MyApp.client`

### Requirement: 排除不應納入範本的檔案
`template.json` 的 `sources` 排除清單 SHALL 包含 `bin`、`obj`、`node_modules`、`.vs`、`.git`、`*.user` 等目錄與檔案。

#### Scenario: 產生目錄不含建置產物
- **WHEN** 使用者產生新專案
- **THEN** 產生目錄中不含 `bin/`、`obj/`、`node_modules/`、`.vs/` 等目錄

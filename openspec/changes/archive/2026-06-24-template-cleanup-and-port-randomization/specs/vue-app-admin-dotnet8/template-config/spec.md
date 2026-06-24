## MODIFIED Requirements

### Requirement: 排除不應納入範本的檔案
`template.json` 的 `sources` 排除清單 SHALL 包含 `bin`、`obj`、`dist`、`node_modules`、`.vs`、`.git`、`*.user`、`package-lock.json` 等目錄與檔案。

#### Scenario: 產生目錄不含建置產物
- **WHEN** 使用者產生新專案
- **THEN** 產生目錄中不含 `bin/`、`obj/`、`node_modules/`、`.vs/`、`dist/` 等目錄

#### Scenario: 產生目錄不含 lock file
- **WHEN** 使用者產生新專案
- **THEN** 產生目錄中不含 `package-lock.json`

## ADDED Requirements

### Requirement: 產生專案時 port 自動隨機化
`template.json` SHALL 定義三個 `generated port` symbols，使每次 `dotnet new` 時 HTTP port、HTTPS port、SPA proxy port 各自產生不重疊的五位數隨機值，並替換所有相關 template 檔案中的對應數字。

Symbol 定義：

| Symbol | replaces | range |
|--------|----------|-------|
| `HttpPort` | `5159` | 50100–50199 |
| `HttpsPort` | `7173` | 50200–50299 |
| `IisPort` | `21655` | 50300–50399 |
| `IisSslPort` | `44385` | 50400–50499 |
| `SpaPort` | `23288` | 50500–50599 |

受影響的 template 檔案（`replaces` 字串必須出現於這些檔案中）：
- `VueAppAdmin.Server/Properties/launchSettings.json`
- `VueAppAdmin.Server/VueAppAdmin.Server.http`
- `VueAppAdmin.Server/VueAppAdmin.Server.csproj`
- `vueappadmin.client/.vscode/launch.json`

#### Scenario: 不同次產生的 port 不同
- **WHEN** 使用者兩次執行 `dotnet new vue-app-admin-dotnet8 -n MyApp` 於不同目錄
- **THEN** 兩個產出目錄中的 `launchSettings.json` HTTP port 值有高機率不同（隨機化）

#### Scenario: 同一次產生的 port 一致
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet8 -n MyApp`
- **THEN** 同一個產出目錄內，`launchSettings.json`、`.http` 檔、`.csproj`、`launch.json` 的對應 port 值相同

### Requirement: `.vscode/launch.json` 僅保留 chrome configuration
`vueappadmin.client/.vscode/launch.json` SHALL 只包含 `type: "chrome"` 的 configuration，不得包含 `type: "edge"` 的 configuration。

#### Scenario: launch.json 不含 edge type
- **WHEN** 使用者產生新專案並開啟 VS Code
- **THEN** `.vscode/launch.json` 中不存在 `"type": "edge"` 的 configuration，VS Code 不顯示 debug type 相關警告

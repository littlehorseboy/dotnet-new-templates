## Why

`vue-app-admin-dotnet8` 範本目前有三個具體問題：build artifacts（`dist/`）和 lock file 會被複製進產出目錄；所有 port 值硬碼，每個消費者產生的專案 port 都一樣，容易衝突；方案檔使用舊的 `.sln` 格式。這些問題在每次 `dotnet new` 時都會發生，影響消費者的開發體驗。

## What Changes

- **template.json 排除清單**：新增 `**/dist/**` 與 `**/package-lock.json`，確保 build artifacts 與 lock file 不進入產出
- **Port 隨機化**：在 `template.json` 新增三個 `generated port` symbols（`HttpPort`、`HttpsPort`、`SpaPort`），五位數不重疊範圍，`dotnet new` 時自動替換所有相關檔案的 port 值
- **`.vscode/launch.json` 清理**：移除 `type: "edge"` configuration（觸發 VS Code 警告），保留 `type: "chrome"`；port 值由 `SpaPort` symbol 自動替換
- **`.sln` → `.slnx` 轉換**：方案檔升級為 XML 格式（VS 2022 17.9+），並確保方案 GUID 納入 `template.json` guids 清單

## Capabilities

### New Capabilities

- `vue-app-admin-dotnet8/solution-format`：定義範本產出的方案檔格式（`.slnx`）與其 GUID 替換行為

### Modified Capabilities

- `vue-app-admin-dotnet8/template-config`：新增排除項目（`dist`、`package-lock.json`）、port 隨機化 symbols 規格，以及 `.vscode/launch.json` 的合法內容要求

## Impact

- **`vue-app-admin-dotnet8/.template.config/template.json`**：新增 excludes、新增 symbols、更新 guids
- **`vue-app-admin-dotnet8/VueAppAdmin.Server/Properties/launchSettings.json`**：port 值成為 symbol placeholder（功能不變）
- **`vue-app-admin-dotnet8/VueAppAdmin.Server/VueAppAdmin.Server.http`**：port 值成為 symbol placeholder
- **`vue-app-admin-dotnet8/VueAppAdmin.Server/VueAppAdmin.Server.csproj`**：`SpaProxyServerUrl` port 成為 symbol placeholder
- **`vue-app-admin-dotnet8/vueappadmin.client/.vscode/launch.json`**：移除 edge configuration，port 成為 symbol placeholder
- **`vue-app-admin-dotnet8/VueAppAdmin.sln`**：刪除，改由 `VueAppAdmin.slnx` 取代
- 不影響前後端功能程式碼、認證邏輯、API 行為
- 不影響 `vue-app-demo` 範本

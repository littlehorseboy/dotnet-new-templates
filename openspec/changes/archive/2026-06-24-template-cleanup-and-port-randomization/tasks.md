## 1. template.json 排除清單與 Port Symbols

- [x] 1.1 在 `template.json` 的 `sources[0].modifiers[0].exclude` 新增 `**/dist/**`、`**/package-lock.json`、`**/pnpm-lock.yaml`、`**/pnpm-workspace.yaml`
- [x] 1.2 在 `template.json` 的 `symbols` 新增 `HttpPort` generated port symbol（replaces `5159`，range 50100–50199）
- [x] 1.3 在 `template.json` 的 `symbols` 新增 `HttpsPort` generated port symbol（replaces `7173`，range 50200–50299）
- [x] 1.4 在 `template.json` 的 `symbols` 新增 `IisPort` generated port symbol（replaces `21655`，range 50300–50399）
- [x] 1.5 在 `template.json` 的 `symbols` 新增 `IisSslPort` generated port symbol（replaces `44385`，range 50400–50499）
- [x] 1.6 在 `template.json` 的 `symbols` 新增 `SpaPort` generated port symbol（replaces `23288`，range 50500–50599）
- [x] 1.7 確認 `template.json` 的 `guids`：.slnx 不使用 GUIDs，N/A

## 2. .vscode/launch.json 清理

- [x] 2.1 移除 `vueappadmin.client/.vscode/launch.json` 中 `type: "edge"` 的整個 configuration block，只保留 `type: "chrome"` 那個

## 3. .sln → .slnx 轉換

- [x] 3.1 在 `vue-app-admin-dotnet8/` 目錄建立 `VueAppAdmin.slnx`，內容為 XML 格式，包含 Server 與 client 兩個專案參考
- [x] 3.2 刪除 `vue-app-admin-dotnet8/VueAppAdmin.sln`

## 4. 驗證

- [x] 4.1 執行 `dotnet new install .\vue-app-admin-dotnet8` 重新安裝範本（若已安裝先 uninstall）
- [x] 4.2 執行 `dotnet new vue-app-admin-dotnet8 -n TestApp`，確認產出目錄不含 `dist/`、`package-lock.json`
- [x] 4.3 確認產出目錄的 `launchSettings.json` HTTP port 在 16100–16199 範圍內（實測 16114）
- [x] 4.4 確認產出目錄的 `launchSettings.json` HTTPS port 在 16200–16299 範圍內（實測 16255）
- [x] 4.5 確認產出目錄的 `.csproj` 的 `SpaProxyServerUrl` port 在 16500–16599 範圍內（實測 16500）
- [x] 4.5a 確認產出目錄的 `launchSettings.json` IIS HTTP port 在 16300–16399 範圍內（實測 16318）
- [x] 4.5b 確認產出目錄的 `launchSettings.json` IIS SSL port 在 16400–16499 範圍內（實測 16476）
- [x] 4.6 確認 `.vscode/launch.json` 中不存在 `"type": "edge"`
- [x] 4.7 確認產出目錄含 `TestApp.slnx`，不含 `TestApp.sln`
- [x] 4.8 於產出目錄執行 `dotnet build`，確認建置成功（0 警告 0 錯誤）

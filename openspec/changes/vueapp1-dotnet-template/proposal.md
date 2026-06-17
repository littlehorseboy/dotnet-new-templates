## Why

VueApp1 目前只是一個普通的方案目錄，無法透過 `dotnet new` 指令直接建立新專案。加入 `template.json` 設定後，開發者可以用一行指令從這個範本產生完整的 Vue 3 + ASP.NET Core 專案，不需要手動複製或修改。

## What Changes

- 在 `VueApp1/.template.config/` 目錄下新增 `template.json`
- 設定 `sourceName`，讓專案名稱在所有檔案內容與檔案路徑中自動替換
- 設定 `nameLower` symbol，處理 `vueapp1.client` 資料夾名稱的小寫替換
- 設定 `guids`，讓每次產生的專案擁有唯一 GUID
- 設定排除清單（`.git`、`bin`、`obj`、`node_modules` 等）

## Capabilities

### New Capabilities

- `template-config`: 為 VueApp1 加入 `dotnet new` 範本設定，使其可透過 `dotnet new vue-app-demo` 指令建立新專案

### Modified Capabilities

（無）

## Impact

- 影響範圍：僅新增 `VueApp1/.template.config/template.json`，不修改任何現有程式碼
- 相依性：依賴 `dotnet new install` 指令與 .NET 8 SDK
- 安裝後可透過 `dotnet new vue-app-demo -n MyApp` 建立專案
- Short name 預設為 `vue-app-demo`，需確認不與其他已安裝範本衝突

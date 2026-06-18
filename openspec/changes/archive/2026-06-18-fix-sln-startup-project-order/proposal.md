## Why

Visual Studio 在開啟 `.sln` 時，若無 `.suo` 檔（例如第一次 clone 或 `dotnet new` 後），會以 `.sln` 中第一個宣告的 project 作為預設 startup project。兩個範本目前均將 `.esproj`（前端）排第一，導致綠色箭頭指向前端，無法觸發 `SpaProxy` 自動執行 `npm run dev`。

## What Changes

- `vue-app-admin-dotnet8/VueAppAdmin.sln`：將 `VueAppAdmin.Server.csproj` 的 `Project(...)` 宣告移至 `.esproj` 之前
- `vue-app-demo/VueApp1.sln`：將 `VueApp1.Server.csproj` 的 `Project(...)` 宣告移至 `.esproj` 之前
- `GlobalSection` 區塊內容不變（GUID 對應不受排序影響）

## Capabilities

### New Capabilities

無新增功能面 capability。

### Modified Capabilities

- `sln-startup-order`：`.sln` 中 Server `.csproj` 排列於 client `.esproj` 之前，確保 VS 預設 startup project 為後端

## Impact

- 影響檔案：`vue-app-admin-dotnet8/VueAppAdmin.sln`、`vue-app-demo/VueApp1.sln`
- 不影響任何 C#、TypeScript、或 npm 程式碼
- 不影響 CI/CD 建置行為（`.sln` project 排序不影響 `dotnet build`）
- 對已存在 `.suo` 的開發者無影響（VS 優先讀取 `.suo`）

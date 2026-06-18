## Context

Visual Studio 解析 `.sln` 時，startup project 預設為第一個 `Project(...)` 宣告的 project（當 `.suo` 不存在時）。兩個範本均依賴 `Microsoft.AspNetCore.SpaProxy`：由 Server `.csproj` 啟動後自動呼叫 `npm run dev`，前端 `.esproj` 不需直接執行。

## Goals / Non-Goals

**Goals:**
- 確保 `dotnet new` 產出的 `.sln`，VS 預設 startup project 為 Server `.csproj`
- 同時修正 `vue-app-admin-dotnet8` 與 `vue-app-demo` 兩個範本

**Non-Goals:**
- 不修改 `GlobalSection` 的 GUID 對應順序
- 不修改 `.csproj`、`.esproj` 或任何應用程式碼
- 不處理「多 startup project」情境

## Decisions

**只調換 `Project(...)` 宣告順序，不動其他部分**

`.sln` 格式中，project 宣告順序與 `GlobalSection(ProjectConfigurationPlatforms)` 的 GUID 對應彼此獨立。調換宣告順序後，VS build/deploy 行為不受影響，只有「預設 startup project」會改變。

替代方案考慮：
- 在 `.sln` 中加入 `StartupProject` 設定 → VS 沒有標準的 `.sln` level startup project 欄位，此資訊儲存於二進位 `.suo`，無法直接寫入 `.sln`
- 移除 `.esproj` 不放入 `.sln` → 會失去 VS Solution Explorer 對前端檔案的感知與 IntelliSense，不採用

## Risks / Trade-offs

- **[低風險] 已存在 `.suo` 的開發者** → VS 優先讀取 `.suo`，不受此改動影響，無 migration 需求
- **[無風險] `dotnet build` / CI** → MSBuild 不依賴 project 宣告順序，建置結果不變

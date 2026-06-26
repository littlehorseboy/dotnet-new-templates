## Why

`vue-app-admin-dotnet8` 是一個 dotnet new 範本專案，目前所有程式碼（C# 伺服器端、TypeScript/Vue 前端）幾乎沒有任何行內註解，對於使用此範本的開發者而言，關鍵設計決策、hardcode 轉換表、非直覺的邏輯都缺乏說明，降低了範本的學習價值。同時三份 README 未反映最新的功能與測試指令，`template.json` 在近期變更後需再確認排除清單正確。

## What Changes

- 所有 C# 伺服器端程式碼（39 個檔案）補上繁體中文行內註解
- 所有 TypeScript / Vue 前端程式碼（~23 個檔案）補上繁體中文行內註解
- 所有 C# 測試程式碼（4 個檔案）補上繁體中文行內註解
- `GroupFeatureStore.cs` 特別標注為「demo 用 hardcode 轉換表，實際專案需替換為資料庫查詢」
- `JwtService.cs` 的到期時間（本次由 `externalize-hardcoded-config` change 搬至設定檔）加說明
- `VueAppAdmin.Server.Tests/README.md` 更新：反映新增的 ExampleCategories、Menu 測試
- `VueAppAdmin.Server/README.md` 更新：補上 Menu、FeatureList、ExampleCategories 的 API endpoint 說明
- `vueappadmin.client/README.md` 更新：前端測試指令改為 `npm run test -- --run`（非阻塞式）
- `template.json` 逐項確認近期排除清單變更（`pnpm-workspace.yaml`）無副作用

## Capabilities

### New Capabilities

- `code-annotation`：所有程式碼檔案具備繁體中文行內註解，hardcode 轉換表有特別標注
- `readme-current`：三份 README 反映最新功能、正確測試指令、完整 API 文件

### Modified Capabilities

（無既有 spec 受影響）

## Impact

- **受影響檔案**：`vue-app-admin-dotnet8/` 下所有 `.cs`、`.ts`、`.vue` 原始碼，以及三份 `README.md`
- **非 breaking change**：只加註解與文件，不修改任何邏輯
- **相依**：建議在 `externalize-hardcoded-config` change 實作完成後執行，確保 `JwtOptions.TokenExpirationHours` 的註解內容正確

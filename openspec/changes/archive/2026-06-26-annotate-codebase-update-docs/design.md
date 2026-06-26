## Context

`vue-app-admin-dotnet8` 是 dotnet new 範本，程式碼直接作為學習材料使用。目前沒有任何行內註解，三份 README 也未反映近期新增的 Menu、FeatureList、ExampleCategories feature。

需要特別處理的 hardcode：
- `GroupFeatureStore.cs`：記憶體 Dictionary 模擬使用者→群組→功能權限的對應，是範本中唯一的「demo 轉換表」，需要最醒目的標注
- `JwtService.cs`（等 `externalize-hardcoded-config` 完成後）：`TokenExpirationHours` 說明從哪裡讀取
- `Program.cs`（等 `externalize-hardcoded-config` 完成後）：`logRetentionDays` 說明從設定讀取

## Goals / Non-Goals

**Goals:**
- 每個檔案補上足夠的行內註解，讓首次使用範本的開發者能理解設計意圖
- `GroupFeatureStore.cs` 加上「TODO: 實際專案替換為資料庫查詢」的明確標注
- 三份 README 精確反映：目前所有 feature、正確的測試指令、完整 API 清單
- `template.json` 逐一對照排除清單確認無遺漏或多餘

**Non-Goals:**
- 不修改任何業務邏輯
- 不新增或移除功能
- 不重構程式碼結構

## Decisions

### 決策 1：註解語言使用繁體中文

範本的主要使用者為繁體中文開發者，行內說明文字一律繁體中文。程式碼識別字（類別名、方法名、路徑）保留英文原文。

### 決策 2：註解風格

- C#：`//` 行內註解置於需要說明的程式碼上方或右側；hardcode 轉換表用多行 `//` 區塊標示
- TypeScript/Vue：同上，用 `//` 或 `/** */`（只有公開 function 才用 JSDoc）
- 避免「逐行翻譯程式碼」型的廢話註解，只說明「為什麼」或「注意事項」

### 決策 3：README 測試指令統一

`vueappadmin.client/README.md` 前端測試指令改為 `npm run test -- --run`，避免 Vitest 以 watch mode 執行卡住 terminal。

### 決策 4：template.json 以人工確認為主

`template.json` 的排除清單變動（加入 `pnpm-workspace.yaml`）已確認正確，但實作任務中仍保留一個驗證步驟，確保 `replaces` port 數字與實際 `launchSettings.json`/`vite.config.ts` 一致。

## Risks / Trade-offs

- **過度註解風險**：加太多廢話反而降低可讀性。  
  → 緩解：只在「非直覺邏輯」或「hardcode 需要替換」處加註，不逐行翻譯。
- **README 資訊過時**：範本日後繼續演進，README 可能再次落後。  
  → 這是範本維護的既有問題，本次只確保「當下正確」，不建立自動化機制。

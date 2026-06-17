## Context

`vue-app-demo` 已提供最精簡的 Vue 3 + ASP.NET Core SPA 範本，並透過 `.template.config/template.json` 支援 `dotnet new` 指令。`vue-app-admin-dotnet8` 在相同機制上，新增一個獨立的範本目錄，內含完整的後台管理系統骨架，供需要 JWT 認證與分層架構的新專案直接使用。

## Goals / Non-Goals

**Goals:**
- 提供可透過 `dotnet new vue-app-admin-dotnet8 -n MyApp` 產生的後台管理系統起點
- 包含 JWT 認證（前後端完整流程）
- 包含 Serilog 日誌、後端分層架構骨架（Controller/Service/DTO/Helpers）
- 包含前端完整套件設定（PrimeVue、Bootstrap、FontAwesome、Pinia、Vue Router、Vee-Validate）
- 包含後台主版面（Login → MainLayout → Dashboard）
- `sourceName: VueAppAdmin` 支援完整的名稱替換

**Non-Goals:**
- 不包含資料庫連線（無 Dapper/EF Core），ExampleItems 使用 hardcoded dummy data
- 不包含排程任務框架
- 不包含 LDAP / SSO 認證
- 不包含通知服務（Mail/SMS/Push）
- 不發佈至 NuGet；僅支援本機 `dotnet new install`
- 不修改現有 `vue-app-demo` 範本

## Decisions

**`sourceName` 設為 `"VueAppAdmin"`**
符合 PascalCase 慣例，SDK 範本引擎自動替換所有出現 `VueAppAdmin` 的地方（檔名、資料夾名、命名空間、內容）。前端目錄名稱為全小寫 `vueappadmin.client`，需額外設定 `nameLower` derived symbol + `fileRename: "vueappadmin"`，與 `vue-app-demo` 相同做法。

**JWT 認證選 JWT Bearer，不用 Cookie**
後台管理 SPA 的標準做法：前端以 `Authorization: Bearer <token>` header 傳送 token。Token 存於 `localStorage`，`auth-store` 初始化時自動設定 `axios.defaults.headers.common['Authorization']`。

**Serilog 取代 ASP.NET Core 內建 logging**
Serilog 提供結構化日誌（JSON）與 rolling file，開箱即用，不需自行設計輸出格式。使用 `SerilogHelper.Initialize()` 在 `Program.cs` 最頂端初始化，確保啟動期錯誤也被捕捉。

**PrimeVue + Bootstrap 同時保留**
兩者用途分工：Bootstrap 負責 grid/utility 樣式（設計師常用此框架交付 HTML），PrimeVue 提供現成的 DataTable/Dialog/Toast 等複雜元件。非重複，是刻意的工作流程選擇。

**Sidebar 從 `router meta.showInSidebar` 衍生**
單一來源：新增路由時在 `meta` 加 `showInSidebar: true`，sidebar 自動顯示，不需另外維護選單陣列。開新專案只需增減路由即可調整選單。

**ExampleItems 使用 hardcoded dummy data**
範本目的是展示架構分層，非示範 DB 操作。加入 DB 連線會引入 connection string 設定複雜度，超出範本應有範疇。使用者可在實際專案中自行替換資料來源。

**靜態路由，不從 API 動態載入**
動態路由對範本使用者而言過度複雜。靜態路由清楚易懂，容易客製，是更適合作為起點的設計。

## Risks / Trade-offs

- **前端 bundle 較大** → PrimeVue + Bootstrap + FontAwesome 增加 bundle 體積。已知取捨，符合目標使用情境。
- **`sourceName` 替換遺漏** → 若有檔案寫死 `VueAppAdmin` 未被替換，scaffold 產出會有殘留舊名。緩解：scaffold 後全文搜尋 `VueAppAdmin` 確認。
- **套件版本過時** → 範本是靜態快照，套件不會自動更新。緩解：定期執行 `npm update` + NuGet 更新並重新驗證 scaffold。
- **`vite.config.ts` 中 `dotenv.config()` 順序敏感** → 需在 `defineConfig` 之前呼叫，設計中已明確規定。

## Open Questions

- `VueAppAdmin` 作為 `sourceName` 是否最終確定？（確認後不易更改，影響所有替換邏輯）
- `appsettings.json` 的 JWT `SignKey` 預設值放 placeholder 字串還是產生隨機值？（目前計劃放 placeholder，使用者需自行替換）

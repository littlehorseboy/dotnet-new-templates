## Context

`vue-app-admin-dotnet8` 是 Vue 3 + .NET 8 的後台管理範本，所有資料均為 in-memory hardcode，無真實資料庫。目前僅有基本 JWT 登入、單一層級靜態選單與簡單 GET 查詢。此次新增的五個能力需要在不引入 DB 的前提下，呈現出與真實後台系統相同的 API 結構與前端互動模式。

## Goals / Non-Goals

**Goals:**
- 實作 Group + Feature 三層權限模型並整合進 JWT / GetMe
- 實作後端過濾的樹狀選單 API
- 將 ExampleItems 查詢改為 POST，加入模糊查詢與類別複選
- 提供 Categories 清單 API
- 補上後端 Service 層單元測試與前端 composable/store 測試

**Non-Goals:**
- Group / Feature 的管理 UI（CRUD 頁面）
- 真實資料庫存取
- 使用者自助註冊
- E2E / 整合測試（Playwright）

## Decisions

### 1. Features 存入 JWT Claims

**決策**：登入時將使用者的 features[] 寫入 JWT，後續請求從 token 中讀取，不額外打 DB。

**理由**：範本無 DB，且示範目的是讓後端 API（如 Menu）能從 JWT 中直接讀到 features，不需要再查使用者資料。

**替代方案**：每次請求查 Service ← 過度設計，範本不需要。

---

### 2. 選單過濾在後端做

**決策**：`POST /api/Menu/Items` 讀取 JWT claims 中的 features，遞迴過濾 MenuNode 後才回傳。

**理由**：前端看不到「自己沒權限的選單節點」，示範後端過濾的正確模式。

**替代方案**：前端自行過濾 ← 洩漏完整選單結構給前端，不符合安全示範目的。

---

### 3. 查詢端點一律用 POST

**決策**：`POST /api/ExampleItems/Search`、`POST /api/Categories`，request body 傳查詢條件。

**理由**：團隊 API 設計習慣，查詢條件複雜時 POST body 比 querystring 更易擴充。

**替代方案**：GET + querystring ← 不符合團隊慣例。

---

### 4. 後端測試專案已存在，直接新增測試檔

**決策**：`VueAppAdmin.Server.Tests` 已有 xUnit + NSubstitute + coverlet，直接在既有結構下新增 Feature 對應的測試檔，不動 csproj。

**理由**：避免重複建置設定，NSubstitute 可滿足 mock 需求。

---

### 5. 前端測試範圍限 composables + stores

**決策**：Vitest 只測 `useTheme.ts`、`auth-store.ts`、`user-info-store.ts`，以及 api module（mock axios）。不做 component 掛載測試。

**理由**：範本示範測試模式為主，component 測試投入成本高但範本複製價值低。

## Risks / Trade-offs

- **JWT payload 膨脹**：features[] 越多 token 越大。目前 hardcode 數量有限，可接受；真實場景應考慮 feature 代碼精簡化。→ 無需緩解，範本明確標示此限制即可。
- **選單過濾遞迴邏輯**：父節點所有子節點被過濾後，父節點本身也應隱藏。→ 後端 `MenuService` 過濾時需遞迴處理，spec 中明確定義此行為。
- **POST /api/ExampleCategories 語意**：端點名稱與 ExampleItems 保持一致，明確為示範資料，團隊 POST 慣例下無語意歧義。

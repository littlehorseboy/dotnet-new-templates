## Context

Vue client 原本以 `axios` 全域 defaults 管理 token（`axios.defaults.headers.common['Authorization']`），需在應用啟動時明確呼叫 `authStore.init()` 才能恢復 token。API 呼叫直接散落在各 view/store，型別為 inline interface，未對應 server 的 `ApiResponse<T>` 包裝格式。錯誤處理各自為政，401 的處理更沒有區分是否來自 login endpoint。

## Goals / Non-Goals

**Goals:**
- 建立集中的 axios instance，token 注入與錯誤處理統一在 interceptors 處理
- 每個 feature 有對應的 API module，view/store 不直接接觸 axios
- TypeScript 型別完整對應 server `ApiResponse<T>` 格式
- 補齊基本 UX：login loading state、API error 回饋、404 頁面、navigation progress bar、sidebar icon、document title、logout confirm

**Non-Goals:**
- Token refresh / silent renew（需 server 端配合 refresh token endpoint，留待後續）
- i18n（template 定位為中文後台）
- Mobile responsive（後台系統以 desktop 為主）
- 全域 Toast notification 系統（留待後續）

## Decisions

### 1. Axios instance interceptor 取代 global defaults 管理 token

**選擇**：在 `src/lib/axios.ts` 建立 `axios.create()` instance，request interceptor 每次請求時從 `localStorage` 讀取 token。

**理由**：`axios.defaults` 是全域狀態，若 app 中有其他 axios 使用（如第三方套件）可能相互干擾；`create()` instance 隔離作用範圍。每次從 localStorage 讀取 token 消除了「需要在對的時機呼叫 init()」的問題，頁面重整後自動恢復無需額外初始化。

**捨棄方案**：繼續使用 `axios.defaults` + `authStore.init()`——需手動管理初始化時機，且 token 過期/清除後若忘記清 header 會造成安全問題。

### 2. Login endpoint 401 不觸發 redirect

**選擇**：response interceptor 遇 401 時，若 `error.config?.url` 包含 `/api/Auth/Login`，不執行 redirect，讓錯誤往上傳至 view 的 catch 處理。

**理由**：login endpoint 的 401 代表「帳密錯誤」，不是「session 過期」。若觸發 redirect，使用者停在 login 頁面（會造成 full page reload），且 LoginView 顯示的錯誤訊息永遠不會出現。

### 3. Feature-level API modules

**選擇**：`src/api/auth.api.ts`、`src/api/example-items.api.ts`，每個 function 回傳已 unwrap 的資料（非 `ApiResponse<T>` 包裝）。

**理由**：view 只需要關心「成功的資料是什麼」，error 由 interceptor 或 try/catch 統一處理。unwrap 邏輯集中在 API module，view 不需要了解 `ApiResponse` 格式。

### 4. NavigationProgress 使用 PrimeVue ProgressBar

**選擇**：`src/components/NavigationProgress.vue` 使用 `primevue/progressbar`（value-based），搭配 router `beforeEach`/`afterEach` 控制 width 值（30% → 70% → 100%）。

**理由**：PrimeVue 已是既有依賴，不需要額外安裝 NProgress，styling 以 `:deep()` 覆蓋達到 3px 頂部進度條效果。

## Risks / Trade-offs

- **`isRedirectingToLogin` flag 跨 tab 問題**：多個 browser tab 同時 token 過期，各自有獨立的 module scope，flag 不跨 tab 共享，可能多次觸發 redirect。→ 可接受，每個 tab 重導向至 login 是預期行為，不影響功能。
- **non-null assertion (`data.result!`)**：API modules 對 `result` 使用 `!` 斷言，若 server 非預期回傳 `null` 會在 runtime 靜默失敗。→ 此 template 為已知 server 合約，可接受；若 server 行為不確定可改為 optional chaining + throw。
- **`confirm()` 做 logout 確認**：瀏覽器原生 `confirm()` 外觀與 UI 框架不一致。→ 可接受，template 層級夠用；若需要統一視覺可改用 PrimeVue ConfirmDialog。

## ADDED Requirements

### Requirement: vite.config.ts 支援 HTTPS、proxy 與 @ alias
`vite.config.ts` SHALL 設定：（1）自動產生或讀取 ASP.NET Core dev cert（`.pem`/`.key`）供 HTTPS；（2）`/api` proxy 至後端（port 由 `ASPNETCORE_HTTPS_PORT` env 決定）；（3）`@` alias 指向 `src/`；（4）以 `dotenv.config()` 載入 `.env`（在 `defineConfig` 之前呼叫）。

#### Scenario: 開發期 HTTPS 正確啟動
- **WHEN** 執行 `npm run dev`
- **THEN** Vite dev server 以 HTTPS 啟動，憑證來自 ASP.NET Core dev cert 路徑

#### Scenario: API 請求被 proxy 至後端
- **WHEN** 前端發送 `GET /api/Auth/Me`
- **THEN** Vite proxy 將請求轉發至後端 `https://localhost:<port>/api/Auth/Me`

#### Scenario: @ alias 可正常 import
- **WHEN** 程式碼使用 `import { useAuthStore } from '@/stores/auth-store'`
- **THEN** TypeScript 與 Vite 均可正確解析路徑

### Requirement: 環境變數以 .env 與 .env.example 管理
`.env.example` SHALL 列出所有 `VITE_` 開頭的環境變數及說明，`.env` 為實際值（不納入版本控制）。範本 SHALL 包含 `VITE_DEFINECONFIG_BASE`（生產部署子路徑，預設 `/`）。

#### Scenario: .env.example 存在且含必要變數
- **WHEN** 開發者 clone 專案後查看 `.env.example`
- **THEN** 可得知所有必要環境變數的名稱與用途說明

### Requirement: 前端套件清單包含完整管理系統所需依賴
`package.json` dependencies SHALL 包含：`pinia`、`vue-router`、`axios`、`primevue`、`@primevue/themes`、`primeicons`、`bootstrap`、`bootstrap-icons`、`@fortawesome/fontawesome-svg-core`、`@fortawesome/free-solid-svg-icons`、`@fortawesome/free-regular-svg-icons`、`@fortawesome/free-brands-svg-icons`、`@fortawesome/vue-fontawesome`、`vee-validate`、`@vee-validate/yup`、`yup`、`date-fns`、`lodash-es`、`uuid`。

#### Scenario: npm install 成功無衝突
- **WHEN** 執行 `npm install`
- **THEN** 所有套件安裝成功，無 peer dependency 衝突

### Requirement: main.ts 完整初始化所有套件
`main.ts` SHALL 初始化 Vue app 並 `use()` PrimeVue（含 theme）、Pinia、Vue Router，並呼叫 `auth-store.init()` 設定初始認證狀態。

#### Scenario: 應用程式啟動完整初始化
- **WHEN** 前端應用程式啟動
- **THEN** PrimeVue、Pinia、Vue Router 均已掛載，`auth-store.init()` 已執行

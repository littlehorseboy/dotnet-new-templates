## ADDED Requirements

### Requirement: auth-store 管理 JWT token 狀態
`auth-store.ts` SHALL 以 Pinia 定義，state 包含 `isAuthenticated: boolean`，初始值依 `localStorage.getItem('SiteToken')` 是否存在決定。`init()` action SHALL 在應用程式啟動時設定 `axios.defaults.headers.common['Authorization']`。

#### Scenario: 頁面重整後維持登入狀態
- **WHEN** 使用者重整頁面，`localStorage` 存有 `SiteToken`
- **THEN** `isAuthenticated` 為 `true`，axios 自動攜帶 Bearer token

#### Scenario: 登入後更新狀態
- **WHEN** `login(token)` action 被呼叫
- **THEN** token 存入 `localStorage`，`isAuthenticated` 設為 `true`，axios header 更新

#### Scenario: 登出後清除狀態
- **WHEN** `logout()` action 被呼叫
- **THEN** `localStorage` 的 `SiteToken` 被移除，`isAuthenticated` 設為 `false`，axios Authorization header 被清除

### Requirement: LoginView 提供帳號密碼登入表單
`LoginView.vue` SHALL 使用 Vee-Validate + Yup 做表單驗證，欄位包含 `username`（必填）與 `password`（必填），送出後呼叫 `POST /api/Auth/Login`，成功後導向 `/dashboard`，失敗顯示錯誤訊息。

#### Scenario: 登入成功導向 dashboard
- **WHEN** 使用者輸入有效帳密並提交
- **THEN** `auth-store.login(token)` 被呼叫，router 導向 `/dashboard`

#### Scenario: 登入失敗顯示錯誤
- **WHEN** 使用者輸入無效帳密並提交
- **THEN** 表單顯示「帳號或密碼錯誤」訊息，不跳轉頁面

#### Scenario: 空白欄位提交觸發驗證
- **WHEN** 使用者未填寫欄位即按送出
- **THEN** Vee-Validate 顯示欄位必填錯誤，不發送 API 請求

### Requirement: router beforeEach 守衛保護需認證的路由
`router/index.ts` 的 `beforeEach` 守衛 SHALL 檢查 `auth-store.isAuthenticated`，未登入時 SHALL 導向 `/login`。`meta.noAuthRequired: true` 的路由（如 `/login`）不受守衛限制。

#### Scenario: 未登入存取受保護頁面
- **WHEN** 未登入使用者嘗試進入 `/dashboard`
- **THEN** router 導向 `/login`

#### Scenario: 已登入存取 login 頁面
- **WHEN** 已登入使用者進入 `/login`
- **THEN** router 導向 `/dashboard`（避免重複登入）

### Requirement: user-info-store 儲存登入使用者基本資訊
`user-info-store.ts` SHALL 以 Pinia 定義，state 包含 `username: string` 與 `displayName: string`，登入成功後由 `GET /api/Auth/Me` 回應填入。

#### Scenario: 登入後取得使用者資訊
- **WHEN** 登入成功並呼叫 `fetchUserInfo()` action
- **THEN** `username` 與 `displayName` 以 API 回應更新

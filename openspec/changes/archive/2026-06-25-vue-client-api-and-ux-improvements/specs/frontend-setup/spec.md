## MODIFIED Requirements

### Requirement: main.ts 完整初始化所有套件
`main.ts` SHALL 初始化 Vue app 並 `use()` PrimeVue（含 theme）、Pinia、Vue Router。`main.ts` SHALL NOT 呼叫 `auth-store.init()`，亦不 import `useAuthStore`；token 注入由 `src/lib/axios.ts` 的 request interceptor 在每次請求時自動處理，不需要啟動時的初始化步驟。PrimeVue theme preset SHALL 從 `@primeuix/themes/aura` 匯入。

#### Scenario: 應用程式啟動完整初始化
- **WHEN** 前端應用程式啟動
- **THEN** PrimeVue、Pinia、Vue Router 均已掛載；不存在 `authStore.init()` 呼叫

#### Scenario: 頁面重整後 API 請求仍自動攜帶 token
- **WHEN** 使用者重整頁面後（`main.ts` 重新執行），`localStorage` 存有 `SiteToken`，並發出 API 請求
- **THEN** 請求自動攜帶 Authorization header，無需 `main.ts` 做任何 token 相關初始化

## MODIFIED Requirements

### Requirement: 前端單元測試基礎架構

前端 SHALL 安裝 `vitest`、`@vue/test-utils`、`happy-dom`、`@vitest/coverage-v8`，並在 `vite.config.ts` 加入 test 設定。

#### Scenario: 測試指令可執行

- **WHEN** 執行 `npm run test`
- **THEN** Vitest 執行所有 `*.spec.ts` 測試並輸出結果

---

### Requirement: 前端 composable 與 store 單元測試

前端 SHALL 包含下列測試檔：

- `useTheme.spec.ts`：init 讀取 localStorage、toggle 切換 isDark 並寫入 localStorage
- `auth-store.spec.ts`：login 儲存 token、logout 清除 token、isAuthenticated 狀態正確
- `user-info-store.spec.ts`：`hasFeature()` 有/無 feature 時回傳正確布林值

#### Scenario: useTheme toggle 測試

- **WHEN** 呼叫 `toggle()` 兩次
- **THEN** `isDark` 回到初始值，localStorage 中的 `theme` 值也對應正確

#### Scenario: auth-store login/logout 測試

- **WHEN** 呼叫 `login('fake-token')` 後呼叫 `logout()`
- **THEN** `isAuthenticated` 先為 true 後為 false，localStorage token 對應清除

#### Scenario: hasFeature 測試

- **WHEN** store 中 `features` 為 `["items:read"]`
- **THEN** `hasFeature('items:read')` 回傳 true，`hasFeature('items:write')` 回傳 false

---

### Requirement: 前端 Vue 元件測試

前端 SHALL 包含 `src/views/__tests__/ExampleItemsView.spec.ts`，使用 `@vue/test-utils` 測試 Vue 元件層行為，涵蓋 API mock、非同步載入、使用者互動觸發 API 重呼叫等 scenario。

詳細規格見 `specs/frontend-component-test/spec.md`。

#### Scenario: 元件測試檔存在且可執行

- **WHEN** 執行 `npm run test`
- **THEN** `ExampleItemsView.spec.ts` 中的所有 test case SHALL 通過，無 skipped

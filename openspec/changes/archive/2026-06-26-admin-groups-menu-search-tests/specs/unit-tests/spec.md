## ADDED Requirements

### Requirement: 後端 Service 層單元測試

`VueAppAdmin.Server.Tests` SHALL 包含涵蓋下列 Service 的 xUnit 測試，使用 xUnit 內建 Assert（不依賴商業授權套件），視需要以 NSubstitute mock 相依介面：

- `ExampleItemsServiceTests`：分頁、排序、名稱模糊查詢、說明模糊查詢、類別複選過濾
- `ExampleCategoriesServiceTests`：回傳 3 筆資料
- `MenuServiceTests`：admin 取完整樹、viewer 過濾後結果正確、群組節點子全空時自動移除
- `AuthServiceTests`：正確密碼驗證成功、錯誤密碼驗證失敗

#### Scenario: ExampleItemsService 名稱模糊查詢測試

- **WHEN** 呼叫 `Search` 傳入 `name: "item 1"`
- **THEN** 回傳結果的每筆 Name 均包含 "item 1"（不分大小寫）

#### Scenario: MenuService 過濾測試

- **WHEN** 傳入 `features: ["items:read"]` 呼叫 `GetFilteredMenu`
- **THEN** 回傳樹中不含 `requiredFeature` 為 `categories:manage` 或 `menu:admin` 的節點，且「系統管理」群組節點不出現

#### Scenario: AuthService 驗證測試

- **WHEN** 傳入正確帳密
- **THEN** 回傳使用者資訊；傳入錯誤密碼時回傳 null 或拋出例外

---

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

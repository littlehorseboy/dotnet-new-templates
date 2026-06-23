## MODIFIED Requirements

### Requirement: 前端套件清單包含完整管理系統所需依賴
`package.json` dependencies SHALL 包含：`pinia`、`vue-router`（^5.x）、`axios`、`primevue`、`@primeuix/themes`（取代已 deprecated 的 `@primevue/themes`）、`primeicons`、`bootstrap`、`bootstrap-icons`、`@fortawesome/fontawesome-svg-core`、`@fortawesome/free-solid-svg-icons`、`@fortawesome/free-regular-svg-icons`、`@fortawesome/free-brands-svg-icons`、`@fortawesome/vue-fontawesome`、`vee-validate`、`@vee-validate/yup`、`yup`、`date-fns`、`lodash-es`、`uuid`。devDependencies SHALL 包含 `dotenv`（^17.x）與 `npm-run-all2`（^9.x）。

#### Scenario: pnpm install 成功無衝突
- **WHEN** 執行 `pnpm install`
- **THEN** 所有套件安裝成功，無 peer dependency 衝突，且 `@primevue/themes` 不存在於 node_modules 中

#### Scenario: @primevue/themes 已移除
- **WHEN** 查看 `package.json` dependencies
- **THEN** 不存在 `@primevue/themes` 條目，僅存在 `@primeuix/themes`

### Requirement: main.ts 完整初始化所有套件
`main.ts` SHALL 初始化 Vue app 並 `use()` PrimeVue（含 theme）、Pinia、Vue Router，並呼叫 `auth-store.init()` 設定初始認證狀態。PrimeVue theme preset SHALL 從 `@primeuix/themes/aura` 匯入（不得使用 `@primevue/themes/aura`）。

#### Scenario: 應用程式啟動完整初始化
- **WHEN** 前端應用程式啟動
- **THEN** PrimeVue、Pinia、Vue Router 均已掛載，`auth-store.init()` 已執行

#### Scenario: Aura preset 從正確路徑匯入
- **WHEN** 查看 `main.ts` 的 import 宣告
- **THEN** `Aura` 從 `@primeuix/themes/aura` 匯入，不存在任何指向 `@primevue/themes` 的 import

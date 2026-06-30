## Why

現有前端測試僅涵蓋 composable 與 Pinia store，沒有任何 Vue 元件層測試。`ExampleItemsView` 是 template 中最複雜的頁面（含搜尋表單、分頁、API 呼叫），作為範例卻缺乏元件測試示範，使用者複製 template 後無從參考如何測試 Vue 元件。

## What Changes

- 新增 `ExampleItemsView.spec.ts`，使用 Vitest + Vue Test Utils + MSW（或 `vi.mock`）示範如何 mock API 回應並測試元件行為
- 測試涵蓋：初始渲染、搜尋表單互動、API 回應渲染、分頁切換
- 更新 `test-structure` spec，補充前端 Vue 元件測試的規範與驗收標準

## Capabilities

### New Capabilities

- `frontend-component-test`：Vue 元件測試規範，涵蓋如何使用 Vue Test Utils 掛載元件、mock API 層、觸發使用者互動並驗證 DOM 輸出

### Modified Capabilities

- `test-structure`：新增「前端 Vue 元件測試」requirement，定義 `ExampleItemsView.spec.ts` 應涵蓋的 scenario

## Impact

- 影響範圍：`vue-app-admin-dotnet8/vueappadmin.client/src/views/__tests__/`（新增目錄與測試檔）
- 可能需要安裝 `@vue/test-utils`（確認是否已在 package.json 中）
- 不影響任何現有程式碼與 API；純測試層新增

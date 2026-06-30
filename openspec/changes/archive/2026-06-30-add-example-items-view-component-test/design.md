## Context

`ExampleItemsView.vue` 是 template 中最複雜的頁面，含有：
- `onMounted` 同時呼叫 `getExampleCategories()` 與 `searchItems()`
- 搜尋欄位 (name / description) 使用 300ms debounce
- 類別多選 (PrimeVue MultiSelect) 立即觸發搜尋
- 分頁與排序透過 PrimeVue DataTable 的 `lazy` mode 回呼

API 模組（`example-items.api.ts`、`example-categories.api.ts`）使用具名 export，方便 `vi.mock()` 替換。`@vue/test-utils` v2.4.11 已在 devDependencies，不需新增套件。

## Goals / Non-Goals

**Goals:**
- 在 `src/views/__tests__/ExampleItemsView.spec.ts` 新增元件測試
- 示範如何 mock API 模組、stub PrimeVue 元件、等待非同步操作
- 涵蓋 4 個核心 scenario：初始載入、API 失敗錯誤提示、名稱篩選觸發 API、分頁切換觸發 API

**Non-Goals:**
- E2E 測試（Playwright / Cypress）
- 測試 PrimeVue 元件本身的行為
- 測試 debounce 計時（`vi.useFakeTimers` 可留作進階示範，不列入基本需求）
- 100% 程式碼覆蓋率

## Decisions

### 使用 `vi.mock()` 而非 MSW

`vi.mock('@/api/example-items.api')` 直接替換模組，設定簡單、執行快。MSW 適合整合測試情境，對純元件測試過重。

### Stub PrimeVue 元件

`DataTable`、`Column`、`MultiSelect` 在 happy-dom 環境中無法完整運作。透過 `mount({ global: { stubs: ['DataTable', 'Column', 'MultiSelect'] } })` 以空元件替換，讓測試專注在元件邏輯而非 PrimeVue 內部實作。

### 等待非同步：`flushPromises()`

`onMounted` 內有兩個 API 呼叫，掛載後需呼叫 `await flushPromises()` 讓所有 pending Promise 完成，再對 DOM 做斷言。

### 不測試 Router

元件本身不使用 `useRoute` 或 `useRouter`，不需要提供 router stub。

## Risks / Trade-offs

- **PrimeVue stub 讓 DataTable 渲染消失**：測試無法驗證資料列是否正確呈現在 table 裡，但仍可驗證 `searchItems` 是否被正確呼叫及帶入正確參數。這是合理的取捨。
  → 如需驗證 DOM，可在測試中直接斷言 `wrapper.vm.items` 的值。
- **debounce 測試略過**：名稱篩選測試需用 `vi.useFakeTimers()` 控制 setTimeout，增加複雜度。基礎 spec 不要求，但實作時可加入進階示範。

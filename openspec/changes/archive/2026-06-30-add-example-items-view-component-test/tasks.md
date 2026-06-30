## 1. 建立測試目錄與檔案骨架

- [x] 1.1 在 `src/views/` 下建立 `__tests__/` 目錄
- [x] 1.2 建立空的 `ExampleItemsView.spec.ts`，確認 `npm run test` 可偵測到此檔案並通過（0 tests）

## 2. 設定 mock 與測試輔助

- [x] 2.1 在測試頂層加入 `vi.mock('@/api/example-items.api')` 與 `vi.mock('@/api/example-categories.api')`，確認 import 不會連到真實 axios
- [x] 2.2 準備 `mountComponent()` 輔助函式，設定 `global.stubs` 替換 `DataTable`、`Column`、`MultiSelect`，並提供 Pinia（`createTestingPinia()`）

## 3. 實作核心 test case

- [x] 3.1 **初始掛載**：mock `searchItems` 回傳 `{ items: [{...}], total: 1 }`，`await flushPromises()` 後斷言 `wrapper.vm.items.length === 1` 且 `wrapper.vm.loading === false`
- [x] 3.2 **API 失敗**：mock `searchItems` 拋出 Error，`await flushPromises()` 後斷言 `.alert-danger` 存在且文字含錯誤訊息
- [x] 3.3 **名稱篩選**：設定名稱 input 值並觸發查詢按鈕 click，`await flushPromises()` 後斷言 `searchItems` 第二次呼叫的 `name` 參數正確
- [x] 3.4 **分頁切換**：呼叫 `wrapper.vm.onPage({ first: 10, rows: 10 })`，`await flushPromises()` 後斷言 `searchItems` 呼叫時 `page === 2`、`pageSize === 10`

## 4. 驗收

- [x] 4.1 執行 `npm run test`，確認 4 個 test case 全部 pass，無 skip
- [x] 4.2 執行 `npm run test:coverage`，確認 `ExampleItemsView.vue` 有被涵蓋

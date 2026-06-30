## 1. ExampleItemsView.vue — Script 調整

- [x] 1.1 移除 `debounceTimer` 變數宣告及相關 `clearTimeout` 呼叫
- [x] 1.2 刪除 `onFilterChange` 函式
- [x] 1.3 刪除 `onCategoryChange` 函式
- [x] 1.4 新增 `onSearch()` 函式：重設 `lazyParams.value.first = 0` 後呼叫 `loadItems()`

## 2. ExampleItemsView.vue — Template 調整

- [x] 2.1 移除兩個文字輸入框的 `form-control-sm`，改為 `form-control`
- [x] 2.2 移除文字輸入框的 `@input="onFilterChange"`，補上 `@keyup.enter="onSearch"`
- [x] 2.3 移除 MultiSelect 的 `@change="onCategoryChange"`
- [x] 2.4 調整欄寬：名稱與說明輸入框改為 `col-md-3`，MultiSelect 保持 `col-md-4`
- [x] 2.5 新增查詢按鈕欄（`col-12 col-md-2`），內含 `<button class="btn btn-primary w-100" @click="onSearch">查詢</button>`

## 3. 驗收確認

- [x] 3.1 瀏覽器確認篩選列所有控件高度視覺一致
- [x] 3.2 驗證輸入文字後不自動發出 API 請求（DevTools Network 觀察）
- [x] 3.3 驗證點擊「查詢」按鈕正確觸發 API 請求並回到第一頁
- [x] 3.4 驗證在文字輸入框按 Enter 與點擊按鈕行為相同
- [x] 3.5 驗證 MultiSelect 選取後不自動觸發查詢

## Why

`ExampleItemsView` 篩選列目前有兩個問題：文字輸入框使用 `form-control-sm`，高度與 PrimeVue MultiSelect 預設高度不一致，視覺上參差不齊；篩選條件異動後立即觸發 API 查詢（debounce 或即時），使用者在尚未輸入完整條件前就送出多餘請求，體驗不佳且增加不必要的後端負載。

## What Changes

- 移除文字輸入框的 `form-control-sm`，改用 `form-control`，讓高度與 PrimeVue MultiSelect 預設高度一致
- 移除 debounce 自動查詢邏輯（`debounceTimer`、`onFilterChange`）
- 移除 MultiSelect 的即時查詢邏輯（`onCategoryChange`）
- 新增「查詢」按鈕，由使用者主動點擊才送出 API 請求
- 文字輸入框加上 Enter 鍵觸發查詢（`@keyup.enter`）
- 篩選列改為四欄排版（名稱 / 說明 / 類別 / 查詢按鈕）

## Capabilities

### New Capabilities

無

### Modified Capabilities

- `vue-app-admin-dotnet8/example-items-search`：前端篩選 UI 需求更新——輸入框高度統一改用 `form-control`；觸發查詢的時機從「debounce / 即時」改為「點擊查詢按鈕」

## Impact

- 受影響檔案：`vueappadmin.client/src/views/ExampleItemsView.vue`
- API 行為、後端、型別定義均不受影響
- 現有 `example-items-search` spec 中「前端 ExampleItemsView 篩選 UI」需求的觸發時機描述需更新

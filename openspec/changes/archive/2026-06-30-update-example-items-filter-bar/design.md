## Context

`ExampleItemsView.vue` 篩選列由兩個 Bootstrap `form-control-sm` 文字輸入框與一個 PrimeVue `MultiSelect` 組成。目前文字欄位靠 debounce（300ms）自動查詢，`MultiSelect` 選項變更後即時查詢。PrimeVue MultiSelect 的預設高度（約 40px）與 Bootstrap `form-control-sm`（約 31px）不一致，同列控件高度落差明顯。

## Goals / Non-Goals

**Goals:**
- 所有篩選控件高度對齊：以 PrimeVue MultiSelect 預設高度為基準，文字輸入框改用 `form-control`
- 使用者點擊「查詢」按鈕後才送出 API 請求，移除自動觸發邏輯
- 篩選列排版調整為四欄（名稱 / 說明 / 類別 / 查詢按鈕）

**Non-Goals:**
- 後端 API 不改動
- 不加「重設」按鈕（本次僅加查詢按鈕）
- 不動 `onPage`、`onSort`（換頁與排序仍即時觸發）
- 不在 MultiSelect 加 Enter 鍵（PrimeVue 元件內部行為不介入）

## Decisions

**D1：輸入框改用 PrimeVue `InputText` 元件**
以 PrimeVue MultiSelect 的高度為基準，改用 PrimeVue `InputText` 取代 Bootstrap `form-control`，讓兩個輸入框與 MultiSelect 同屬 Aura 主題，高度由同一套設計系統決定，無需額外 CSS 覆寫。

**D2：移除 debounce，改為明確 `onSearch()` 函式**
刪除 `debounceTimer`、`onFilterChange`、`onCategoryChange`，新增 `onSearch()` 統一重設分頁並呼叫 `loadItems()`。查詢按鈕的 `@click` 綁定 `onSearch`。

**D3：篩選列欄寬 col-md-3 / col-md-3 / col-md-4 / col-md-2**
查詢按鈕佔 col-md-2，類別 MultiSelect 保持 col-md-4，兩個文字欄位各 col-md-3，合計 12 欄。

**D4：文字輸入框監聽 `@keyup.enter`**
名稱與說明輸入框加上 `@keyup.enter="onSearch"`，讓使用者按 Enter 可直接觸發查詢，與點擊按鈕行為相同。MultiSelect 不介入（PrimeVue 元件內部行為）。

## Risks / Trade-offs

- [體驗差異] 移除 debounce 後，使用者需主動點擊才查詢，對習慣即時篩選的使用者需適應。→ 可接受，為本次明確需求。Enter 鍵可觸發查詢作為補充，降低操作成本。

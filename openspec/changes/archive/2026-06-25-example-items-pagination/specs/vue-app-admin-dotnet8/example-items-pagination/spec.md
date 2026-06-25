## ADDED Requirements

### Requirement: ExampleItems 列表支援 server-side 分頁
`GET /api/ExampleItems` SHALL 接受 `skip`（int，預設 0）、`top`（int，預設 10）query parameters，回傳當頁資料與 `total`（資料庫總筆數）。後端 SHALL 依 `skip` / `top` 執行 Skip / Take，不得一次回傳全部資料。

#### Scenario: 第一頁請求
- **WHEN** 前端發送 `GET /api/ExampleItems?skip=0&top=10`
- **THEN** 後端 SHALL 回傳第 1–10 筆資料，`total` 為資料總筆數，HTTP 200

#### Scenario: 翻至第二頁
- **WHEN** 前端發送 `GET /api/ExampleItems?skip=10&top=10`
- **THEN** 後端 SHALL 回傳第 11–20 筆資料，`total` 不變

#### Scenario: skip 超出範圍
- **WHEN** `skip` 大於等於 `total`
- **THEN** 後端 SHALL 回傳空 `results` 陣列，`total` 為實際總筆數，HTTP 200

#### Scenario: 省略分頁參數
- **WHEN** 未傳 `skip` 或 `top`
- **THEN** 後端 SHALL 使用預設值（skip=0, top=10）回傳第一頁

---

### Requirement: ExampleItems 列表支援單欄 server-side 排序
`GET /api/ExampleItems` SHALL 接受 `sortField`（string，預設 `"id"`）與 `sortOrder`（string，`"asc"` 或 `"desc"`，預設 `"asc"`）query parameters，並依此排序後再執行分頁。

#### Scenario: 依名稱升冪排序
- **WHEN** 前端發送 `?sortField=name&sortOrder=asc`
- **THEN** 後端 SHALL 回傳依 `Name` 升冪排序後的當頁資料

#### Scenario: 依名稱降冪排序
- **WHEN** 前端發送 `?sortField=name&sortOrder=desc`
- **THEN** 後端 SHALL 回傳依 `Name` 降冪排序後的當頁資料

#### Scenario: 不合法的 sortField
- **WHEN** `sortField` 為未定義欄位名稱
- **THEN** 後端 SHALL 回退至依 `id` 升冪排序，不拋例外

#### Scenario: 不合法的 sortOrder
- **WHEN** `sortOrder` 非 `"asc"` 或 `"desc"`
- **THEN** 後端 SHALL 視為 `"asc"`，不拋例外

---

### Requirement: 前端 DataTable 使用 PrimeVue lazy 模式
`ExampleItemsView` SHALL 以 PrimeVue DataTable 的 lazy 模式呈現列表，翻頁與欄標題點擊排序時 SHALL 觸發 API 呼叫並更新資料，不得在前端對全量資料做排序或分頁。

#### Scenario: 首次載入
- **WHEN** 元件掛載（onMounted）
- **THEN** SHALL 以 skip=0, top=10, sortField="id", sortOrder="asc" 發送第一次請求，DataTable 顯示第一頁資料

#### Scenario: 使用者翻頁
- **WHEN** 使用者點擊分頁器切換頁次
- **THEN** SHALL 依新的 `first` / `rows` 值發送請求，DataTable 更新為新頁資料

#### Scenario: 使用者點擊欄標題排序
- **WHEN** 使用者點擊可排序欄標題
- **THEN** SHALL 帶入新的 sortField / sortOrder 重新從第一頁（skip=0）發送請求

#### Scenario: 載入中狀態
- **WHEN** API 請求進行中
- **THEN** DataTable SHALL 顯示 loading 狀態，不得顯示舊資料或空白

---

### Requirement: 假資料擴充至 30 筆
`ExampleItemsService` 的 in-memory 假資料 SHALL 包含至少 30 筆，各筆 `Name` 與 `Description` 值有所差異，確保排序與分頁可視覺驗證。

#### Scenario: 分頁驗證
- **WHEN** 以 top=10 查詢
- **THEN** SHALL 至少有三頁資料（total >= 30）

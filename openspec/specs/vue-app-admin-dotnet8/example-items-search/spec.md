## ADDED Requirements

### Requirement: ItemResponse 加入類別欄位

`ItemResponse` SHALL 包含 `categoryId: int` 與 `categoryName: string`。30 筆 in-memory 資料各自指定 categoryId（1、2、3 隨意分配）。

#### Scenario: 單筆查詢包含類別資訊

- **WHEN** 呼叫 `GET /api/ExampleItems/{id}`
- **THEN** response 包含 `categoryId` 與 `categoryName` 欄位

---

### Requirement: POST /api/ExampleItems/Search

系統 SHALL 以 `POST /api/ExampleItems/Search`（需 JWT）取代原 `GET /api/ExampleItems`。Request body：

```json
{
  "page": 1,
  "pageSize": 10,
  "sortField": "name",
  "sortOrder": "asc",
  "name": "",
  "description": "",
  "categoryIds": []
}
```

所有查詢條件為選填。`name` 與 `description` 為模糊查詢（不區分大小寫，contains 語意）。`categoryIds` 為複選過濾，空陣列代表不過濾。回傳 `ApiPagedResponse<ItemResponse>`。

#### Scenario: 不帶條件查詢回傳全部分頁資料

- **WHEN** 呼叫 `POST /api/ExampleItems/Search` body 為 `{ "page": 1, "pageSize": 10 }`
- **THEN** 回傳第 1 頁 10 筆，total 為 30

#### Scenario: 名稱模糊查詢

- **WHEN** body 包含 `"name": "item 1"`
- **THEN** 僅回傳 name 包含 "item 1"（不分大小寫）的項目

#### Scenario: 說明模糊查詢

- **WHEN** body 包含 `"description": "desc"`
- **THEN** 僅回傳 description 包含 "desc" 的項目

#### Scenario: 類別複選過濾

- **WHEN** body 包含 `"categoryIds": [1, 3]`
- **THEN** 僅回傳 categoryId 為 1 或 3 的項目

#### Scenario: 複合條件查詢

- **WHEN** body 包含 name 模糊條件與 categoryIds
- **THEN** 回傳同時符合兩個條件的項目

---

### Requirement: 前端 ExampleItemsView 篩選 UI

`ExampleItemsView.vue` SHALL 在表格上方提供篩選列，包含：
- 名稱輸入框（debounce 300ms 後觸發查詢）
- 說明輸入框（debounce 300ms 後觸發查詢）
- PrimeVue MultiSelect 類別複選（選項來自 `POST /api/ExampleCategories`，變更後立即觸發查詢）

#### Scenario: 名稱輸入觸發查詢

- **WHEN** 使用者在名稱輸入框輸入文字並等待 300ms
- **THEN** 前端以最新條件重新呼叫 `POST /api/ExampleItems/Search`

#### Scenario: 類別 MultiSelect 載入選項

- **WHEN** ExampleItemsView 頁面初始化
- **THEN** 呼叫 `POST /api/ExampleCategories` 並將結果填入 MultiSelect 選項

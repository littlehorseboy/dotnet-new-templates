## MODIFIED Requirements

### Requirement: 前端 ExampleItemsView 篩選 UI

`ExampleItemsView.vue` SHALL 在表格上方提供篩選列，包含：
- 名稱輸入框（PrimeVue `InputText`，無自動查詢）
- 說明輸入框（PrimeVue `InputText`，無自動查詢）
- PrimeVue MultiSelect 類別複選（選項來自 `POST /api/ExampleCategories`，選取後不觸發查詢）
- 「查詢」按鈕，點擊後重設至第一頁並呼叫 `POST /api/ExampleItems/Search`

篩選列採四欄排版（名稱 / 說明 / 類別 / 查詢按鈕），所有控件高度一致，以 PrimeVue MultiSelect 預設高度為基準。

#### Scenario: 點擊查詢按鈕才觸發 API 請求

- **WHEN** 使用者填寫任意篩選條件後點擊「查詢」按鈕
- **THEN** 前端重設分頁至第一頁，並以最新條件呼叫 `POST /api/ExampleItems/Search`

#### Scenario: 輸入文字不自動觸發查詢

- **WHEN** 使用者在名稱或說明輸入框輸入文字
- **THEN** 前端不發出任何 API 請求，直到使用者點擊「查詢」按鈕

#### Scenario: 選擇類別不自動觸發查詢

- **WHEN** 使用者在 MultiSelect 選擇或取消選擇類別
- **THEN** 前端不發出任何 API 請求，直到使用者點擊「查詢」按鈕

#### Scenario: 在文字輸入框按 Enter 觸發查詢

- **WHEN** 使用者在名稱或說明輸入框按下 Enter 鍵
- **THEN** 前端重設分頁至第一頁，並以最新條件呼叫 `POST /api/ExampleItems/Search`

#### Scenario: 類別 MultiSelect 載入選項

- **WHEN** ExampleItemsView 頁面初始化
- **THEN** 呼叫 `POST /api/ExampleCategories` 並將結果填入 MultiSelect 選項

## ADDED Requirements

### Requirement: ApiPagedResponse\<T\> 分頁專用回傳型別
分頁端點 SHALL 使用 `ApiPagedResponse<T>` 作為回傳型別。`ApiPagedResponse<T>` 繼承 `ApiResponse<T>`，額外包含 `Total`（int）屬性，代表未分頁前的資料總筆數。非分頁端點 SHALL 繼續使用 `ApiResponse<T>`，不得混用。

#### Scenario: 分頁端點回傳結構
- **WHEN** 分頁端點操作成功
- **THEN** Controller SHALL 回傳 HTTP 200，body 包含 `success: true`、`results`（當頁資料陣列）、`total`（總筆數整數），`result` 為 null

#### Scenario: 非分頁端點不含 total
- **WHEN** 非分頁端點（如 GetById）操作成功
- **THEN** response body SHALL NOT 包含 `total` 欄位，繼續使用 `ApiResponse<T>`

---

### Requirement: ApiPagedResponse\<T\> 靜態工廠方法 OkPaged
`ApiPagedResponse<T>` SHALL 提供 `OkPaged(IEnumerable<T> results, int total)` 靜態工廠方法，簡化 Controller 撰寫。

#### Scenario: OkPaged 工廠方法
- **WHEN** 建立分頁成功 response
- **THEN** SHALL 使用 `ApiPagedResponse<T>.OkPaged(results, total)`，`Results` 有值，`Result` 為 null，`Success` 為 true，`Total` 為傳入的總筆數

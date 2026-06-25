### Requirement: PasswordHasherHelper 提供密碼雜湊工具
`PasswordHasherHelper.cs` SHALL 提供 `HashPassword(plainText)` 與 `VerifyPassword(plainText, hash)` 靜態方法，使用 BCrypt 或 ASP.NET Core 內建 `PasswordHasher<T>`。

#### Scenario: 雜湊後可驗證
- **WHEN** `HashPassword("myPassword")` 產生 hash，再以 `VerifyPassword("myPassword", hash)` 驗證
- **THEN** 回傳 `true`

---

### Requirement: ExampleItems 展示 Controller/Service 分層
`ExampleItemsController.cs` 與 `ExampleItemsService.cs` SHALL 作為示範用分層架構範例，使用 hardcoded dummy data（無 DB），提供 `GET /api/ExampleItems`（列表）與 `GET /api/ExampleItems/{id}`（單筆）。

#### Scenario: 取得 ExampleItems 列表
- **WHEN** 呼叫 `GET /api/ExampleItems`（已登入）
- **THEN** 回應 HTTP 200，body 為 `ApiResponse<ItemResponse>` 集合（至少 3 筆 hardcoded 資料）

#### Scenario: 取得單筆 ExampleItem
- **WHEN** 呼叫 `GET /api/ExampleItems/{id}`（已登入）
- **THEN** 回應 HTTP 200，body 為 `ApiResponse<ItemResponse>` 單筆物件

#### Scenario: 不存在的 id
- **WHEN** 呼叫 `GET /api/ExampleItems/999`（已登入）
- **THEN** 回應 HTTP 404，body 為 `ApiResponse<object>.Fail("找不到指定項目")`

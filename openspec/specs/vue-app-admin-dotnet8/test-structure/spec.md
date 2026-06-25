## ADDED Requirements

### Requirement: 獨立測試專案
Solution SHALL 包含獨立的測試專案 `VueAppAdmin.Server.Tests`，使用 xUnit 框架與 NSubstitute mock 套件。

#### Scenario: 測試專案建立
- **WHEN** 初始化 template
- **THEN** solution 中 SHALL 包含 `VueAppAdmin.Server.Tests` 專案，並加入 `.sln` 中

#### Scenario: NuGet 套件
- **WHEN** 設定測試專案相依
- **THEN** `VueAppAdmin.Server.Tests.csproj` SHALL 包含 `xunit`、`xunit.runner.visualstudio`、`Microsoft.NET.Test.Sdk`、`NSubstitute` 參考

---

### Requirement: 測試專案結構 Mirror Source
測試專案的資料夾結構 SHALL 完全對應 source 專案的 `Features/` 結構，每個 Feature 對應一個測試子資料夾。

#### Scenario: Feature 測試資料夾
- **WHEN** source 中存在 `Features/Auth/` 與 `Features/ExampleItems/`
- **THEN** 測試專案 SHALL 對應建立 `Features/Auth/` 與 `Features/ExampleItems/` 資料夾

#### Scenario: 測試檔案命名
- **WHEN** 為 `AuthService` 撰寫測試
- **THEN** 測試檔案 SHALL 命名為 `AuthServiceTests.cs`，放在 `Features/Auth/` 下

---

### Requirement: Unit Test 結構規範
每個 Unit Test 方法 SHALL 使用 Arrange / Act / Assert 三段結構，方法命名使用 `方法名稱_情境_預期結果` 格式。

#### Scenario: 測試方法命名
- **WHEN** 為 `ValidateCredentials` 撰寫正向情境測試
- **THEN** 方法名稱 SHALL 為 `ValidateCredentials_ValidCredentials_ReturnsTrue` 形式

#### Scenario: Mock 建立方式
- **WHEN** 需要 mock 一個 Interface（如 `IUserRepository`）
- **THEN** SHALL 使用 `Substitute.For<IUserRepository>()`，不得使用 Moq 或手動實作 fake class

---

### Requirement: Service Unit Test 隔離 Repository
Service 的 Unit Test SHALL mock 所有 Repository 相依，不得連接真實資料庫或 in-memory 資料結構。

#### Scenario: AuthService 測試隔離
- **WHEN** 測試 `AuthService` 的業務邏輯
- **THEN** `IUserRepository` SHALL 以 NSubstitute mock 替換，測試結果不依賴外部狀態

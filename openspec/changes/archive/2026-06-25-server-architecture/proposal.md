## Why

`VueAppAdmin.Server` 目前採用 by-layer 資料夾結構，加上命名不一致（DTO 資料夾、匿名 Response 物件、缺少 Interface）和 Logging 設定不完整，導致新增功能時需要跨多個資料夾操作，且 Service 無法被 mock，不利於後續 TDD 實踐。現在趁 template 還小，一次整頓結構與規範。

## What Changes

- 將 `Controllers/`、`Services/`、`DTO/` 合併重組為 `Features/<feature-name>/` by-feature 結構
- 每個 Feature 資料夾內分 `Requests/`、`Responses/` 子資料夾
- 每個 Feature 建立自己的 `Add<Feature>Extensions.cs` 負責 DI 註冊，取代集中式 `ServiceRegistrationExtensions`
- `IServiceCollectionExtensions/` 資料夾重命名並移至 `Shared/`，改為主題式子資料夾（`Jwt/`、`Logging/`、`Middleware/`）
- 所有 Service 加上對應 Interface（`IAuthService`、`IExampleItemsService`、`IJwtService`）
- 補上 `MeResponse.cs`，取代 `AuthController.Me()` 目前回傳的匿名物件
- Serilog 移除 JSON sink，保留 txt rolling log，保存天數改為 365 天
- 加入 `UseSerilogRequestLogging()` 記錄每次 HTTP 請求
- Service 層改用 `ILogger<T>` 注入，不再使用靜態 `Log.*`
- 新增 `ExceptionHandlingMiddleware` 集中捕捉 unhandled exception 並 log
- 新增 `ApiResponse<T>` 統一回傳 wrapper，搭配 HTTP 狀態碼使用，提供靜態工廠方法 `Ok()` / `Fail()`
- DataAnnotations 驗證失敗客製化 400 格式配合 `ApiResponse<T>`，並主動 log 驗證錯誤詳情（含狀態碼）
- 加入 Dapper 作為資料存取層，每個 Feature 建立對應 Repository（Interface + 實作），SQL 集中在 Repository
- 新增 `Shared/Database/` 管理 Dapper 連線設定
- 新增測試專案 `VueAppAdmin.Server.Tests`，結構 mirror `Features/`，使用 xUnit + NSubstitute

## Capabilities

### New Capabilities

- `feature-folder-structure`：by-feature 資料夾組織規範，包含 Features/、Shared/ 的分層方式與 Feature 自我註冊模式
- `logging`：HTTP request logging、Service 層結構化 logging、全域 exception 捕捉與 log 的完整配置
- `response-contracts`：API Response DTO 一致性規範，`ApiResponse<T>` wrapper 搭配正確 HTTP 狀態碼，所有端點回傳具名型別
- `data-access`：Dapper + Repository 模式規範，SQL 集中在 Repository，Service 不直接接觸 IDbConnection
- `test-structure`：xUnit + NSubstitute 測試專案結構，mirror source 的 Features/ 組織方式

### Modified Capabilities

（無）

## Impact

- `VueAppAdmin.Server` 所有現有檔案需搬移或重命名
- `Program.cs` 需更新 middleware pipeline 與 service 註冊呼叫
- Namespace 全面更新（`VueAppAdmin.Server.Features.Auth` 等）
- 不影響 API 路由、JWT 驗證邏輯、前端呼叫方式
- 新增 `VueAppAdmin.Server.Tests` 專案至 solution
- 新增 NuGet 套件：Dapper、NSubstitute

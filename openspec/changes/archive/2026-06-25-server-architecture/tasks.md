## 1. 基礎建設：Shared 層

- [x] 1.1 建立 `Shared/Logging/SerilogHelper.cs`：移除 JSON sink，txt 保留 365 天
- [x] 1.2 建立 `Shared/Jwt/JwtOptions.cs`：從原 `DTO/JwtOptions.cs` 搬移，更新 namespace
- [x] 1.3 建立 `Shared/Jwt/IJwtService.cs` 與 `Shared/Jwt/JwtService.cs`：從原 `Services/JwtService.cs` 搬移，Service 改用 `ILogger<JwtService>` 注入
- [x] 1.4 建立 `Shared/Jwt/JwtExtensions.cs`：從原 `IServiceCollectionExtensions/JwtServiceCollectionExtensions.cs` 搬移
- [x] 1.5 建立 `Shared/Middleware/ExceptionHandlingMiddleware.cs`：捕捉 unhandled exception，`LogError` 後回傳 HTTP 500 + `ApiResponse<object>.Fail("系統錯誤")`
- [x] 1.6 建立 `Shared/Database/DatabaseExtensions.cs`：以 Scoped 方式註冊 `IDbConnection`，connection string 來自 `ConnectionStrings:Default`
- [x] 1.7 刪除舊有 `IServiceCollectionExtensions/` 資料夾與 `ServiceRegistrationExtensions.cs`

## 2. ApiResponse\<T\>

- [x] 2.1 建立 `Shared/ApiResponse.cs`：包含 `Success`、`Message`、`Data` 屬性，實作 `Ok(data)` / `Ok(data, message)` / `Fail(message)` 靜態工廠方法

## 3. Auth Feature 重構

- [x] 3.1 建立 `Features/Auth/Requests/LoginRequest.cs`：從原 `DTO/Request/Auth/LoginRequest.cs` 搬移，加上 `[Required]`、`[StringLength]` DataAnnotations
- [x] 3.2 建立 `Features/Auth/Responses/LoginResponse.cs`：從原 `DTO/Response/Auth/LoginResponse.cs` 搬移
- [x] 3.3 建立 `Features/Auth/Responses/MeResponse.cs`：新增，包含 `Username`、`DisplayName` 屬性
- [x] 3.4 建立 `Features/Auth/IAuthService.cs` 定義 Interface
- [x] 3.5 建立 `Features/Auth/AuthService.cs`：從原 `Services/AuthService.cs` 搬移，實作 `IAuthService`，注入 `ILogger<AuthService>`
- [x] 3.6 建立 `Features/Auth/AuthController.cs`：從原 `Controllers/AuthController.cs` 搬移，`Login` 改回傳 `ApiResponse<LoginResponse>`，`Me` 改回傳 `ApiResponse<MeResponse>`
- [x] 3.7 建立 `Features/Auth/Helpers/PasswordHasherHelper.cs`：從原 `Helpers/PasswordHasherHelper.cs` 搬移（或保留在 Shared/ 視使用情境）
- [x] 3.8 建立 `Features/Auth/AuthExtensions.cs`：`AddAuthFeature()` 方法，註冊 `IAuthService`

## 4. ExampleItems Feature 重構

- [x] 4.1 建立 `Features/ExampleItems/Responses/ItemResponse.cs`：從原 `DTO/Response/ExampleItems/ExampleItemResponse.cs` 搬移
- [x] 4.2 建立 `Features/ExampleItems/IExampleItemsService.cs` 定義 Interface
- [x] 4.3 建立 `Features/ExampleItems/ExampleItemsService.cs`：從原 `Services/ExampleItems/ExampleItemsService.cs` 搬移，實作 `IExampleItemsService`
- [x] 4.4 建立 `Features/ExampleItems/ExampleItemsController.cs`：從原 `Controllers/ExampleItems/ExampleItemsController.cs` 搬移，回傳改為 `ApiResponse<T>`
- [x] 4.5 建立 `Features/ExampleItems/ExampleItemsExtensions.cs`：`AddExampleItemsFeature()` 方法，註冊 `IExampleItemsService`

## 5. Dapper Repository 骨架

- [x] 5.1 加入 `Dapper` NuGet 套件至 `VueAppAdmin.Server.csproj`
- [x] 5.2 新增 `ConnectionStrings:Default` 至 `appsettings.json` 與 `appsettings.Development.json`（placeholder 值）
- [x] 5.3 建立 `Features/Auth/IUserRepository.cs`：定義基本 user 查詢 Interface（如 `FindByUsername`）
- [x] 5.4 建立 `Features/Auth/UserRepository.cs`：實作 `IUserRepository`，注入 `IDbConnection`，SQL 集中於此（template 階段可為空實作）
- [x] 5.5 更新 `Features/Auth/AuthExtensions.cs`：同時註冊 `IUserRepository → UserRepository`（Scoped）

## 6. Program.cs 更新

- [x] 6.1 更新 `Program.cs`：`app.UseMiddleware<ExceptionHandlingMiddleware>()` 放在最外層
- [x] 6.2 更新 `Program.cs`：加入 `app.UseSerilogRequestLogging()`
- [x] 6.3 更新 `Program.cs`：`AddControllers()` 加入 `ConfigureApiBehaviorOptions`，客製化 400 格式為 `ApiResponse<object>.Fail()`，並 `LogWarning` 驗證錯誤詳情（含 `{StatusCode}`）
- [x] 6.4 更新 `Program.cs`：Service 註冊改為 `builder.Services.AddAuthFeature()` / `AddExampleItemsFeature()` / `AddJwtAuthentication()` / `AddDatabase()`
- [x] 6.5 刪除舊有 `Controllers/`、`Services/`、`DTO/`、`Helpers/` 資料夾（確認全部搬移完畢後）

## 7. 測試專案建立

- [x] 7.1 建立 `VueAppAdmin.Server.Tests` xUnit 測試專案
- [x] 7.2 加入至 solution：`dotnet sln add VueAppAdmin.Server.Tests`
- [x] 7.3 加入 NuGet 套件：`NSubstitute`、`xunit`、`xunit.runner.visualstudio`、`Microsoft.NET.Test.Sdk`
- [x] 7.4 加入 `VueAppAdmin.Server` 專案參考至測試專案
- [x] 7.5 建立 `Features/Auth/AuthServiceTests.cs`：撰寫 `ValidateCredentials` 的基本 Unit Test（正向與負向情境），使用 NSubstitute mock `ILogger`
- [x] 7.6 確認 `dotnet test` 可正常執行並通過

## 8. 驗收確認

- [x] 8.1 啟動應用程式，確認 Swagger UI 正常顯示所有端點
- [x] 8.2 測試 `POST /api/auth/login`：成功回傳 `ApiResponse<LoginResponse>`，失敗回傳 401 + `ApiResponse`
- [x] 8.3 測試 `GET /api/auth/me`：回傳 `ApiResponse<MeResponse>`（不再是匿名物件）
- [x] 8.4 測試驗證失敗（空 username）：回傳 400 + `ApiResponse<object>.Fail`，並確認 log 有記錄驗證錯誤
- [x] 8.5 確認 `logs/log-*.txt` 有每次 HTTP request 記錄
- [x] 8.6 確認觸發未捕捉 exception 時回傳 500 + 通用訊息，exception detail 不洩漏

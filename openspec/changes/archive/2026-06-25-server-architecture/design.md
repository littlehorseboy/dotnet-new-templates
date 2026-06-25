## Context

`VueAppAdmin.Server` 為 .NET 8 WebAPI + Vue 3 SPA 的 dotnet new template。目前 source 採 by-layer 結構（Controllers/、Services/、DTO/），Service 沒有 Interface、Response 不一致、Logging 不完整、沒有資料存取層、沒有測試專案。本次整頓為後續所有在此 template 上開發的專案建立一致的結構基礎。

## Goals / Non-Goals

**Goals:**
- 完整重構為 by-feature 資料夾結構
- 建立 `ApiResponse<T>` 統一回傳規範
- 完整 Logging 配置（request log、驗證 log、exception log）
- 引入 Dapper + Repository 模式作為資料存取規範
- 建立 `VueAppAdmin.Server.Tests` 測試專案基礎結構

**Non-Goals:**
- 實作真實資料庫連線（DB connection string 留 placeholder）
- 實作完整的 CRUD 功能
- 調整 Vue 前端任何部分
- 引入 API versioning

## Decisions

### 1. 資料夾結構：by-feature

每個 Feature 自成一個資料夾，內含 Controller、Service（含 Interface）、Repository（含 Interface）、Requests/、Responses/，以及自己的 DI 註冊擴充方法。

```
Features/
├── Auth/
│   ├── Requests/
│   │   └── LoginRequest.cs
│   ├── Responses/
│   │   ├── LoginResponse.cs
│   │   └── MeResponse.cs
│   ├── AuthController.cs
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   └── AuthExtensions.cs          ← builder.Services.AddAuthFeature()
└── ExampleItems/
    ├── Responses/
    │   └── ItemResponse.cs
    ├── ExampleItemsController.cs
    ├── IExampleItemsService.cs
    ├── ExampleItemsService.cs
    └── ExampleItemsExtensions.cs
```

共用跨 Feature 的基礎設施放 `Shared/`，以主題為子資料夾單位：

```
Shared/
├── Database/
│   └── DatabaseExtensions.cs      ← IDbConnection 註冊
├── Jwt/
│   ├── JwtOptions.cs
│   ├── IJwtService.cs
│   ├── JwtService.cs
│   └── JwtExtensions.cs
├── Logging/
│   └── SerilogHelper.cs
└── Middleware/
    └── ExceptionHandlingMiddleware.cs
```

**為何選 by-feature 而非 by-layer：** 新增或修改一個功能只需進入單一資料夾，不需跨三層資料夾操作。測試專案可完全 mirror 此結構。

### 2. ApiResponse\<T\> 回傳規範

所有 Controller 端點統一使用 `ApiResponse<T>`，搭配正確 HTTP 狀態碼回傳，不使用匿名物件。

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };
}
```

HTTP 狀態碼不重複放入 body，避免不一致。Log 時狀態碼由呼叫端明確帶入 log 參數。

**為何不用 always-200：** 破壞 REST 語意，Swagger 文件混亂，axios interceptor 無法靠 HTTP status 做統一處理。

### 3. DataAnnotations 驗證 + 客製化 400

使用 .NET 內建 DataAnnotations，不引入 FluentValidation。`[ApiController]` 自動攔截驗證失敗，透過 `ConfigureApiBehaviorOptions` 客製化 400 格式：

```csharp
options.InvalidModelStateResponseFactory = context =>
{
    var logger = context.HttpContext.RequestServices
        .GetRequiredService<ILogger<Program>>();

    var errors = context.ModelState
        .Where(x => x.Value?.Errors.Count > 0)
        .ToDictionary(
            x => x.Key,
            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

    logger.LogWarning("Validation failed {Path} {StatusCode} {@ValidationErrors}",
        context.HttpContext.Request.Path, 400, errors);

    return new BadRequestObjectResult(ApiResponse<object>.Fail("驗證失敗"));
};
```

### 4. Logging 配置

```
SerilogHelper.Initialize()：
  - Console sink
  - 每日 rolling txt，保留 365 天
  - 移除 JSON sink

Program.cs pipeline：
  - app.UseMiddleware<ExceptionHandlingMiddleware>()  ← 最外層
  - app.UseSerilogRequestLogging()                   ← HTTP request log

Service 層：
  - 注入 ILogger<T>，不使用靜態 Log.*
```

`ExceptionHandlingMiddleware` 捕捉所有 unhandled exception，log `LogError` 後回傳 500 + `ApiResponse<object>.Fail("系統錯誤")`，不洩漏 exception detail 給前端。

### 5. Dapper + Repository 模式

引入 Dapper 作為資料存取層。每個 Feature 擁有自己的 Repository Interface 與實作，SQL 語法集中在 Repository，Service 不直接接觸 `IDbConnection`。

```
Features/Auth/
  IUserRepository.cs    ← 定義資料存取合約
  UserRepository.cs     ← Dapper SQL 實作

Shared/Database/
  DatabaseExtensions.cs ← 註冊 IDbConnection (Scoped)，connection string 來自 appsettings
```

**為何 Dapper 搭配 Repository 而非直接在 Service 用：** Dapper 是 raw SQL，若不抽象化，Service 會混入 SQL 語法。Repository 讓 Service 專注業務邏輯，也讓測試可替換 fake repository。

**EF Core 的差異：** EF Core 本身已具備 Repository/UoW 概念，額外包一層意義不大；Dapper 無此機制，Repository 是必要的。

### 6. 測試專案結構

新增 `VueAppAdmin.Server.Tests` 專案（xUnit + NSubstitute），結構完全 mirror source 的 Features/：

```
VueAppAdmin.Server.Tests/
└── Features/
    ├── Auth/
    │   ├── AuthServiceTests.cs
    │   └── AuthControllerTests.cs
    └── ExampleItems/
        └── ExampleItemsServiceTests.cs
```

Unit Test 以 Arrange / Act / Assert 三段結構撰寫，使用 `NSubstitute.Substitute.For<T>()` 建立 mock。

## Risks / Trade-offs

- **Dapper 無 migration 支援** → DB schema 異動需手動管理，template 階段用 placeholder 連線，不是實際風險
- **by-feature 的 Shared/ 邊界模糊** → 遵守規則：只有被兩個以上 Feature 使用的元件才移入 Shared/，否則留在 Feature 內
- **ApiResponse\<T\> 與 Swagger 的相容性** → Swagger 能正確產生 `ApiResponse<LoginResponse>` 型別，需確認泛型顯示正常

## Migration Plan

1. 新增 `VueAppAdmin.Server.Tests` 專案並加入 solution
2. 搬移現有檔案至新資料夾結構，更新所有 namespace
3. 新增 `ApiResponse<T>`、`ExceptionHandlingMiddleware`，更新 `SerilogHelper`
4. 更新 `Program.cs` middleware pipeline 與 service 註冊
5. 補上 `MeResponse.cs`、所有 Interface、`AuthFeatureExtensions.cs` 等
6. 加入 Dapper NuGet，建立 `Shared/Database/` 與各 Feature 的 Repository 骨架
7. 確認 Swagger UI、JWT 驗證、既有 API 路由功能正常

## Open Questions

- Dapper 的 `IDbConnection` 要用 `Scoped` 還是 `Transient`？（建議 Scoped，一個 request 共用一條連線）
- `ExampleItemsService` 目前是 in-memory，Repository 骨架先建 interface，實作保留 in-memory 作為示範？

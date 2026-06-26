# VueAppAdmin.Server

ASP.NET Core 8 WebAPI，採用 by-feature 資料夾結構。

---

## 專案結構

```
VueAppAdmin.Server/
├── Features/
│   ├── Auth/
│   │   ├── Helpers/          # PasswordHasherHelper
│   │   ├── Requests/         # LoginRequest（DataAnnotations 驗證）
│   │   ├── Responses/        # LoginResponse、MeResponse
│   │   ├── AuthController.cs
│   │   ├── AuthExtensions.cs # AddAuthFeature() — DI 自我註冊
│   │   ├── AuthService.cs    # 實作 IAuthService
│   │   ├── IAuthService.cs
│   │   ├── IUserRepository.cs
│   │   └── UserRepository.cs # Dapper，SQL 集中於此
│   ├── ExampleItems/         # 示範用 Feature（in-memory，無 DB）
│   │   ├── Requests/
│   │   ├── Responses/
│   │   ├── ExampleItemsController.cs
│   │   ├── ExampleItemsExtensions.cs
│   │   ├── ExampleItemsService.cs
│   │   └── IExampleItemsService.cs
│   ├── ExampleCategories/    # 示範用類別 Feature（in-memory）
│   ├── FeatureList/          # 功能識別字清單 Feature
│   └── Menu/                 # 選單（依使用者 features 過濾）
├── Shared/
│   ├── ApiResponse.cs        # 統一回傳型別 ApiResponse<T>
│   ├── Database/             # IDbConnection Scoped 註冊
│   ├── Jwt/                  # JWT 設定、IJwtService、JwtExtensions
│   ├── Logging/              # SerilogHelper
│   └── Middleware/           # ExceptionHandlingMiddleware
└── Program.cs
```

### 規則
- 每個業務功能放在 `Features/<FeatureName>/`，包含該功能所有相關檔案
- 跨 Feature 共用的基礎設施放在 `Shared/<TopicName>/`
- 每個 Feature 提供 `Add<Name>Feature()` 擴充方法，在 `Program.cs` 呼叫

---

## 設定

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=VueAppAdmin;..."
  },
  "Jwt": {
    "Issuer": "VueAppAdmin",
    "SignKey": "REPLACE_WITH_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS",
    "TokenExpirationHours": 8
  },
  "Logging": {
    "LogLevel": { "Default": "Information" },
    "RetentionDays": 365
  }
}
```

> **重要**：上線前必須替換 `Jwt:SignKey` 為至少 32 字元的強密鑰。

---

## 啟動

```bash
# 在專案根目錄（含 .sln）時，需指定專案路徑
dotnet run --project .\VueAppAdmin.Server\ --launch-profile https

# 已在 VueAppAdmin.Server/ 目錄內時可直接執行
dotnet run --launch-profile https
```

Swagger UI：`https://localhost:7173/swagger`（Development 環境）

---

## API 端點

所有端點需 JWT Bearer Token，除了 `POST /api/auth/login`。

| Method | 路徑 | 說明 |
|--------|------|------|
| POST | `/api/Auth/Login` | 登入，回傳 Token |
| GET | `/api/Auth/Me` | 取得目前登入者資訊（username、groups、features） |
| POST | `/api/ExampleItems/Search` | 分頁搜尋範例清單（含篩選、排序） |
| GET | `/api/ExampleItems/{id}` | 取得單筆範例 |
| POST | `/api/ExampleCategories` | 取得所有類別清單 |
| GET | `/api/Features` | 取得系統所有功能識別字清單 |
| POST | `/api/Menu/Items` | 取得依使用者功能過濾後的選單樹 |

### 回傳格式 `ApiResponse<T>`

```json
// 成功（單筆）
{ "success": true, "message": null, "result": { ... }, "results": null }

// 成功（集合）
{ "success": true, "message": null, "result": null, "results": [ ... ] }

// 失敗
{ "success": false, "message": "錯誤說明", "result": null, "results": null }
```

---

## Logging

| 檔案 | 內容 |
|------|------|
| `logs/log-system-<date>.txt` | 所有 log，含 framework（DI `ILogger<T>`） |
| `logs/log-<date>.txt` | 靜態 `Log.*`，用於啟動與崩潰記錄 |

### 為特定服務建立獨立 log 資料夾

```csharp
// 寫入 logs/SmsService/SmsService-<date>.txt
private static readonly Serilog.ILogger _logger = SerilogHelper.GetLogger<SmsService>();

_logger.Information("Sending SMS to {Phone}", phone);
```

適合場景：排程任務、外部整合（SMS/Email/Push）、稽核 log。
一般 Service / Controller 請使用 DI 注入的 `ILogger<T>`。

---

## 新增 Feature

以新增 `Products` Feature 為例：

**1. 建立資料夾結構**
```
Features/Products/
├── Requests/     CreateProductRequest.cs
├── Responses/    ProductResponse.cs
├── IProductsService.cs
├── ProductsService.cs
├── IProductRepository.cs
├── ProductRepository.cs
├── ProductsController.cs
└── ProductsExtensions.cs
```

**2. `ProductsExtensions.cs`**
```csharp
public static class ProductsExtensions
{
    public static IServiceCollection AddProductsFeature(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}
```

**3. `Program.cs` 加入一行**
```csharp
builder.Services.AddProductsFeature();
```

---

## 測試

```bash
dotnet test
```

測試專案：`VueAppAdmin.Server.Tests`（xUnit + NSubstitute）

測試結構對應 `Features/` 資料夾，Service 的 Repository 相依以 NSubstitute mock 替換。

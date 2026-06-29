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
│   ├── Logging/              # SerilogHelper、ApiLogFilter、LogMaskAttribute
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
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning"
      }
    }
  },
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=VueAppAdmin;..."
  },
  "Jwt": {
    "Issuer": "VueAppAdmin",
    "SignKey": "REPLACE_WITH_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS",
    "TokenExpirationHours": 8
  },
  "Logging": {
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
| `logs/log-system-<date>.txt` | 系統層 log，含 framework 訊息（DI `ILogger<T>`） |
| `logs/log-<date>.txt` | 靜態 `Log.*`，用於啟動與崩潰記錄 |
| `logs/ApiLogFilter/ApiLogFilter-<date>.txt` | API 請求 / 回應記錄（獨立檔案） |

### Log Level 設定

Log level 透過 `appsettings.json` 的 `Serilog:MinimumLevel` 控制：

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",   // 全域預設等級
    "Override": {
      "Microsoft.AspNetCore": "Warning"  // 指定 namespace 覆寫等級
    }
  }
}
```

`appsettings.Development.json` 可將 `Default` 設為 `Debug`，開發時取得更細的 log 輸出。

可用等級由低到高：`Verbose` → `Debug` → `Information` → `Warning` → `Error` → `Fatal`

> **注意**：應用程式啟動初期（DI 容器建立之前）使用的 bootstrap logger（`SerilogHelper.Initialize()`）固定為 `Information` 等級，不受 appsettings 設定影響。這是 Serilog 兩階段初始化的已知限制。

### API 全域 Log（ApiLogFilter）

所有 API 端點自動記錄，涵蓋三個案例：

| 案例 | Level | 格式 |
|------|-------|------|
| 正常流程（2xx、4xx 業務錯誤） | INF | `[API] {Method} {Path} \| user:{User} \| {StatusCode} \| req:{...} \| res:{...} \| {N}ms` |
| 401 授權短路 | WRN | 同上，`req` 與 `res` 為 `null` |
| 400 驗證失敗 | WRN | 同上，`res` 顯示驗證錯誤欄位 |

**敏感欄位遮罩**：在 Request 類別的屬性加上 `[LogMask]`，該欄位在 log 中顯示為 `***`。

```csharp
[LogMask]
public string Password { get; set; }
```

**分頁回應精簡**：`ApiPagedResponse<T>` 的 `res` 只記錄 `{ success, total, count }`，不含完整 items 清單。

> TODO：未來 `ApiLogs` 存表功能實作後，可在 `ApiResponse<T>.ToLogSummary()` 切換為精簡格式，完整 payload 改由資料庫保存。

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

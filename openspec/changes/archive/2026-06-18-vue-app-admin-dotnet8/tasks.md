## 1. 建立專案目錄骨架

- [x] 1.1 以 `dotnet new vue-app-demo -n VueAppAdmin` scaffold 出基礎方案，放在 `vue-app-admin-dotnet8/` 目錄下
- [x] 1.2 確認前端目錄名稱為 `vueappadmin.client`，後端目錄為 `VueAppAdmin.Server`，方案檔為 `VueAppAdmin.sln`
- [x] 1.3 刪除 scaffold 產出中不需要的預設檔案（`WeatherForecast.cs`、`WeatherForecastController.cs`、`HelloWorld.vue`、`TheWelcome.vue`、`WelcomeItem.vue`、icons 目錄）

## 2. 後端：套件與設定

- [x] 2.1 在 `VueAppAdmin.Server.csproj` 加入 NuGet 套件：`Microsoft.AspNetCore.Authentication.JwtBearer`、`Microsoft.AspNetCore.Mvc.NewtonsoftJson`、`Serilog.AspNetCore`
- [x] 2.2 在 `appsettings.json` 加入 `Jwt` section：`Issuer`、`SignKey`（placeholder 值）
- [x] 2.3 在 `appsettings.Development.json` 加入開發用 `Jwt` 設定

## 3. 後端：Helpers

- [x] 3.1 建立 `Helpers/SerilogHelper.cs`：`Initialize()` 方法，輸出至 Console + rolling file（`.txt` 與 `.json`）
- [x] 3.2 建立 `Helpers/PasswordHasherHelper.cs`：`HashPassword()` 與 `VerifyPassword()` 靜態方法（使用 `IPasswordHasher<object>`）

## 4. 後端：JWT 認證層

- [x] 4.1 建立 `DTO/JwtOptions.cs`：`SectionName`、`Issuer`、`SignKey` 欄位，加上 `[Required]` 標注
- [x] 4.2 建立 `IServiceCollectionExtensions/JwtServiceCollectionExtensions.cs`：`AddJwtAuthentication(IConfiguration)` 擴充方法
- [x] 4.3 建立 `Services/JwtService.cs`：注入 `IOptions<JwtOptions>`，提供 `GenerateToken(username, displayName)` 方法
- [x] 4.4 建立 `DTO/Request/Auth/LoginRequest.cs`
- [x] 4.5 建立 `DTO/Response/Auth/LoginResponse.cs`（含 `Token` 欄位）
- [x] 4.6 建立 `Services/AuthService.cs`：`ValidateCredentials()` 使用 hardcoded `admin`/`password`，`GetUserInfo()` 回傳 displayName
- [x] 4.7 建立 `Controllers/AuthController.cs`：`POST /api/Auth/Login`、`GET /api/Auth/Me`（`[Authorize]`）

## 5. 後端：服務註冊與 ExampleItems

- [x] 5.1 建立 `IServiceCollectionExtensions/ServiceRegistrationExtensions.cs`：`AddCustomServices()` 擴充方法，註冊 `AuthService`、`JwtService`、`ExampleItemsService`
- [x] 5.2 建立 `DTO/Response/ExampleItems/ExampleItemResponse.cs`
- [x] 5.3 建立 `Services/ExampleItems/ExampleItemsService.cs`：`GetAll()` 與 `GetById(id)` 方法，hardcoded 3 筆 dummy data
- [x] 5.4 建立 `Controllers/ExampleItems/ExampleItemsController.cs`：`GET /api/ExampleItems`、`GET /api/ExampleItems/{id}`（均加 `[Authorize]`）

## 6. 後端：Program.cs 重構

- [x] 6.1 重寫 `Program.cs`：最頂端呼叫 `SerilogHelper.Initialize()`，加入 `AddJwtAuthentication`、`AddCustomServices`、`AddNewtonsoftJson`，Controller 預設加 `[Authorize]` filter
- [x] 6.2 確認 Swagger 設定含 JWT Bearer security definition（`bearerAuth`）
- [x] 6.3 確認 `app.UseAuthentication()` 在 `app.UseAuthorization()` 之前

## 7. 前端：套件與基礎設定

- [x] 7.1 在 `package.json` 加入所有 dependencies：`pinia`、`vue-router`、`axios`、`primevue`、`@primevue/themes`、`primeicons`、`bootstrap`、`bootstrap-icons`、`@fortawesome/*`（4 個 icon 套件 + `vue-fontawesome`）、`vee-validate`、`@vee-validate/yup`、`yup`、`date-fns`、`lodash-es`、`uuid`
- [x] 7.2 在 `package.json` devDependencies 加入 `dotenv`、`@types/lodash-es`、`@types/bootstrap`
- [x] 7.3 建立 `.env.example`，包含 `VITE_DEFINECONFIG_BASE` 說明；建立 `.env`，`VITE_DEFINECONFIG_BASE=/`
- [x] 7.4 重寫 `vite.config.ts`：`dotenv.config()` 在最頂端，HTTPS dev cert 自動產生，`/api` proxy，`@` alias，`base` 依 `VITE_DEFINECONFIG_BASE`
- [x] 7.5 執行 `npm install`，確認無 peer dependency 錯誤

## 8. 前端：Pinia Stores

- [x] 8.1 建立 `src/stores/auth-store.ts`：state `isAuthenticated`，actions `init()`/`login(token)`/`logout()`，`init()` 設定 axios header
- [x] 8.2 建立 `src/stores/user-info-store.ts`：state `username`/`displayName`，action `fetchUserInfo()`（呼叫 `GET /api/Auth/Me`）

## 9. 前端：Router

- [x] 9.1 建立 `src/router/index.ts`：定義靜態路由（`/login`、`/`→MainLayout、`/dashboard`、`/example-items`），`/login` 設 `meta.noAuthRequired: true`
- [x] 9.2 在 `router/index.ts` 的 `beforeEach` 守衛實作：未登入導向 `/login`；已登入存取 `/login` 導向 `/dashboard`
- [x] 9.3 確認 `/dashboard` 與 `/example-items` 路由設定 `meta.showInSidebar: true` 與 `meta.sidebarLabel`

## 10. 前端：Login 頁面

- [x] 10.1 建立 `src/views/LoginView.vue`：使用 Vee-Validate + Yup 的帳密表單，欄位必填驗證
- [x] 10.2 LoginView 送出後呼叫 `POST /api/Auth/Login`，成功呼叫 `auth-store.login(token)` 後導向 `/dashboard`，失敗顯示錯誤訊息

## 11. 前端：MainLayout 與骨架元件

- [x] 11.1 建立 `src/views/layouts/MainLayout.vue`：含 MainHeader、MainSidebar slot 與 `<RouterView>`
- [x] 11.2 建立 `src/components/MainLayout/MainHeader.vue`：顯示 app 名稱、`user-info-store.displayName`、登出按鈕
- [x] 11.3 建立 `src/components/MainLayout/MainSidebar.vue`：從 `router.getRoutes()` 過濾 `meta.showInSidebar === true` 動態產生選單連結
- [x] 11.4 建立 `src/views/DashboardView.vue`：placeholder，顯示歡迎文字與 `displayName`
- [x] 11.5 建立 `src/views/ExampleItemsView.vue`：呼叫 `GET /api/ExampleItems`，以 PrimeVue DataTable 顯示列表，含 loading 狀態

## 12. 前端：main.ts 整合

- [x] 12.1 重寫 `src/main.ts`：`createApp` → `use(PrimeVue)` → `use(createPinia())` → `use(router)` → `mount`，並在 mount 前呼叫 `authStore.init()`
- [x] 12.2 確認 PrimeVue 套用 Aura theme（或其他預設 preset）
- [x] 12.3 確認 Bootstrap CSS 與 FontAwesome 在 `main.ts` 或 `assets/main.css` 中 import

## 13. 建立 .template.config/template.json

- [x] 13.1 產生三組新 GUID 供範本使用
- [x] 13.2 建立 `vue-app-admin-dotnet8/.template.config/template.json`：`sourceName: "VueAppAdmin"`、`shortName: "vue-app-admin-dotnet8"`、`nameLower` derived symbol（`lowerCase` + `fileRename: "vueappadmin"`）、排除清單（`bin`、`obj`、`node_modules`、`.vs`、`.git`、`*.user`）、`guids` 含新 GUID
- [x] 13.3 確認 `preferNameDirectory: true`

## 14. 安裝與驗證

- [x] 14.1 執行 `dotnet new install .\vue-app-admin-dotnet8`，確認安裝成功
- [x] 14.2 執行 `dotnet new list`，確認 `vue-app-admin-dotnet8` 出現在清單中
- [x] 14.3 執行 `dotnet new vue-app-admin-dotnet8 -n TestAdmin`，確認專案產生成功
- [x] 14.4 確認 `TestAdmin/` 中所有 `VueAppAdmin` 字串已替換（方案檔、csproj、命名空間、前端 package name）
- [x] 14.5 確認前端目錄名稱為 `testadmin.client`（小寫）
- [x] 14.6 確認產生目錄不含 `bin`、`obj`、`node_modules`、`.vs`
- [x] 14.7 在 `TestAdmin/` 執行 `dotnet run`，確認前後端均可正常啟動
- [x] 14.8 測試登入流程：以 `admin`/`password` 登入，API 回傳 JWT 與 displayName
- [x] 14.9 測試 ExampleItems 頁面：API 回傳 3 筆 dummy 資料

## 15. 清理

- [x] 15.1 刪除測試用 `TestAdmin/` 目錄
- [x] 15.2 若需要重置，執行 `dotnet new uninstall <path>`

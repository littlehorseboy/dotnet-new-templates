## Why

現有的 `vue-app-demo` 範本只提供最精簡的 Vue 3 + ASP.NET Core 骨架，每次開新的後台管理專案仍需手動加入 JWT 認證、Serilog 日誌、Pinia/Vue Router、UI 套件等基礎建設，耗時且容易遺漏細節。需要一個更有主見的範本，讓新專案直接具備後台管理系統的完整起點。

## What Changes

- 在 repo 新增 `vue-app-admin-dotnet8/` 目錄，作為獨立的 `dotnet new` 範本
- 後端新增：JWT 認證、Serilog 日誌、PasswordHasher、`IServiceCollectionExtensions` 服務註冊模式、`AuthController`/`AuthService`/`JwtService`、一個 ExampleItems controller + service（hardcoded dummy data）、DTO Request/Response 分層結構
- 前端新增：Pinia、Vue Router（含 beforeEach 登入守衛）、Axios、PrimeVue 4、Bootstrap 5、FontAwesome、Vee-Validate + Yup、`auth-store`、`user-info-store`、LoginView、MainLayout（header + sidebar from router meta）、DashboardView、ExampleItemsView
- 前端設定新增：`vite.config.ts`（HTTPS 憑證自動產生、`/api` proxy、`@` alias、dotenv）、`.env` + `.env.example`
- 新增 `.template.config/template.json`：`sourceName: VueAppAdmin`、`nameLower` symbol、排除清單

## Capabilities

### New Capabilities

- `template-config`: 為 vue-app-admin-dotnet8 加入 `dotnet new` 範本設定，可透過 `dotnet new vue-app-admin-dotnet8 -n MyApp` 建立專案
- `backend-auth`: JWT 認證層，含 `AuthController`（login/logout/me）、`AuthService`、`JwtService`、`JwtOptions` DTO、`JwtServiceCollectionExtensions`
- `backend-architecture`: 後端分層架構骨架，含 `ServiceRegistrationExtensions`、`SerilogHelper`、`PasswordHasherHelper`、DTO Request/Response 目錄結構、ExampleItems controller + service
- `frontend-auth`: 前端登入流程，含 `auth-store`（JWT localStorage + axios header）、`LoginView`、router beforeEach 守衛
- `frontend-layout`: 後台主版面，含 `MainLayout`、`MainHeader`、`MainSidebar`（從 `router meta.showInSidebar` 衍生）、`DashboardView`、`ExampleItemsView`
- `frontend-setup`: 前端基礎設定，含 `vite.config.ts`、`.env`/`.env.example`、完整套件清單（PrimeVue、Bootstrap、FontAwesome 等）

### Modified Capabilities

（無，不修改現有 `vue-app-demo` 範本）

## Impact

- 影響範圍：僅新增 `vue-app-admin-dotnet8/` 目錄，不修改任何現有程式碼或範本
- 相依性：依賴 `dotnet new install` 指令與 .NET 8 SDK；前端需 Node.js ≥ 20.19.0
- 安裝後透過 `dotnet new vue-app-admin-dotnet8 -n MyApp` 建立專案
- Short name `vue-app-admin-dotnet8` 含版本資訊，未來 .NET 9 版本可另立 `vue-app-admin-dotnet9`，互不衝突
- 前端 bundle 較 `vue-app-demo` 大幅增加（PrimeVue + Bootstrap + FA），屬已知取捨

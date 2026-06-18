## 1. 修正 vue-app-admin-dotnet8

- [x] 1.1 在 `vue-app-admin-dotnet8/VueAppAdmin.sln` 中，將 `VueAppAdmin.Server.csproj` 的 `Project(...)` 宣告移至 `vueappadmin.client.esproj` 之前
- [x] 1.2 確認 `GlobalSection` 區塊內容未被異動

## 2. 修正 vue-app-demo

- [x] 2.1 在 `vue-app-demo/VueApp1.sln` 中，將 `VueApp1.Server.csproj` 的 `Project(...)` 宣告移至 `vueapp1.client.esproj` 之前
- [x] 2.2 確認 `GlobalSection` 區塊內容未被異動

## 3. 驗證

- [x] 3.1 以 `dotnet new vue-app-admin-dotnet8` 產出新專案，用 VS 開啟確認預設 startup project 為 Server
- [x] 3.2 以 `dotnet new vue-app-demo` 產出新專案，用 VS 開啟確認預設 startup project 為 Server

## 1. 建立 template.json

- [x] 1.1 在 `VueApp1/` 下建立 `.template.config/` 目錄
- [x] 1.2 產生三組新 GUID
- [x] 1.3 建立 `.template.config/template.json`，內容包含：`author`、`classifications`、`identity`、`name`、`shortName`（`vue-app-demo`）、`description`、`tags`、`sourceName`（`VueApp1`）、`preferNameDirectory`、`guids`、`symbols`（`nameLower`）、`sources`（排除清單）

## 2. 安裝與驗證

- [x] 2.1 執行 `dotnet new install .\VueApp1`，確認安裝成功無錯誤
- [x] 2.2 執行 `dotnet new list`，確認 `vue-app-demo` 出現在清單中
- [x] 2.3 執行 `dotnet new vue-app-demo -n TestApp`，確認專案產生成功
- [x] 2.4 確認 `TestApp/` 中所有 `VueApp1` 字串已替換（方案檔、csproj、命名空間）
- [x] 2.5 確認前端目錄名稱為 `testapp.client`（小寫）
- [x] 2.6 確認產生目錄中不含 `bin`、`obj`、`node_modules`、`.vs`
- [x] 2.7 在 `TestApp/` 執行 `dotnet run`，確認前後端均可正常啟動

## 3. 清理

- [x] 3.1 刪除測試用 `TestApp/` 目錄
- [x] 3.2 執行 `dotnet new uninstall <path>` 若需要重置安裝狀態

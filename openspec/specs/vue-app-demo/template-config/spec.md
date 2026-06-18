## ADDED Requirements

### Requirement: 範本可成功安裝

系統 SHALL 允許使用者透過 `dotnet new install .\VueApp1` 將 VueApp1 安裝為本機範本，安裝後可在 `dotnet new list` 中看到該範本。

#### Scenario: 安裝成功
- **WHEN** 使用者在 `dotnet-new-templates` 根目錄執行 `dotnet new install .\VueApp1`
- **THEN** 指令回傳成功，`dotnet new list` 中出現短名稱 `vue-app-demo`

#### Scenario: 重複安裝
- **WHEN** 範本已安裝，使用者再次執行 `dotnet new install .\VueApp1`
- **THEN** 指令成功完成（覆蓋安裝），不產生錯誤

---

### Requirement: 專案名稱自動替換

系統 SHALL 將所有出現 `VueApp1` 的檔名、資料夾名稱與檔案內容，替換為使用者指定的 `-n` 名稱。

#### Scenario: 方案名稱替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo -n MyApp`
- **THEN** 產生的方案檔名稱為 `MyApp.sln`，內容中不含 `VueApp1`

#### Scenario: 命名空間替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo -n MyApp`
- **THEN** C# 檔案中的命名空間為 `MyApp.Server`，不含 `VueApp1`

#### Scenario: 小寫資料夾名稱替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo -n MyApp`
- **THEN** 前端目錄名稱為 `myapp.client`（全小寫），不含 `vueapp1`

---

### Requirement: 不納入暫存與產出檔案

系統 SHALL 在產生新專案時，排除 `bin`、`obj`、`node_modules`、`.git`、`.vs`、`.template.config`、`*.user` 及 `.DS_Store` 等目錄與檔案。

#### Scenario: 產生的專案不含 node_modules
- **WHEN** 使用者執行 `dotnet new vue-app-demo -n MyApp`
- **THEN** 產生目錄中不存在 `node_modules` 資料夾

#### Scenario: 產生的專案不含 bin/obj
- **WHEN** 使用者執行 `dotnet new vue-app-demo -n MyApp`
- **THEN** 產生目錄中不存在 `bin` 或 `obj` 資料夾

---

### Requirement: 產生的專案具備唯一 GUID

系統 SHALL 為每次產生的專案賦予獨立的 GUID，不與範本本身或其他範本的 GUID 重複。

#### Scenario: 兩次產生結果 GUID 不同
- **WHEN** 使用者先後執行 `dotnet new vue-app-demo -n AppA` 與 `dotnet new vue-app-demo -n AppB`
- **THEN** `AppA.sln` 與 `AppB.sln` 中的專案 GUID 不相同

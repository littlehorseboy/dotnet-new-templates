## ADDED Requirements

### Requirement: 方案檔使用 .slnx 格式
`vue-app-admin-dotnet8` 範本 SHALL 提供 `.slnx` 方案檔（XML 格式），不提供舊的 `.sln` 格式。

#### Scenario: 產生目錄含 .slnx 而非 .sln
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet8 -n MyApp`
- **THEN** 產生目錄中存在 `MyApp.slnx`，不存在 `MyApp.sln`

#### Scenario: .slnx 可被 dotnet CLI 識別
- **WHEN** 使用者於產生目錄執行 `dotnet build`
- **THEN** 建置成功，CLI 正確識別 `.slnx` 並建置 Server 與 client 兩個專案

### Requirement: 方案 GUID 由 template engine 替換
`template.json` 的 `guids` 清單 SHALL 包含 `.slnx` 中的方案 GUID（`D03907AB-36C4-4CD5-B032-407E50BBB78C`），使每次產生的方案有唯一 GUID。

#### Scenario: 產生的 .slnx 含新 GUID
- **WHEN** 使用者產生新專案
- **THEN** 產生的 `.slnx` 中的 `SolutionGuid` 與範本來源的 GUID 不同，為 template engine 重新產生的值

## ADDED Requirements

### Requirement: Server 為 .sln 預設 startup project
範本產出的 `.sln` 中，Server `.csproj` 的 `Project(...)` 宣告 SHALL 排列於 client `.esproj` 之前，使 Visual Studio 在無 `.suo` 時預設以 Server 作為 startup project。

#### Scenario: 首次開啟 .sln（無 .suo）
- **WHEN** 使用者以 VS 開啟 `dotnet new` 產出的 `.sln`，且同目錄無 `.suo` 檔
- **THEN** VS 綠色執行箭頭指向 Server `.csproj`，而非 client `.esproj`

#### Scenario: SpaProxy 自動啟動前端
- **WHEN** 使用者按下綠色箭頭執行 Server `.csproj`
- **THEN** SpaProxy 自動在 `SpaRoot` 目錄執行 `npm run dev`，前端 dev server 一併啟動

#### Scenario: GlobalSection 不受影響
- **WHEN** `.sln` 中 project 宣告順序對調後
- **THEN** `GlobalSection(ProjectConfigurationPlatforms)` 的 GUID 對應與 build 行為不變

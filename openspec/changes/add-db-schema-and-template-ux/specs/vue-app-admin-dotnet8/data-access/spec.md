## ADDED Requirements

### Requirement: In-memory 骨架的 TODO 註解提供對應 schema 的完整 SQL
維持 in-memory dummy 實作的資料存取點,其 TODO 註解 SHALL 包含對應 `db/schema.sql` 資料表的完整、可直接使用的 Dapper SQL,而非示意片段。涵蓋範圍:`Features/Auth/UserRepository.cs`(登入驗證與顯示名稱查詢,對應 `Basic_Users`,含 `Status` 與鎖定欄位判斷)、`Features/Auth/GroupFeatureStore.cs`(使用者功能權限查詢,對應 `Basic_Users_Groups` JOIN `Basic_Group_Modules` JOIN `Basic_Modules`)、`Features/Menu/MenuService.cs`(選單樹查詢,對應 `Basic_Modules`,含 `FatherModuleId` 組樹說明)。runtime 行為 SHALL 維持 in-memory,不建立實際資料庫連線。

#### Scenario: TODO SQL 與 schema 欄位一致
- **WHEN** 比對 `UserRepository.cs`、`GroupFeatureStore.cs`、`MenuService.cs` 註解中的 SQL 與 `db/schema.sql` 的資料表定義
- **THEN** SQL 引用的資料表名與欄位名皆存在於 schema 中,無拼字或欄位不一致

#### Scenario: runtime 行為不變
- **WHEN** 在未建立任何資料庫的環境啟動後端並呼叫登入、選單、功能清單 API
- **THEN** 行為與變更前相同(in-memory dummy 資料),不嘗試連線資料庫

#### Scenario: 既有測試不受影響
- **WHEN** 執行 `VueAppAdmin.Server.Tests` 全部測試
- **THEN** 測試全數通過,無需修改任何測試

## ADDED Requirements

### Requirement: Serilog Log 保留天數可透過設定檔控制

系統 SHALL 從 `appsettings.json` 的 `Logging:RetentionDays` 讀取 Log 檔案保留天數，而非 hardcode。若設定檔未包含該值，SHALL fallback 至預設值 `365`，確保行為不變。

#### Scenario: 設定檔包含 RetentionDays

- **WHEN** `appsettings.json` 的 `Logging:RetentionDays` 設為 `30`
- **THEN** Serilog 每日 rolling log 檔案最多保留 30 天，超過自動刪除

#### Scenario: 設定檔未包含 RetentionDays

- **WHEN** `appsettings.json` 的 `Logging` 區段未包含 `RetentionDays`
- **THEN** Log 保留天數預設為 365 天，行為與修改前相同

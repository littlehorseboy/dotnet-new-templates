## ADDED Requirements

### Requirement: JWT Token 到期時間可透過設定檔控制

系統 SHALL 從 `appsettings.json` 的 `Jwt:TokenExpirationHours` 讀取 Token 到期時數，而非 hardcode。`JwtOptions` SHALL 提供 `TokenExpirationHours` 屬性，預設值為 `8`，確保設定檔缺少該欄位時行為不變。

#### Scenario: 設定檔包含 TokenExpirationHours

- **WHEN** `appsettings.json` 的 `Jwt:TokenExpirationHours` 設為 `24`
- **THEN** 登入後取得的 JWT Token 到期時間距離簽發時間為 24 小時

#### Scenario: 設定檔未包含 TokenExpirationHours

- **WHEN** `appsettings.json` 的 `Jwt` 區段未包含 `TokenExpirationHours`
- **THEN** Token 到期時間預設為 8 小時，行為與修改前相同

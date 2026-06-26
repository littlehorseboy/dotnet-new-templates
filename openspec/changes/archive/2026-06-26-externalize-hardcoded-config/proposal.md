## Why

`JwtService.cs` 將 Token 到期時間寫死為 8 小時，`Program.cs` 將 Serilog Log 保留天數寫死為 365 天，導致調整這兩個值時必須修改程式碼重新 build，而非修改設定檔即可。這兩個值屬於環境/營運層級的參數，應透過 `appsettings.json` 管理。

## What Changes

- `JwtOptions.cs` 新增 `TokenExpirationHours` 屬性（預設 8）
- `appsettings.json` 的 `Jwt` 區段加入 `"TokenExpirationHours": 8`
- `JwtService.cs` 改用 `_jwtOptions.TokenExpirationHours` 取代 hardcode 的 `8`
- `appsettings.json` 的 `Logging` 區段加入 `"RetentionDays": 365`
- `Program.cs` 讀取 `Logging:RetentionDays` 設定值取代 hardcode 的 `365`

## Capabilities

### New Capabilities

- `jwt-token-config`：JWT Token 到期時間可透過 `appsettings.json` 設定，不需修改程式碼
- `log-retention-config`：Serilog Log 保留天數可透過 `appsettings.json` 設定，不需修改程式碼

### Modified Capabilities

（無既有 spec 受影響）

## Impact

- **受影響檔案**：`JwtOptions.cs`、`JwtService.cs`、`Program.cs`、`appsettings.json`
- **非 breaking change**：預設值維持不變（8 小時、365 天），行為不受影響
- **測試**：`AuthServiceTests.cs` 目前 mock `IJwtService`，不需調整

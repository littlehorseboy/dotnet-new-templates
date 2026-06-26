## Context

目前有兩處 hardcode 數值：

1. `JwtService.cs`：`DateTime.UtcNow.AddHours(8)` — Token 到期時間固定 8 小時
2. `Program.cs`：`retainedFileCountLimit: 365` — Serilog 檔案 Log 保留天數固定 365 天

`JwtOptions` 已存在並透過 `appsettings.json` 注入（`Issuer`、`SignKey`），模式已建立，直接擴充即可。
`Program.cs` 的 Serilog logger 建立於 `builder` 之後，可直接取用 `builder.Configuration`。

## Goals / Non-Goals

**Goals:**
- `TokenExpirationHours` 可透過 `appsettings.json` 設定，不需修改程式碼
- `RetentionDays` 可透過 `appsettings.json` 設定，不需修改程式碼
- 預設值維持不變（8 小時、365 天），不影響現有行為

**Non-Goals:**
- 不修改 Serilog 其他設定（log level、路徑等）
- 不將 Serilog 改為完全由 `appsettings.json` 驅動（`ReadFrom.Configuration()`）
- 不調整 `appsettings.Development.json`

## Decisions

### 決策 1：`TokenExpirationHours` 放入現有 `JwtOptions`

`JwtOptions` 已封裝所有 JWT 相關設定，加入 `TokenExpirationHours` 符合單一職責原則，不需新增 Options class。`[Required]` 不加（有預設值即可），避免舊設定檔升級時驗證失敗。

### 決策 2：`RetentionDays` 放 `Logging` 區段而非新增獨立區段

`appsettings.json` 已有 `Logging` 區段，將 `RetentionDays` 放於同層（`Logging:RetentionDays`）符合現有結構，不需新增頂層 key。

```json
"Logging": {
  "LogLevel": { ... },
  "RetentionDays": 365
}
```

讀取方式：
```csharp
var retentionDays = builder.Configuration.GetValue<int>("Logging:RetentionDays", 365);
```

`GetValue<int>` 提供 fallback 預設值，舊設定檔不設定此值時行為不變。

## Risks / Trade-offs

- **設定檔遺漏**：若部署時 `appsettings.json` 未包含新欄位，`TokenExpirationHours` 預設為 `0`（int 預設值），會產生已到期 Token。  
  → 緩解：`JwtOptions.TokenExpirationHours` 屬性設定預設值 `8`，確保即使設定檔缺少該欄位也能正常運作。

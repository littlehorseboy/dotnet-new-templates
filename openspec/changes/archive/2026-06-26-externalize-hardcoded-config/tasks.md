## 1. JWT Token 到期時間外部化

- [x] 1.1 `JwtOptions.cs`：新增 `public int TokenExpirationHours { get; set; } = 8;` 屬性
- [x] 1.2 `appsettings.json`：在 `Jwt` 區段加入 `"TokenExpirationHours": 8`
- [x] 1.3 `JwtService.cs`：將 `AddHours(8)` 改為 `AddHours(_jwtOptions.TokenExpirationHours)`

## 2. Serilog Log 保留天數外部化

- [x] 2.1 `appsettings.json`：在 `Logging` 區段加入 `"RetentionDays": 365`
- [x] 2.2 `Program.cs`：在 `systemLogger` 建立前加入 `var logRetentionDays = builder.Configuration.GetValue<int>("Logging:RetentionDays", 365);`
- [x] 2.3 `Program.cs`：將 `retainedFileCountLimit: 365` 改為 `retainedFileCountLimit: logRetentionDays`

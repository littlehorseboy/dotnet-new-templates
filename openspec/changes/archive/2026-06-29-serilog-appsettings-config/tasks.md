## 1. 加入 NuGet 套件

- [x] 1.1 在 `VueAppAdmin.Server.csproj` 加入 `Serilog.Settings.Configuration` 套件（使用 `dotnet add package` 或直接編輯 csproj）

## 2. 更新 appsettings 設定檔

- [x] 2.1 修改 `appsettings.json`：移除 `Logging:LogLevel` 區段，新增 `Serilog:MinimumLevel` 區段（`Default: Information`，`Override: { "Microsoft.AspNetCore": "Warning" }`）
- [x] 2.2 修改 `appsettings.Development.json`：移除 `Logging:LogLevel` 區段，新增 `Serilog:MinimumLevel` 區段（`Default: Debug`，`Override: { "Microsoft.AspNetCore": "Warning" }`）

## 3. 更新 Program.cs

- [x] 3.1 在 system logger 的 `LoggerConfiguration` 加入 `.ReadFrom.Configuration(builder.Configuration)`，讓 appsettings 的 `Serilog:MinimumLevel` 生效，並移除原本的 `.MinimumLevel.Information()` hardcode

## 4. 更新 README.md

- [x] 4.1 在 `VueAppAdmin.Server/README.md` 的 logging 相關段落新增說明：`Serilog:MinimumLevel` 的設定方式、`Default` 與 `Override` 的用途、以及 bootstrap logger 不受 appsettings 控制的說明

## 5. 驗證

- [x] 5.1 執行 `dotnet build` 確認編譯無誤
- [x] 5.2 確認 `appsettings.json` 中已無 `Logging:LogLevel` 殭屍設定
- [x] 5.3 確認 `appsettings.Development.json` 的 `Serilog:MinimumLevel:Default` 為 `Debug`

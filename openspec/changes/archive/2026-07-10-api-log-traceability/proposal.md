## Why

目前範本的 API log 機制存在兩個缺口：

1. `ExceptionHandlingMiddleware` 掛在 MVC pipeline 最外層，攔截未處理例外時會繞過 `ApiLogFilter.OnResultExecuted`。同一個 request，正常結束時只有 `ApiLogFilter` 寫的一行 log，發生未處理例外時只有 middleware 寫的一行 log，兩者沒有任何共同欄位可以確認是同一次呼叫。
2. `db/schema.sql` 中已存在 `Basic_Api_Log`、`Basic_Api_Change_Log` 兩張 log 表，目前完全沒有程式碼寫入（已用 grep 確認 `*.cs` 中無任何引用）。`Basic_Api_Log` 的欄位設計（`Actions` 有 `REQUEST/RESPONSE/EXCEPTION/SCHEDULE` 幾種值）暗示同一次呼叫應寫入多列並互相對應，但現有欄位沒有任何 correlation id 可以配對這些列，也沒有耗時、Method、Path、StatusCode 等可直接查詢的欄位（`Datas` 只是一包未強制格式的 JSON blob）。此外 `Basic_Api_Log.GroupGuid` 欄位型別（`uniqueidentifier`）與範本 `Basic_Groups.GroupId`（`int`）不符、無 FK constraint、程式碼零引用，為範本沿用過程留下的孤兒欄位。

範本產生的專案一開始就是空白骨架（Repository 方法多為 TODO），是在缺陷被實際踩到之前，於範本層級把 log 可追溯性基礎補齊的合適時機，讓所有由此範本產生的專案都能直接受益。

## What Changes

- 全域採用 ASP.NET Core 內建的 `HttpContext.TraceIdentifier` 作為單一 correlation id，不另外設計 id 產生機制。
- `ApiLogFilter`（正常路徑）與 `ExceptionHandlingMiddleware`（例外路徑）皆在既有 log 樣板中加入此 correlation id 欄位，使兩條路徑寫出的 log 可透過同一個值互相對應。
- `Basic_Api_Log` 新增欄位：`RequestId`、`Method`、`Path`、`StatusCode`、`ElapsedMs`，並移除現有的 `GroupGuid` 欄位（確認為無關聯的孤兒欄位）。
- `Basic_Api_Log.Datas` 欄位加上 `CHECK (Datas IS NULL OR ISJSON(Datas) = 1)` constraint，強制只能寫入合法 JSON。**BREAKING**：若未來有程式寫入非 JSON 字串到 `Datas`，該次 INSERT/UPDATE 會被資料庫拒絕（目前無任何程式碼寫入此表，範本層級不影響既有資料）。
- `Basic_Api_Change_Log` 新增欄位：`RequestId`，用於串回造成該筆資料異動的 API 呼叫。
- 所有新增欄位依範本既有慣例，以 `sys.sp_addextendedproperty` 補齊繁體中文欄位說明（`MS_Description`）。
- `Basic_Api_Log.ModulesNames` 欄位沿用現有格式（`Namespace.Class/Method`），與新增的 `Path` 欄位互補（前者為 code 視角、後者為 HTTP 視角），並於 `ApiLogFilter.cs` 加上程式碼註解，說明未來串接 `ModulesNames` 時應透過 `ActionDescriptor` 自動取得，不得在各 Controller/Action 手動硬編碼。

## Capabilities

### New Capabilities

無新增獨立 capability（本次為 `logging` 的需求延伸，含 log 表 schema 部分）。

### Modified Capabilities

- `vue-app-admin-dotnet8/logging`：新增「correlation id 貫穿正常與例外兩種結束路徑」的需求，並新增 `Basic_Api_Log`/`Basic_Api_Change_Log` 的欄位層級需求（可直接查詢的關鍵欄位、`Datas` 僅能存放合法 JSON、`Basic_Api_Change_Log` 可串回觸發異動的 API 呼叫）。

## Impact

- **程式碼**：`VueAppAdmin.Server/Shared/Logging/ApiLogFilter.cs`、`VueAppAdmin.Server/Shared/Middleware/ExceptionHandlingMiddleware.cs` 調整 log 輸出樣板，加入 correlation id 欄位。
- **資料庫 schema**：`db/schema.sql` 異動 `Basic_Api_Log`（新增 `RequestId`/`Method`/`Path`/`StatusCode`/`ElapsedMs`、移除 `GroupGuid`、新增 `Datas` 的 JSON CHECK constraint）與 `Basic_Api_Change_Log`（新增 `RequestId`）。
- **範圍**：僅影響 `vue-app-admin-dotnet8` 範本；`vue-app-demo` 未包含此 log 機制，不涉及。
- **不影響**：`Basic_LoginLog`、`Basic_OperationLog` 兩張表本次不異動。實際將 `Basic_Api_Log`/`Basic_Api_Change_Log` 接上程式碼寫入邏輯（Repository/Service）不在本次範圍內，留待後續依當時需求另行規劃。
- **相依性**：不新增任何 NuGet 套件，`HttpContext.TraceIdentifier` 為框架內建功能。

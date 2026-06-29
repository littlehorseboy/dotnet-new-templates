## Context

目前 `vue-app-admin-dotnet8` 的 logging 架構已具備：
- Serilog 基礎設定（bootstrap logger + system logger）
- `UseSerilogRequestLogging()` 自動記錄 HTTP 層基本資訊
- `ExceptionHandlingMiddleware` 攔截未處理例外
- `InvalidModelStateResponseFactory` 記錄 400 驗證失敗

缺口：沒有統一機制記錄 API 的 request payload 與 response 內容，也沒有覆蓋 401 授權短路的案例。各處 log 格式不一，缺乏可比對的一致性。

Controller 層均為薄層（無額外邏輯，直接呼叫 Service），Service 層無法取得 HTTP context（user、path、method、status code），因此選擇在 MVC Filter Pipeline 實作而非 Service 層。

## Goals / Non-Goals

**Goals:**
- 統一 API log 格式，三個案例（正常流程、401、400）均使用相同 template
- log 完整 request payload（敏感欄位以 `[LogMask]` 遮罩）
- log 完整 response 內容供初期觀察（`ApiResponse<T>` 完整序列化）
- 覆蓋 Authorization 短路（401）的案例
- 預留程式碼結構供未來「存表」擴充

**Non-Goals:**
- 登入紀錄寫入資料庫（TODO：未來開 `LoginLogs` 表）
- API log 存表（TODO：未來開 `ApiLogs` 表，屆時 `ToLogSummary()` 改為精簡格式）
- Response body 壓縮或截斷（目前全量 log，待觀察後再決定是否精簡）
- Service 層個別 log 補全

## Decisions

### 決策 1：使用 ActionFilter 而非 Service 層 logging

**選擇**：`ApiLogFilter` 實作 `IActionFilter` + `IAlwaysRunResultFilter`，全域套用於 MVC pipeline。

**理由**：
- Service 層無法取得 HTTP context（user identity、path、method、status code）
- `IAlwaysRunResultFilter` 是唯一能在 Authorization 短路（401）後仍執行的 filter 介面
- Controller 為薄層，ActionFilter 的 `ActionArguments` 與 service 接收的參數等價
- 一個 class 覆蓋三個案例，不需對每個 Service 個別改動

**捨棄的替代方案**：
- Middleware 層攔截：request body stream 只能讀一次，需 `EnableBuffering()` 且在 model binding 前無法取得強型別物件
- Service Decorator：每個 Service 介面都需一個裝飾器，維護成本高，且無法取得 HTTP context

---

### 決策 2：`[LogMask]` attribute + reflection 遮罩

**選擇**：自訂空 attribute `[LogMask]`，filter 序列化前用 reflection 將標記欄位替換為 `"***"`。

**理由**：
- 顯式標記，搜尋 codebase 可立即找到所有敏感欄位
- 不侵入序列化設定，不影響其他 JSON 輸出（API response 仍正常輸出）
- 未來新增 Request 類別時，開發者明確知道需要加 `[LogMask]`

**目前適用欄位**：`LoginRequest.Password`

---

### 決策 3：`ApiResponse<T>` 加 `ToLogSummary()`，目前回傳完整內容

**選擇**：在 `ApiResponse<T>` 加入 `ToLogSummary()` 方法，當前實作直接回傳 `this`（完整序列化）。

**理由**：
- 先觀察完整 log 是否過大，再決定是否精簡
- 方法存在後，未來切換只需修改一個地方
- `ApiPagedResponse<T>` 另外加 `ToLogSummary()` 回傳 `{ Success, Total, Count }` 的精簡摘要，因為分頁資料量較大

**TODO**：當「API Log 存表」功能實作時，`ToLogSummary()` 改為精簡格式（`Success`、`Message`、筆數），完整 payload 由資料庫欄位保存。

---

### 決策 4：401 案例只記錄 HTTP context，不讀取 request body

**選擇**：401 短路後，log 僅包含 method、path、user（空）、status code，不嘗試讀取 request body。

**理由**：
- 401 發生在 model binding 之前，body 尚未反序列化
- 強行讀取需 `EnableBuffering()`，會對所有請求造成額外記憶體開銷
- 401 的診斷重點是「誰在沒帶 token 的情況下呼叫哪個端點」，body 內容非必要

---

### 決策 5：整合 validation log，移除 `InvalidModelStateResponseFactory` 中的重複 log

**選擇**：`InvalidModelStateResponseFactory` 的 `LogWarning` 邏輯移除，統一由 `ApiLogFilter` 的 `IAlwaysRunResultFilter` 路徑處理。

**理由**：維持單一 log 來源，避免同一請求出現兩筆格式不同的 log。

## Risks / Trade-offs

- **[風險] Response 完整序列化導致 log 過大** → 初期觀察，視情況調整 `ToLogSummary()` 或加截斷長度上限
- **[風險] reflection 遮罩有效能開銷** → 僅在 request 序列化時執行一次（per request），屬可接受範圍；若未來 Request 物件欄位數增加再評估快取策略
- **[取捨] ActionFilter 無法感知 Service 內部邏輯** → 此層 log 定位為「API 邊界」記錄，Service 內部的業務邏輯 log 仍可個別補充
- **[取捨] 401 案例無 request body** → 已在決策 4 說明，屬設計選擇非缺陷

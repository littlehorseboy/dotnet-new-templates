## Context

`ExampleItemsController.GetAll` 目前一次回傳全部資料，前端 DataTable 以 client-side 模式呈現。`ApiResponse<T>` 是唯一的回傳型別，不含分頁後設資料。

目前 3 筆假資料無法展示分頁效果；作為 template 應示範真實的 server-side 分頁與欄位排序流程。

## Goals / Non-Goals

**Goals:**
- 後端 `GetAll` 支援 `skip` / `top` / `sortField` / `sortOrder` query parameters
- 新增 `ApiPagedResponse<T>` 型別，供分頁端點專用
- 前端 DataTable 改為 PrimeVue lazy 模式，翻頁與排序時觸發 API 呼叫

**Non-Goals:**
- 不實作 OData 完整規格（`$filter`、`$expand`、`$select`）
- 不修改 `ApiResponse<T>` 現有結構或工廠方法
- 不對 `ExampleItems` 以外的端點套用分頁

## Decisions

### 決策 1：ApiPagedResponse\<T\> 繼承 ApiResponse\<T\>

`ApiPagedResponse<T>` 繼承 `ApiResponse<T>`，額外加入 `Total` 屬性。

```csharp
public class ApiPagedResponse<T> : ApiResponse<T>
{
    public int Total { get; set; }

    public static ApiPagedResponse<T> OkPaged(IEnumerable<T> results, int total)
        => new() { Success = true, Results = results, Total = total };
}
```

**考量替代方案**：
- 在 `ApiResponse<T>` 直接加 `Total?` — 被捨棄，因為非分頁端點會出現 `total: null`，語義混雜
- 全新獨立型別（不繼承）— 被捨棄，會重複 `Success`、`Message` 欄位

### 決策 2：Query Parameters 採用 skip / top / sortField / sortOrder

鏡像 OData 語義但不引入套件：

| 參數 | 型別 | 預設值 | 說明 |
|------|------|--------|------|
| `skip` | int | 0 | 跳過筆數 |
| `top` | int | 10 | 取回筆數上限 |
| `sortField` | string | `"id"` | 排序欄位名稱 |
| `sortOrder` | string | `"asc"` | `asc` 或 `desc` |

PrimeVue DataTable lazy 事件送出的 `first` / `rows` 直接對應 `skip` / `top`，前後端映射零摩擦。

**考量替代方案**：
- page / pageSize — 直覺但需要後端多算 offset；`skip` 更接近 LINQ `.Skip()` 語義
- 完整 OData 套件 — 引入額外路由設定與序列化複雜度，template 不需要

### 決策 3：假資料擴充至 30 筆

Service 內以迴圈產生 30 筆假資料，確保分頁（每頁 10 筆）有三頁可示範翻頁。

### 決策 4：sortField 後端以 switch 對應 LINQ 排序

接收到 `sortField` 字串後以 `switch` 表達式對應至 LINQ 排序屬性，避免反射安全疑慮。`sortOrder` 為 `"desc"` 時使用 `OrderByDescending`，其餘一律視為 `asc`。

## Risks / Trade-offs

- **假資料限制** → 排序與分頁邏輯正確性依賴假資料涵蓋度；已擴充至 30 筆且欄位值有差異，足以驗證
- **sortField 字串輸入未嚴格驗證** → 後端 `switch` default 回退到 `id` 排序，不拋例外；前端只能點欄標題觸發，不存在任意輸入風險
- **ApiPagedResponse 繼承帶有 Result 屬性** → `OkPaged` 只填 `Results`，`Result` 維持 null；前端不使用 `result` 欄位，無實際影響

## Open Questions

（無）

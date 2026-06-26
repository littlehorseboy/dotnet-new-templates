namespace VueAppAdmin.Server.Shared;

// 分頁版回應格式，繼承 ApiResponse<T> 並加上總筆數
// Total 供前端計算總頁數，Results 為當頁資料
public class ApiPagedResponse<T> : ApiResponse<T>
{
    public int Total { get; set; }

    public static ApiPagedResponse<T> OkPaged(IEnumerable<T> results, int total)
        => new() { Success = true, Results = results, Total = total };
}

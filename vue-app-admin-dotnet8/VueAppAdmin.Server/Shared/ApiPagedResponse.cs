namespace VueAppAdmin.Server.Shared;

public class ApiPagedResponse<T> : ApiResponse<T>
{
    public int Total { get; set; }

    public static ApiPagedResponse<T> OkPaged(IEnumerable<T> results, int total)
        => new() { Success = true, Results = results, Total = total };
}

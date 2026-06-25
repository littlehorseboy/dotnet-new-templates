namespace VueAppAdmin.Server.Shared;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }
    public IEnumerable<T>? Results { get; set; }

    public static ApiResponse<T> Ok(T result, string? message = null)
        => new() { Success = true, Result = result, Message = message };

    public static ApiResponse<T> OkList(IEnumerable<T> results, string? message = null)
        => new() { Success = true, Results = results, Message = message };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };
}

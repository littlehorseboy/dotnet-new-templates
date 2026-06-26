using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Shared.Middleware;

// 全域例外攔截中介軟體：捕捉所有未處理的例外，
// 統一回傳 HTTP 500 + ApiResponse 格式，避免原始堆疊追蹤洩漏給前端
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail("系統錯誤"));
        }
    }
}

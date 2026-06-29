using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Shared.Logging;

public class ApiLogFilter : IActionFilter, IAlwaysRunResultFilter
{
    private static readonly Serilog.ILogger _logger = SerilogHelper.GetLogger<ApiLogFilter>();

    private const string RequestKey = "ApiLog.Request";
    private const string StartTickKey = "ApiLog.StartTick";
    internal const string ValidationErrorsKey = "ApiLog.ValidationErrors";

    // 正常流程：暫存 masked request 與起始時間
    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items[StartTickKey] = Environment.TickCount64;
        var requestObj = context.ActionArguments.Values.FirstOrDefault();
        context.HttpContext.Items[RequestKey] = MaskRequest(requestObj);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    // 401/400 短路時 OnActionExecuting 未執行，此處補設起始時間
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (!context.HttpContext.Items.ContainsKey(StartTickKey))
            context.HttpContext.Items[StartTickKey] = Environment.TickCount64;
    }

    // 所有案例（正常、401、400）都在這裡寫 log
    public void OnResultExecuted(ResultExecutedContext context)
    {
        var httpContext = context.HttpContext;

        var elapsedMs = httpContext.Items[StartTickKey] is long startTick
            ? Environment.TickCount64 - startTick
            : -1L;

        var user = httpContext.User?.FindFirstValue(ClaimTypes.Name) ?? "-";
        var statusCode = httpContext.Response.StatusCode;
        var requestData = httpContext.Items[RequestKey];

        object? responseData;
        if (httpContext.Items.TryGetValue(ValidationErrorsKey, out var validationErrors))
            responseData = validationErrors;
        else if (context.Result is ObjectResult { Value: IApiResponse apiResp })
            responseData = apiResp.ToLogSummary();
        else
            responseData = null;

        if (statusCode is 400 or 401 or 403)
            _logger.Warning(
                "[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms",
                httpContext.Request.Method, httpContext.Request.Path, user, statusCode, requestData, responseData, elapsedMs);
        else
            _logger.Information(
                "[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms",
                httpContext.Request.Method, httpContext.Request.Path, user, statusCode, requestData, responseData, elapsedMs);
    }

    // 用 reflection 將標記 [LogMask] 的欄位替換為 "***"
    private static object? MaskRequest(object? requestObj)
    {
        if (requestObj is null) return null;

        var properties = requestObj.GetType().GetProperties();
        var dict = new Dictionary<string, object?>(properties.Length);

        foreach (var prop in properties)
        {
            dict[prop.Name] = prop.IsDefined(typeof(LogMaskAttribute), false)
                ? "***"
                : prop.GetValue(requestObj);
        }
        return dict;
    }
}

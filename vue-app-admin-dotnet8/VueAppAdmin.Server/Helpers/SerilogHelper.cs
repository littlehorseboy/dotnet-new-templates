using Serilog;

namespace VueAppAdmin.Server.Helpers;

public static class SerilogHelper
{
    public static void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 180)
            .WriteTo.File(
                new Serilog.Formatting.Json.JsonFormatter(),
                "logs/log-.json",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 180)
            .CreateLogger();
    }
}

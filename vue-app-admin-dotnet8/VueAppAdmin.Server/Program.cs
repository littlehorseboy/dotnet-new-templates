using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Serilog;
using VueAppAdmin.Server.Features.Auth;
using VueAppAdmin.Server.Features.ExampleItems;
using VueAppAdmin.Server.Shared;
using VueAppAdmin.Server.Shared.Database;
using VueAppAdmin.Server.Shared.Jwt;
using VueAppAdmin.Server.Shared.Logging;
using VueAppAdmin.Server.Shared.Middleware;

SerilogHelper.Initialize();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var systemLogger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/log-system-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 365)
        .CreateLogger();

    builder.Services.AddSerilog(systemLogger);

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
    })
    .AddNewtonsoftJson()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            logger.LogWarning("Validation failed {Path} {StatusCode} {@ValidationErrors}",
                context.HttpContext.Request.Path, 400, errors);

            return new BadRequestObjectResult(ApiResponse<object>.Fail("驗證失敗"));
        };
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }
                },
                []
            }
        });
    });

    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddAuthFeature();
    builder.Services.AddExampleItemsFeature();

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapFallbackToFile("/index.html");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

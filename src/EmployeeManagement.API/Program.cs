using Asp.Versioning;
using EmployeeManagement.API.ExceptionHandling;
using EmployeeManagement.API.Filters;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ─── Logging Setup (Serilog) ────────────────────────────────────────────────
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddlewareImpl", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
});

// ─── Services ───────────────────────────────────────────────────────────────

builder.Services.AddInfrastructure(builder.Configuration);

// Add globally wrapped responses via ApiResponseFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Health Checks
builder.Services.AddHealthChecks();

// Built-in .NET OpenAPI document generation (Scalar UI)
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new()
        {
            Title = "Employee Management API",
            Version = "v1",
            Description = "REST API for managing employees and departments. " +
                          "Built with ASP.NET Core, Clean Architecture, Repository/UoW pattern, and FluentValidation.",
            Contact = new() { Name = "Osama Mahmoud" }
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ─── Pipeline ────────────────────────────────────────────────────────────────

var app = builder.Build();

// Auto-apply migrations on startup
await ApplyMigrationsAsync(app);

app.UseExceptionHandler();

// Custom Request Logging Middleware
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    // Serve the OpenAPI JSON at /openapi/v1.json
    app.MapOpenApi();

    // Scalar UI at /scalar/v1
    app.MapScalarApiReference(options =>
    {
        options.Title = "Employee Management API";
        options.Theme = ScalarTheme.DeepSpace;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.MapHealthChecks("/health");

app.MapControllers();

// Serve index.html for root — fallback after API routes
app.MapFallbackToFile("index.html");

app.Lifetime.ApplicationStarted.Register(() =>
{
    var urls = app.Urls.Count > 0 ? string.Join(", ", app.Urls) : "http://localhost:5000";
    app.Logger.LogInformation("API Documentation is available at: {Url}/scalar/v1", urls.Split(',').First().Trim());
    app.Logger.LogInformation("Application Frontend is available at: {Url}", urls.Split(',').First().Trim());
});

app.Run();

// ─── Helpers ─────────────────────────────────────────────────────────────────
static async Task ApplyMigrationsAsync(WebApplication app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var pending = await db.Database.GetPendingMigrationsAsync();
        if (pending.Any())
        {
            app.Logger.LogInformation("Applying {Count} pending migration(s)...", pending.Count());
            await db.Database.MigrateAsync();
            app.Logger.LogInformation("Migrations applied successfully.");
        }
        else
        {
            app.Logger.LogInformation("No pending database migrations found.");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database migration failed");
        throw;
    }
}

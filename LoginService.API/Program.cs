using LoginService.Application.Interfaces;
using LoginService.Infrastructure.Data;
using LoginService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation;

var builder = WebApplication.CreateBuilder(args);

//  TELEMETRY CONSTANTS
const string ServiceName = "login-service";
const string ServiceVersion = "1.0.0";

// RESOURCE (Datadog-required)
var resource = ResourceBuilder.CreateDefault()
    .AddService(ServiceName, ServiceVersion)
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = "development"
    });

//  LOGGING (Logs + Correlation)
builder.Logging.ClearProviders(); //Removes default .NET logging providers
builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resource);
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.AddOtlpExporter(); // OTLP → Datadog
});

//  TRACING (Spans, DB, APIs)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(resource)
            .SetSampler(new AlwaysOnSampler())

            //  Service spans
            .AddAspNetCoreInstrumentation()

            // Downstream/internal/external API spans
            .AddHttpClientInstrumentation()

            // Database spans
            .AddSqlClientInstrumentation(o => o.RecordException = true)

            // Custom spans
            .AddSource(ServiceName)

            .AddConsoleExporter()   // DEBUG (temporary)

            // Export traces
            .AddOtlpExporter();

            
    });

//  REQUIRED for HttpClient instrumentation
builder.Services.AddHttpClient();

//  APPLICATION SERVICES
builder.Services.AddDbContext<LoginDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//  MIDDLEWARE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
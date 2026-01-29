using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(serviceName: builder.Environment.ApplicationName))
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddPrometheusExporter())
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation());
}

var app = builder.Build();
{
    app.UseOpenTelemetryPrometheusScrapingEndpoint();

    app.MapGet("/test", () =>
    {
        var list = Enumerable
            .Range(1, 10000)
            .Select(c => new
            {
                Name = new string('*', 10),
                Description = new string('*', 1000),
            })
            .ToList();
        return list;
    });
}
app.Run();

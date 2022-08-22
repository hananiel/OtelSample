using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Steeltoe.Management.Endpoint;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddAllActuators();

// OpenTelemetry configuration
var openTelemetryServiceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
var openTelemetryEndpoint =  Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT");


if (!string.IsNullOrWhiteSpace(openTelemetryEndpoint))
{
    //// Configure metrics
    builder.Services.AddOpenTelemetryMetricsForSteeltoe((_, b) =>
    {
        b.AddHttpClientInstrumentation();
        b.AddAspNetCoreInstrumentation();
        b.AddMeter(openTelemetryServiceName + "-metrics");
        b.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(openTelemetryEndpoint);
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    });

    // Configure tracing
    builder.Services.AddOpenTelemetryTracing(b =>
    {
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(openTelemetryServiceName));
        b.AddHttpClientInstrumentation();
        b.AddAspNetCoreInstrumentation();
        b.AddSource(openTelemetryServiceName + "-activity-source");
        b.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(openTelemetryEndpoint);
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    });

    // Configure logging
    builder.Logging.AddOpenTelemetry(b =>
    {
        b.IncludeFormattedMessage = true;
        b.IncludeScopes = true;
        b.ParseStateValues = true;
        b.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(openTelemetryEndpoint);
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
        b.AddConsoleExporter();
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();



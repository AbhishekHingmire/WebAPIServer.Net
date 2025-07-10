

using OpenTelemetry.Resources;
using System.Runtime.InteropServices;
using WK.Server.OpenTelemetry.Contrib.Net.Builder;
using WebAPIServer.Net;
using OpenTelemetry.Logs;

const string SERVICE_NAME = "WebAPIServer.Net";
const string SERVICE_NAMESPACE = "WebAPIServer.Net";


var builder = WebApplication.CreateBuilder(args);
void SetupLogger()
{
    // Initializing observability
    try
    {
        StartObservability();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
void StartObservability()
{
    WKInstrumentationBuilder wkInstrumentationBuilder = new WKInstrumentationBuilder(SERVICE_NAME, SERVICE_NAMESPACE, "1.0.0");
    
    var isOtelSqlInstrumentationEnabled = builder.Configuration.GetValue<bool>("Observability:EnableOtelSqlInstrumentation", false);
    var isOtelHttpInstrumentationEnabled = builder.Configuration.GetValue<bool>("Observability:EnableOtelHttpInstrumentation", false);
    var isOtelAspNetInstrumentationEnabled = builder.Configuration.GetValue<bool>("Observability:EnableOtelAspNetInstrumentation", false);

    wkInstrumentationBuilder.SetWkInstrumentationOptions((opt) =>
    {
        opt.IsHttpInstrumentationEnabled = isOtelHttpInstrumentationEnabled;
        opt.IsAspNetInstrumentationEnabled = isOtelAspNetInstrumentationEnabled;
        opt.IsSqlInstrumentationEnabled = isOtelSqlInstrumentationEnabled;
        opt.SuppressWKInstrumentation = (x) => false;
    }).Build();
}
SetupLogger();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();

    loggingBuilder.AddOpenTelemetry(options =>
    {
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName: "WebAPIServer.Net").AddAttributes(new Dictionary<string, object>
        {
            ["host.name"] = Environment.MachineName,
            ["os.description"] = RuntimeInformation.OSDescription
        }).AddTelemetrySdk();

        options.IncludeScopes = true;
        options.IncludeFormattedMessage = true;
        options.SetResourceBuilder(resourceBuilder).AddProcessor(new LogProcessor()).AddOtlpExporter();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

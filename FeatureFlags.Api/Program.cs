using FeatureFlags.Api;
using Itenium.Forge.Controllers;
using Itenium.Forge.HealthChecks;
using Itenium.Forge.Logging;
using Itenium.Forge.Settings;
using Serilog;

Log.Logger = LoggingExtensions.CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddForgeSettings<FeatureFlagsSettings>();
    builder.AddForgeLogging();

    builder.AddForgeControllers();
    builder.AddForgeProblemDetails();
    builder.AddForgeHealthChecks();

    builder.Services.AddSingleton<FlagStore>();

    var app = builder.Build();

    app.UseForgeProblemDetails();
    app.UseForgeLogging();
    app.UseForgeControllers();
    app.UseForgeHealthChecks();

    app.MapGet("/", () => "Hello from Feature Flags");

    await app.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync().ConfigureAwait(false);
}

#pragma warning disable S1118 // needed for WebApplicationFactory<Program> in tests
public partial class Program { }

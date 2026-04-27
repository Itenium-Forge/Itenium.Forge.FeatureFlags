using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FeatureFlags.Api.Tests;

public class FeatureFlagsFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.PostConfigure<HealthCheckServiceOptions>(options =>
            {
                var otlp = options.Registrations.FirstOrDefault(r => string.Equals(r.Name, "otlp", StringComparison.Ordinal));
                if (otlp != null)
                    options.Registrations.Remove(otlp);
            });
        });
    }
}

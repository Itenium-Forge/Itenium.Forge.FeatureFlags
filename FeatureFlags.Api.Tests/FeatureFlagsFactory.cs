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
                // Remove all health checks except 'self' to avoid external dependencies during tests
                var toRemove = options.Registrations
                    .Where(r => !string.Equals(r.Name, "self", StringComparison.Ordinal))
                    .ToList();

                foreach (var registration in toRemove)
                {
                    options.Registrations.Remove(registration);
                }
            });
        });
    }
}

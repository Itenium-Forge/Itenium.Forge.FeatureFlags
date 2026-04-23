using Itenium.Forge.Core;
using Itenium.Forge.Settings;

namespace FeatureFlags.Api;

/// <summary>Application settings for the FeatureFlags microservice.</summary>
public class FeatureFlagsSettings : IForgeSettings
{
    /// <inheritdoc />
    public ForgeSettings Forge { get; } = new();
}

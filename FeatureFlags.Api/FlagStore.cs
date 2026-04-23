namespace FeatureFlags.Api;

/// <summary>In-memory store for feature flags.</summary>
public class FlagStore
{
    private readonly Dictionary<string, bool> _flags = new(StringComparer.Ordinal)
    {
        ["dark-mode"]     = true,
        ["new-dashboard"] = false,
        ["beta-export"]   = false,
    };

    /// <summary>Returns all feature flags and their current state.</summary>
    public IEnumerable<Flag> GetAll() =>
        _flags.Select(kv => new Flag(kv.Key, kv.Value));

    /// <summary>Toggles the flag with the given name and returns the updated flag, or null if not found.</summary>
    public Flag? Toggle(string name)
    {
        if (!_flags.ContainsKey(name))
            return null;

        _flags[name] = !_flags[name];
        return new Flag(name, _flags[name]);
    }
}

namespace FeatureFlags.Api;

public class FlagStore
{
    private readonly Dictionary<string, bool> _flags = new(StringComparer.Ordinal)
    {
        ["dark-mode"]     = true,
        ["new-dashboard"] = false,
        ["beta-export"]   = false,
    };

    public IEnumerable<Flag> GetAll() =>
        _flags.Select(kv => new Flag(kv.Key, kv.Value));

    public Flag? Toggle(string name)
    {
        if (!_flags.ContainsKey(name))
            return null;

        _flags[name] = !_flags[name];
        return new Flag(name, _flags[name]);
    }
}

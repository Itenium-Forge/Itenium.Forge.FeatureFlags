using FeatureFlags.Api;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.Api.Controllers;

/// <summary>Manages feature flags.</summary>
[ApiController]
[Route("api/[controller]")]
public class FlagsController(FlagStore store) : ControllerBase
{
    /// <summary>Returns all feature flags.</summary>
    [HttpGet]
    [ProducesResponseType<Flag[]>(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(store.GetAll());

    /// <summary>Toggles the specified feature flag.</summary>
    [HttpPut("{name}/toggle")]
    [ProducesResponseType<Flag>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Toggle(string name)
    {
        var flag = store.Toggle(name);
        return flag is null ? NotFound() : Ok(flag);
    }
}

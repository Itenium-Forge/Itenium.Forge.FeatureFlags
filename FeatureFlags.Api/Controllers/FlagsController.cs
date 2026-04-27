using FeatureFlags.Api;
using Itenium.Forge.Core;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.Api.Controllers;

/// <summary>Manages feature flags.</summary>
[ApiController]
[Route("api/[controller]")]
public class FlagsController(FlagStore store) : ControllerBase
{
    /// <summary>Returns a paginated list of feature flags.</summary>
    [HttpGet]
    [ProducesResponseType<ForgePagedResult<Flag>>(StatusCodes.Status200OK)]
    public IActionResult Get([FromQuery] ForgePageQuery query)
    {
        var allFlags = store.GetAll().ToArray();
        var pagedFlags = allFlags
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        return Ok(new ForgePagedResult<Flag>(pagedFlags, allFlags.Length, query.Page, query.PageSize));
    }

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

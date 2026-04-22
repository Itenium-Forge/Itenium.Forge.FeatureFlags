using FeatureFlags.Api;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlagsController(FlagStore store) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(store.GetAll());

    [HttpPut("{name}/toggle")]
    public IActionResult Toggle(string name)
    {
        var flag = store.Toggle(name);
        return flag is null ? NotFound() : Ok(flag);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamDesk.Api.Dtos.Lookups;
using TeamDesk.Api.Services;

namespace TeamDesk.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Manager")]               // seul un Manager a besoin de la liste (pour assigner)
public class UsersController : ControllerBase
{
    private readonly ILookupService _lookup;
    public UsersController(ILookupService lookup) => _lookup = lookup;

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> Get() => Ok(await _lookup.GetUsersAsync());
}
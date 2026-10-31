using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamDesk.Api.Dtos.Lookups;
using TeamDesk.Api.Services;

namespace TeamDesk.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]                                  // tout utilisateur connecté
public class CategoriesController : ControllerBase
{
    private readonly ILookupService _lookup;
    public CategoriesController(ILookupService lookup) => _lookup = lookup;

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> Get() => Ok(await _lookup.GetCategoriesAsync());
}
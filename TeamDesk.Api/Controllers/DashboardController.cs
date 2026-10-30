using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamDesk.Api.Services;
using static TeamDesk.Api.Dtos.Requests.RequestDtos;

namespace TeamDesk.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(Roles = "Manager")]            // tout le contrôleur est réservé aux Managers
    public class DashboardController : ControllerBase
    {
        private readonly IRequestService _service;
        public DashboardController(IRequestService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> Get()
            => Ok(await _service.GetDashboardAsync());
    }
}
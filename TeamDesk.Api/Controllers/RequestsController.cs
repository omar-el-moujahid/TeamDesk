using Microsoft.AspNetCore.Mvc;
using TeamDesk.Api.Dtos;
using TeamDesk.Api.Entities.enums;
using TeamDesk.Api.Serveur;
using static TeamDesk.Api.Dtos.RequestDtos;

namespace TeamDesk.Api.Controllers
{
    [ApiController]                   // validation auto des DTOs (400) + liaison du JSON
    [Route("api/[controller]")]       // [controller] = "Requests" → /api/requests
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _service;

        // TEMPORAIRE : Sara (Id 2). Demain, on lira l'Id depuis le token JWT.
        private const int CurrentUserId = 2;

        public RequestsController(IRequestService service)   // on injecte l'INTERFACE
        {
            _service = service;
        }

        // GET /api/requests?status=New&page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PagedResult<RequestListItemDto>>> GetAll(
            [FromQuery] RequestStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 50)
                return BadRequest("Pagination invalide.");            // 400

            return Ok(await _service.GetAllAsync(status, page, pageSize));   // 200
        }

        // GET /api/requests/5
        [HttpGet("{id:int}")]             // :int = la route n'accepte que des nombres
        public async Task<ActionResult<RequestDetailDto>> GetById(int id)
        {
            var request = await _service.GetByIdAsync(id);
            return request is null ? NotFound() : Ok(request);        // 404 ou 200
        }

        // POST /api/requests
        [HttpPost]
        public async Task<ActionResult<RequestDetailDto>> Create(RequestCreateDto dto)
        {
            var created = await _service.CreateAsync(dto, CurrentUserId);
            if (created is null) return BadRequest("Catégorie inconnue.");

            // 201 Created + en-tête Location: /api/requests/{id}
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PATCH /api/requests/5/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
        {
            var updated = await _service.UpdateStatusAsync(id, dto.Status);
            return updated ? NoContent() : NotFound();                // 204 ou 404
        }
    }
}
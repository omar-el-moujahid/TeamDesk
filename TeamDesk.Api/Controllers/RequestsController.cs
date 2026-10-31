using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamDesk.Api.Entities.enums;
using TeamDesk.Api.Services;
using static TeamDesk.Api.Dtos.Requests.RequestDtos;

namespace TeamDesk.Api.Controllers
{
    [ApiController]
    [Authorize]                               // toutes les routes exigent un token valide (sinon 401)
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _service;

        public RequestsController(IRequestService service)
        {
            _service = service;
        }

        // L'Id de l'utilisateur connecté, lu dans le claim NameIdentifier du token
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /api/requests?status=New&page=1&pageSize=10
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<RequestListItemDto>>> GetAll(
            [FromQuery] RequestStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 50)
                return BadRequest("Pagination invalide.");

            return Ok(await _service.GetAllAsync(status, page, pageSize));
        }

        // GET /api/requests/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RequestDetailDto>> GetById(int id)
        {
            var request = await _service.GetByIdAsync(id);
            return request is null ? NotFound() : Ok(request);
        }

        // POST /api/requests
        [HttpPost]
        [Authorize(Roles = "User")]           // "User" doit correspondre exactement à UserRole.User.ToString()
        public async Task<ActionResult<RequestDetailDto>> Create(RequestCreateDto dto)
        {
            var created = await _service.CreateAsync(dto, CurrentUserId);   // le créateur vient du token
            if (created is null) return BadRequest("Catégorie inconnue.");

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // 201 + Location
        }

        // PUT /api/requests/5/assign
        [HttpPut("{id:int}/assign")]
        [Authorize(Roles = "Manager")]        // un User reçoit 403
        public async Task<IActionResult> Assign(int id, AssignRequestDto dto)
        {
            var result = await _service.AssignAsync(id, dto.AssignedToId);
            return result switch              // switch expression : un résultat → un code HTTP
            {
                OperationResult.Success => NoContent(),                              // 204
                OperationResult.NotFound => NotFound(),                              // 404
                OperationResult.Invalid => BadRequest("Utilisateur à assigner inconnu."), // 400
                _ => StatusCode(500)          // cas imprévu
            };
        }

        // PATCH /api/requests/5/status
        [HttpPatch("{id:int}/status")]        // pas de Roles ici : la règle "Manager OU assigné" est dans le service
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
        {
            var result = await _service.UpdateStatusAsync(
                id, dto.Status, CurrentUserId, User.IsInRole("Manager"));

            return result switch
            {
                OperationResult.Success => NoContent(),   // 204
                OperationResult.NotFound => NotFound(),   // 404
                OperationResult.Forbidden => Forbid(),    // 403
                _ => StatusCode(500)
            };
        }

        // POST /api/requests/5/comments
        [HttpPost("{id:int}/comments")]       // tout utilisateur connecté (hérite du [Authorize] de la classe)
        public async Task<ActionResult<CommentDto>> AddComment(int id, CommentCreateDto dto)
        {
            var comment = await _service.AddCommentAsync(id, dto.Content, CurrentUserId);
            if (comment is null) return NotFound();

            return CreatedAtAction(nameof(GetById), new { id }, comment);   // 201, Location → la demande
        }
    }
}
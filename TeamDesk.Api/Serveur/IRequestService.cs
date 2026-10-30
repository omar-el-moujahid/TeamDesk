using Azure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamDesk.Api.Entities;
using TeamDesk.Api.Entities.enums;
using static TeamDesk.Api.Dtos.RequestDtos;

namespace TeamDesk.Api.Serveur
{
    public interface IRequestService
    {
        Task<PagedResult<RequestListItemDto>> GetAllAsync(RequestStatus? status, int page, int pageSize);
        Task<RequestDetailDto?> GetByIdAsync(int id);              // ? = null si introuvable
        Task<RequestDetailDto?> CreateAsync(RequestCreateDto dto, int userId);
        Task<bool> UpdateStatusAsync(int id, RequestStatus status); // false si introuvable

    }
}

using Azure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamDesk.Api.Entities;
using TeamDesk.Api.Entities.enums;
using static TeamDesk.Api.Dtos.Requests.RequestDtos;

namespace TeamDesk.Api.Services
{
    public interface IRequestService
    {
        Task<PagedResult<RequestListItemDto>> GetAllAsync(RequestStatus? status, int page, int pageSize);
        Task<RequestDetailDto?> GetByIdAsync(int id);              // ? = null si introuvable
        Task<RequestDetailDto?> CreateAsync(RequestCreateDto dto, int userId);
        Task<bool> UpdateStatusAsync(int id, RequestStatus status); // false si introuvable

        Task<OperationResult> AssignAsync(int requestId, int assignedToId);
        Task<OperationResult> UpdateStatusAsync(int requestId, RequestStatus status, int userId, bool isManager); // ← signature modifiée
        Task<CommentDto?> AddCommentAsync(int requestId, string content, int authorId);
        Task<DashboardDto> GetDashboardAsync();

    }
}

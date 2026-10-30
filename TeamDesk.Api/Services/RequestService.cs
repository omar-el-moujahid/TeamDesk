using Microsoft.EntityFrameworkCore;
using TeamDesk.Api.Data;
using TeamDesk.Api.Entities;
using TeamDesk.Api.Entities.enums;
using static TeamDesk.Api.Dtos.Requests.RequestDtos;

namespace TeamDesk.Api.Services
{
    public class RequestService : IRequestService
    {
        // peut plus le remplacer après le constructeur 
        private readonly AppDbContext _context;

        public RequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RequestListItemDto>> GetAllAsync(RequestStatus? status, int page, int pageSize)
        {
            var query = _context.Requests.AsNoTracking();
            if (status.HasValue)
            
                query = query.Where(r => r.Status == status.Value);
                var total = await query.CountAsync();   // 1re requête SQL : SELECT COUNT(*)

                var items = await query
                    .OrderByDescending(r => r.CreatedAt)   // les plus récentes d'abord
                    .Skip((page - 1) * pageSize)           // page 2, taille 10 → on saute 10 lignes
                    .Take(pageSize)                        // → OFFSET / FETCH en SQL
                    .Select(r => new RequestListItemDto(   // PROJECTION : SQL ne lit que ces colonnes
                        r.Id, r.Title, r.Status, r.Priority,
                        r.Category.Name,                   // EF Core génère le JOIN tout seul
                        r.CreatedBy.Name,
                        r.AssignedTo != null ? r.AssignedTo.Name : null,
                        r.CreatedAt))
                    .ToListAsync();                        // 2e requête SQL : exécutée ICI

                return new PagedResult<RequestListItemDto>(items, total, page, pageSize);
        }

        public async Task<RequestDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Requests
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RequestDetailDto(
                    r.Id, r.Title, r.Description, r.Status, r.Priority,
                    r.Category.Name, r.CreatedBy.Name,
                    r.AssignedTo != null ? r.AssignedTo.Name : null,
                    r.CreatedAt,
                    r.Comments
                        .OrderBy(c => c.CreatedAt)
                        .Select(c => new CommentDto(c.Id, c.Content, c.Author.Name, c.CreatedAt))
                        .ToList()))
                .FirstOrDefaultAsync();                // null si aucune demande avec cet Id
        }


        public async Task<RequestDetailDto?> CreateAsync(RequestCreateDto dto, int userId)
        {
            // Règle métier : la catégorie doit exister
            if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                return null;

            var request = new Request
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Priority = dto.Priority,
                CreatedById = userId
                // Status = New et CreatedAt = UtcNow : valeurs par défaut de l'entité
            };

            _context.Requests.Add(request);      // EF Core SUIT l'objet (état "Added")
            await _context.SaveChangesAsync();   // INSERT en SQL ; request.Id est rempli après

            return await GetByIdAsync(request.Id);
        }

        public async Task<bool> UpdateStatusAsync(int id, RequestStatus status)
        {
            var request = await _context.Requests.FindAsync(id);   // AVEC tracking : on va modifier
            if (request is null) return false;

            request.Status = status;               // EF Core détecte le changement
            await _context.SaveChangesAsync();     // UPDATE uniquement sur la colonne Status
            return true;
        }

        public async Task<OperationResult> AssignAsync(int requestId, int assignedToId)
        {
            var request = await _context.Requests.FindAsync(requestId);       // FindAsync : cherche par clé primaire (vérifie d'abord le cache du DbContext)
            if (request is null) return OperationResult.NotFound;

            var userExists = await _context.Users.AnyAsync(u => u.Id == assignedToId); // AnyAsync → SQL "EXISTS" : ne charge pas l'utilisateur
            if (!userExists) return OperationResult.Invalid;

            request.AssignedToId = assignedToId;   // entité suivie (tracking) : EF détecte le changement
            await _context.SaveChangesAsync();          // génère un UPDATE uniquement sur la colonne modifiée
            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateStatusAsync(int requestId, RequestStatus status, int userId, bool isManager)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request is null) return OperationResult.NotFound;

            // Règle métier : seul un Manager OU la personne assignée peut changer le statut
            if (!isManager && request.AssignedToId != userId) return OperationResult.Forbidden;

            request.Status = status;
            await _context.SaveChangesAsync();
            return OperationResult.Success;
        }

        public async Task<CommentDto?> AddCommentAsync(int requestId, string content, int authorId)
        {
            var exists = await _context.Requests.AnyAsync(r => r.Id == requestId);
            if (!exists) return null;              // le contrôleur traduira en 404

            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.UtcNow,       // UTC en base, conversion à l'affichage côté Angular
                RequestId = requestId,
                AuthorId = authorId                // vient du token, jamais du client
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();          // après ça, comment.Id est rempli par SQL Server (IDENTITY)

            var authorName = await _context.Users
                .Where(u => u.Id == authorId)
                .Select(u => u.Name)               // projection : on ne lit que la colonne Name
                .FirstAsync();

            return new CommentDto(comment.Id, comment.Content, authorName, comment.CreatedAt);
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var counts = await _context.Requests
                .GroupBy(r => r.Status)                                   // → SQL : GROUP BY Status
                .Select(g => new { Status = g.Key, Count = g.Count() })   // → SELECT Status, COUNT(*)
                .ToListAsync();                                           // le calcul est fait par SQL Server, pas en mémoire

            // Un statut sans demande n'apparaît pas dans le GROUP BY : on complète avec 0
            var byStatus = Enum.GetValues<RequestStatus>()
                .Select(s => new StatusCountDto(
                    s.ToString(),
                    counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0))
                .ToList();

            return new DashboardDto(byStatus.Sum(x => x.Count), byStatus);
        }
    }
}

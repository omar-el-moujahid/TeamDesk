using System.ComponentModel.DataAnnotations;
using TeamDesk.Api.Entities.enums;

namespace TeamDesk.Api.Dtos
{
    public class RequestDtos
    {
        // ---------- ENTRÉE : ce que le client ENVOIE ----------
        public class RequestCreateDto
        {
            [Required, MaxLength(200)]           // [ApiController] renvoie un 400 automatiquement si invalide
            public string Title { get; set; } = string.Empty;

            [Required]
            public string Description { get; set; } = string.Empty;

            [Range(1, int.MaxValue)]             // refuse CategoryId = 0
            public int CategoryId { get; set; }

            [EnumDataType(typeof(Priority))]     // refuse une valeur hors de l'enum (ex. 99)
            public Priority Priority { get; set; } = Priority.Medium;
            // PAS de Status, CreatedAt, CreatedById : c'est le serveur qui les décide
        }

        public class UpdateStatusDto
        {
            [EnumDataType(typeof(RequestStatus))]
            public RequestStatus Status { get; set; }
        }


        // ---------- SORTIE : ce que l'API RENVOIE ----------
        // record = classe immuable, en une ligne : idéal pour des données qu'on renvoie sans les modifier
        public record CommentDto(int Id, string Content, string AuthorName, DateTime CreatedAt);

        public record RequestListItemDto(
            int Id, string Title, RequestStatus Status, Priority Priority,
            string CategoryName, string CreatedByName, string? AssignedToName, DateTime CreatedAt);

        public record RequestDetailDto(
            int Id, string Title, string Description, RequestStatus Status, Priority Priority,
            string CategoryName, string CreatedByName, string? AssignedToName, DateTime CreatedAt,
            List<CommentDto> Comments);

        // Réponse paginée générique : réutilisable pour n'importe quelle liste
        public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
    }
}

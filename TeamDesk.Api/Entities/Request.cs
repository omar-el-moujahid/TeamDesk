using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using TeamDesk.Api.Entities.enums;

namespace TeamDesk.Api.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public string Title {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RequestStatus Status { get; set; } = RequestStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Priority Priority { get; set; } = Priority.Medium;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}

//Request(Id, Title, Description, Category, Priority, Status,
//CreatedAt, CreatedById → User, AssignedToId → User ?)
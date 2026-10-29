using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Xml.Linq;
using TeamDesk.Api.Entities.enums;

namespace TeamDesk.Api.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;

        public ICollection<Request> CreatedRequests { get; set; } = new List<Request>();
        public ICollection<Request> AssignedRequests { get; set; } = new List<Request>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}

//User(Id, Name, Email, PasswordHash, Role)

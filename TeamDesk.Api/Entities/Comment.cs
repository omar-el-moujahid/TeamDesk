namespace TeamDesk.Api.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RequestId { get; set; }
        public Request Request { get; set; } = null!;

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;
    }
}

//Comment(Id, Content, CreatedAt, RequestId → Request, AuthorId → User)

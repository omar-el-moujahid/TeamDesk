using System.Xml.Linq;

namespace TeamDesk.Api.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection <Request> Requests { get; set; } = new List<Request> ();
    }
}

//Category(Id, Name)
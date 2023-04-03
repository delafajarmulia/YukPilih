using System.Text.Json.Serialization;

namespace Polling.Model
{
    public class Division
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public List<User> Users { get; set; }
        //public List<Vote> Votes { get; set; }
    }
}

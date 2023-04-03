using System.Text.Json.Serialization;

namespace Polling.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime? CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set;}

        [JsonIgnore]
        public Division Division { get; set; }
        //public int? DivisionId { get; set; }
        //public List<Poll> Polls { get; set; }
        
        //public List<Vote> Votes { get; set; }
    }
}

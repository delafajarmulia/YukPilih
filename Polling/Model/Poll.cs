using Newtonsoft.Json;

namespace Polling.Model
{
    public class Poll
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CreatedAt { get; set;}
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
        //public int? UserId { get; set; }
        public string? CreatedBy { get; set; }
    }
}

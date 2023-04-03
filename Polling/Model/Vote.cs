using Newtonsoft.Json;

namespace Polling.Model
{
    public class Vote
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //[JsonIgnore]
        //[JsonRequired]
        public Choices Choice { get; set; }

        public User User { get; set; }
        public Division Division { get; set; }
    }
}

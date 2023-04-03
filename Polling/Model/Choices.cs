using Newtonsoft.Json;

namespace Polling.Model
{
    public class Choices
    {
        public int Id { get; set; }
        public string? Choice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //[JsonIgnore]
        public Poll Poll { get; set; }

        [JsonIgnore]
        public List<Vote> Votes { get; set; }
    }
}

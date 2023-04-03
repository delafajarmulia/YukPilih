namespace Polling.Model
{
    public class ChoiceDto
    {
        public string? Choice { get; set; }
        public int PollId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

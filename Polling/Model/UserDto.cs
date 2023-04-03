using System.Text.Json.Serialization;

namespace Polling.Model
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int DivisionId { get; set; }
    }
}

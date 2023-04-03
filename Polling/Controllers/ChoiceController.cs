using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polling.Data;
using Polling.Model;
using Polling.Service.UserService;

namespace Polling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoiceController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public ChoiceController(DataContext context, IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Choices>> PostChoice(ChoiceDto request)
        {
            Poll? poll = await _context.Polls.Where(p => p.Id == request.PollId).FirstOrDefaultAsync();
            if (poll == null) 
                return NotFound("Poll not found.");

            var choice = new Choices
            {
                Choice = request.Choice,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                Poll = poll
            };

            _context.Choices.Add(choice);
            await _context.SaveChangesAsync();
            return Ok(choice);
        }
    }
}

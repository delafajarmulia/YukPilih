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
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public UserController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET api/User/{id}
        [HttpGet, Authorize(Roles = "Admin")]   
        public async Task<ActionResult<List<User>>> Get(int userId)
        {
            var user = await _context.Users.Where(u => u.Id == userId).ToListAsync();

            if (user.Count == 0)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST user
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(UserDto request)
        {
            var division = await _context.Divisions.Where(d => d.Id == request.DivisionId).FirstAsync();
            if (division == null) 
                return NotFound();

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                Role = request.Role,
                CreatedAt = DateTime.Now,
                Division = division
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, UserDto request)
        {
            var division = await _context.Divisions.Where(d => d.Id == request.DivisionId).FirstOrDefaultAsync();
            if (division == null)
                return NotFound("division not found");

            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return NotFound("user not found");

            user.Username = request.Username;
            user.Password = request.Password;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.Now;
            user.Division = division;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<User>>> Delete(int id)
        {
            var user = await _context.Users.Where(u =>u.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return NotFound("user not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.ToListAsync());
        }

        
    }
}

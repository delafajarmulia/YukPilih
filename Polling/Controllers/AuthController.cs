using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polling.Data;
using Polling.Model;
using Polling.Service.UserService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Polling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(DataContext context, IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(new
            {
                Username = userName
            });
            //return Ok(userName);
        }

        /*
         
         */

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login(LoginDto request)
        {

            var user = await _context.Users.Where(u => u.Username == request.Username).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            } 
            else if (user.Password !=  request.Password)
            {
                return BadRequest();
            } 
            else
            {
                var token = CreateToken(user);
                user.Token = token;
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Token = token
                });  
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims : claims,
                    expires : DateTime.Now.AddDays(1),
                    signingCredentials : cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [HttpPut("ChangePassword"), Authorize]
        public async Task<ActionResult<User>> ChangePw(ChangePwDto request)
        {
            var userName = _userService.GetMyName();
            User? user = await _context.Users.Where(u => u.Username == userName).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            } 
            else if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest("new password and confirm password must be the same.");
            }
            else if (request.OldPassword != user.Password)
            {
                return BadRequest("Password wrong.");
            }
            else
            {
                user.Password = request.NewPassword;
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    User = user
                });
                //return Ok(user);
            }
            
        }
    }
}

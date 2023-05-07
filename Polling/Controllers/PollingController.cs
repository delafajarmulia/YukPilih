using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Polling.Data;
using Polling.Model;
using Polling.Service.UserService;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace Polling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollingController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public PollingController(DataContext context, IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Poll>>> CreatePoll(PollDto request)
        {
            var userName = _userService.GetMyName();
            User? user = await _context.Users.Where(u => u.Username == userName).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User not found.");
            }
            else
            {
                var poll = new Poll
                {
                    Title = request.Title,
                    Description = request.Description,
                    Deadline = request.Deadline,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    DeletedAt = null,
                    User = user,
                    CreatedBy = user.Username
                };
                _context.Polls.Add(poll);
                await _context.SaveChangesAsync();
                return Ok(poll);
            }
        }

        [HttpGet("{id}"), Authorize(Roles = "User")]
        public async Task<ActionResult<Poll>> GetPoll(int id)
        {
            var poll = await _context.Polls.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (poll == null)
                return NotFound("Polling not found.");

            return Ok(poll);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Poll>>> GetAllPoll()
        {
            return Ok(await _context.Polls.ToListAsync());
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Poll>>> DeletePoll(int id)
        {
            var poll = await _context.Polls.Where(p =>p.Id == id).FirstOrDefaultAsync();
            if (poll == null)
                return NotFound("Polling not found.");

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();
            return Ok(await _context.Polls.ToListAsync());
        }

        
        [HttpPost("{id}/Vote"), Authorize(Roles = "User")]
        public async Task<ActionResult<Poll>> Voting(VoteDto request)
        {
            var user = await _context.Users
                .Where(u => u.Username == _userService.GetMyName())
                .Include(d => d.Division)
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            var choice = await _context.Choices
                .Where(c => c.Id == request.ChoiceId)
                .FirstOrDefaultAsync();

            if (choice == null) return NotFound();

            var vote = new Vote
            {
                Choice = choice,
                User = user,
                Division = user.Division,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                User = user,
                Division = user.Division,
                CreatedAt = vote.CreatedAt,
                UpdatedAt = vote.UpdatedAt,
                Choice = new
                {
                    id = choice.Id,
                    name = choice.Choice
                }
            });
        }

        [HttpGet("{id}/Result")]
        public async Task<ActionResult<List<Poll>>> ResultPoll(int id)
        {
            
            var choices = await _context.Choices.Where(c => c.Poll.Id == id).Include(v => v.Votes).ToListAsync();

            int jumDiv = await _context.Divisions.CountAsync();
            int[] divisi = new int[jumDiv];
            var divisis = await _context.Divisions.ToListAsync();
            double[] voteChoice = new double[(int)(double)jumDiv];
            Dictionary<int, Dictionary<int, double>> votesAllDivision = new Dictionary<int, Dictionary<int, double>>();
            Dictionary<int, double> voteAllChoice = new Dictionary<int, double>();
            Dictionary<int, double> persenVote = new Dictionary<int, double>();
            
            
            foreach (var div in divisis)
            {
                Dictionary<int, int> votePerDivisiDictionary = new Dictionary<int, int>();
                foreach (var choice in choices)
                {
                    votePerDivisiDictionary.Add(choice.Id, choice.Votes.Where(v => v.Division.Id == div.Id).Count());
                    Console.WriteLine(choice.Id);
                }
                var max = votePerDivisiDictionary.Values.Max();


                var filter = votePerDivisiDictionary.Where(v => v.Value.Equals(max)).ToDictionary(x => x.Key, x => x.Value);

                var point = filter.Select(di =>
                            new KeyValuePair<int, double>(di.Key, max != 0 ? 1.0/filter.Count() : 0.0)).ToDictionary(x => x.Key, x => x.Value);

                votesAllDivision.Add(div.Id, point);
            }

            foreach (var item in votesAllDivision)
            {
                foreach (var iv in item.Value)
                {
                    if(voteAllChoice.Count == 0)
                    {
                        voteAllChoice.Add(iv.Key, iv.Value);
                        Console.WriteLine(iv.Key);
                    }
                    else if(voteAllChoice.Count != 0)
                    {
                        if (voteAllChoice.ContainsKey(iv.Key)) {
                            voteAllChoice[iv.Key] += iv.Value;
                        } else
                        {
                            voteAllChoice.Add(iv.Key, iv.Value);
                        }
                    } 

                }
                Console.WriteLine(item.Key);
            }

            var jumlah = 0.0;
            foreach (var item in voteAllChoice)
            {
               var allVote = await _context.Votes.CountAsync();
                jumlah += item.Value;
            }

            foreach(var item in voteAllChoice)
            {
                var persen = item.Value / jumlah * 100;
                persenVote.Add(item.Key, persen);
            }
            return Ok(persenVote);
        }
        
    }
}

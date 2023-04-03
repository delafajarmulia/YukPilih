using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polling.Data;
using Polling.Model;

namespace Polling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionController : ControllerBase
    {
        private readonly DataContext _context;
        
        public DivisionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Division>>> Get(int id)
        {
            var division = await _context.Divisions
                            .Where(d => d.Id == id)
                            .Include(x => x.Users)
                            .ToListAsync();
            if (division == null) return NotFound();

            return Ok(await _context.Divisions.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<List<Division>>> Post(DivisionDto request)
        {
            var division = new Division
            {
                Name = request.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Divisions.Add(division);
            await _context.SaveChangesAsync();

            return Ok(division);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Division>> Update(int id, DivisionDto request)
        {
            var division = await _context.Divisions.Where(d => d.Id == id).Include(x => x.Users).FirstOrDefaultAsync();
            if (division == null) return NotFound();

            division.Name = request.Name;
            division.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(division);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Division>>> Delete(int id)
        {
            var division = await _context.Divisions.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (division == null) return NotFound();

            _context.Divisions.Remove(division);
            await _context.SaveChangesAsync();

            return Ok(await _context.Divisions.ToListAsync());
        }
    }
}

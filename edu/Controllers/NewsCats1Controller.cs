using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using edu.Models;

namespace edu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsCats1Controller : ControllerBase
    {
        private readonly edu_portal_dbContext _context;

        public NewsCats1Controller(edu_portal_dbContext context)
        {
            _context = context;
        }

        // GET: api/NewsCats1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsCat>>> GetNewsCats()
        {
          if (_context.NewsCats == null)
          {
              return NotFound();
          }
            return await _context.NewsCats.ToListAsync();
        }

        // GET: api/NewsCats1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsCat>> GetNewsCat(int id)
        {
          if (_context.NewsCats == null)
          {
              return NotFound();
          }
            var newsCat = await _context.NewsCats.FindAsync(id);

            if (newsCat == null)
            {
                return NotFound();
            }

            return newsCat;
        }

        // PUT: api/NewsCats1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNewsCat(int id, NewsCat newsCat)
        {
            if (id != newsCat.Id)
            {
                return BadRequest();
            }

            _context.Entry(newsCat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsCatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/NewsCats1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NewsCat>> PostNewsCat(NewsCat newsCat)
        {
          if (_context.NewsCats == null)
          {
              return Problem("Entity set 'edu_portal_dbContext.NewsCats'  is null.");
          }
            _context.NewsCats.Add(newsCat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNewsCat", new { id = newsCat.Id }, newsCat);
        }

        // DELETE: api/NewsCats1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsCat(int id)
        {
            if (_context.NewsCats == null)
            {
                return NotFound();
            }
            var newsCat = await _context.NewsCats.FindAsync(id);
            if (newsCat == null)
            {
                return NotFound();
            }

            _context.NewsCats.Remove(newsCat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NewsCatExists(int id)
        {
            return (_context.NewsCats?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

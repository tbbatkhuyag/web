using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using edu.Models;
using System.Security.Claims;
using Kendo.Mvc.Extensions;

namespace edu.Controllers
{
    public class GroupLessonsController : Controller
    {
        private readonly edu_portal_dbContext _context;
      
        public GroupLessonsController(edu_portal_dbContext context)
        {
            _context = context;
        }
        
        // GET: GroupLessons
        public async Task<IActionResult> Index()
        {
            var id = User.FindFirst(ClaimTypes.Dns)?.Value;
            return _context.GroupLessons != null ? 
                        View(await _context.GroupLessons.Where(vs=>vs.TeacherId ==Convert.ToInt32(id)).ToListAsync()) :
                      

                Problem("Entity set 'edu_portal_dbContext.GroupLessons'  is null.");
        }
        public async Task<IActionResult> Indexstudent()
        {
            return _context.GroupLessons != null ?
                        View(await _context.GroupLessons.ToListAsync()) :
                        Problem("Entity set 'edu_portal_dbContext.GroupLessons'  is null.");
        }
        public async Task<IActionResult> Indexpublic()
        {
            return _context.GroupLessons != null ?
                        View(await _context.GroupLessons.ToListAsync()) :
                        Problem("Entity set 'edu_portal_dbContext.GroupLessons'  is null.");
        }

        // GET: GroupLessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GroupLessons == null)
            {
                return NotFound();
            }

            var groupLesson = await _context.GroupLessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupLesson == null)
            {
                return NotFound();
            }

            return View(groupLesson);
        }

    

        // GET: GroupLessons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GroupLessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Logo,TeacherId,UniId,Txtdatetime")] GroupLesson groupLesson)
        {
            if (ModelState.IsValid)
            {
                var id = User.FindFirst(ClaimTypes.Dns)?.Value; 
                groupLesson.TeacherId = Convert.ToInt32(id) ;
                groupLesson.Txtdatetime = DateTime.Now;
                _context.Add(groupLesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groupLesson);
        }

        // GET: GroupLessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GroupLessons == null)
            {
                return NotFound();
            }

            var groupLesson = await _context.GroupLessons.FindAsync(id);
            if (groupLesson == null)
            {
                return NotFound();
            }
            return View(groupLesson);
        }

        // POST: GroupLessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Logo,TeacherId,UniId,Txtdatetime")] GroupLesson groupLesson)
        {
            if (id != groupLesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupLesson);
                    _context.Entry(groupLesson).Property(v => v.TeacherId).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupLessonExists(groupLesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(groupLesson);
        }

        // GET: GroupLessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GroupLessons == null)
            {
                return NotFound();
            }

            var groupLesson = await _context.GroupLessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupLesson == null)
            {
                return NotFound();
            }

            return View(groupLesson);
        }

        // POST: GroupLessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GroupLessons == null)
            {
                return Problem("Entity set 'edu_portal_dbContext.GroupLessons'  is null.");
            }
            var groupLesson = await _context.GroupLessons.FindAsync(id);
            if (groupLesson != null)
            {
                _context.GroupLessons.Remove(groupLesson);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupLessonExists(int id)
        {
          return (_context.GroupLessons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

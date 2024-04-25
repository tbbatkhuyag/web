using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using edu.Models;

namespace edu.Controllers
{
    public class NewsCatsController : Controller
    {
        private readonly edu_portal_dbContext _context;

        public NewsCatsController(edu_portal_dbContext context)
        {
            _context = context;
        }

        // GET: NewsCats
        public async Task<IActionResult> Index()
        {
              return _context.NewsCats != null ? 
                          View(await _context.NewsCats.ToListAsync()) :
                          Problem("Entity set 'edu_portal_dbContext.NewsCats'  is null.");
        }

        // GET: NewsCats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NewsCats == null)
            {
                return NotFound();
            }

            var newsCat = await _context.NewsCats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsCat == null)
            {
                return NotFound();
            }

            return View(newsCat);
        }

        // GET: NewsCats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewsCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Txtname,Vis,TxtOrd,SubId,Txtmore,Txtcontent,TxtDate")] NewsCat newsCat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newsCat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsCat);
        }

        // GET: NewsCats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NewsCats == null)
            {
                return NotFound();
            }

            var newsCat = await _context.NewsCats.FindAsync(id);
            if (newsCat == null)
            {
                return NotFound();
            }
            return View(newsCat);
        }

        // POST: NewsCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Txtname,Vis,TxtOrd,SubId,Txtmore,Txtcontent,TxtDate")] NewsCat newsCat)
        {
            if (id != newsCat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsCat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsCatExists(newsCat.Id))
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
            return View(newsCat);
        }

        // GET: NewsCats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.NewsCats == null)
            {
                return NotFound();
            }

            var newsCat = await _context.NewsCats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsCat == null)
            {
                return NotFound();
            }

            return View(newsCat);
        }

        // POST: NewsCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.NewsCats == null)
            {
                return Problem("Entity set 'edu_portal_dbContext.NewsCats'  is null.");
            }
            var newsCat = await _context.NewsCats.FindAsync(id);
            if (newsCat != null)
            {
                _context.NewsCats.Remove(newsCat);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsCatExists(int id)
        {
          return (_context.NewsCats?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

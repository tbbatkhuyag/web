using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using edu.Models;
using SQLitePCL;
using System.Web;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


namespace edu.Controllers
{
    public class NewsListsController : Controller
    {
        private readonly edu_portal_dbContext _context;

        public NewsListsController(edu_portal_dbContext context)
        {
            _context = context;
        }

        // GET: NewsLists
        public async Task<IActionResult> Index(int? sid)
        {
            IQueryable<NewsList> edu_portal_dbContext;

            if (sid != null && sid != 0)
            {
                TempData["newsid"] = sid;
                ViewData["newsid"] = sid;

                edu_portal_dbContext = _context.NewsLists.Where(v => v.NewsCatId == sid.Value);
            }
            else
            {
                edu_portal_dbContext = _context.NewsLists;
            }

            return View(await edu_portal_dbContext.ToListAsync());
        }
        public async Task<IActionResult> list(int? id)
        {
            try
            {
                IQueryable<NewsList> edu_portal_dbContext;

            if (id != null && id != 0)
            {
                TempData["newsid"] = id;
                ViewData["newsid"] = id;

                edu_portal_dbContext = _context.NewsLists.Where(v => v.NewsCatId == id.Value);
            }
            else
            {
                edu_portal_dbContext = _context.NewsLists;
            }

            return View(await edu_portal_dbContext.ToListAsync());
            }
            catch (Exception ex)
            {
                // Алдааг log хийнэ
                Console.WriteLine(ex.ToString());
                return Content("Error: " + ex.Message); // Түр хугацаанд алдааг шууд харуулах
            }
        }

        // GET: NewsLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NewsLists == null)
            {
                return NotFound();
            }

            var newsList = await _context.NewsLists
               
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsList == null)
            {
                return NotFound();
            }

            return View(newsList);
        }
        public async Task<IActionResult> read(int? id)
        {
            if (id == null || _context.NewsLists == null)
            {
                return NotFound();
            }

            var newsList = await _context.NewsLists
                //.Include(n => n.Cat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsList == null)
            {
                return NotFound();
            }

            return View(newsList);
        }

        // GET: NewsLists/Create
        public IActionResult Create(int? sid)
        {
            if (sid != null && sid != 0)
            {
                TempData["newsid"] = sid;
                ViewData["newsid"] = sid;
            }
            //ViewData["CatId"] = new SelectList(_context.NewsCats, "Id", "Id");
            return View();
        }

        // POST: NewsLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Txtname,Txtmore,Txtcontent,Txtdate,NewsCatId,Txtdatetime")] NewsList newsList)
        {
            if (ModelState.IsValid)
            {
                newsList.NewsCatId = Convert.ToInt32(TempData["newsid"]);
                _context.Add(newsList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new{ sid = TempData["newsid"] });
            }
            //ViewData["CatId"] = new SelectList(_context.NewsCats, "Id", "Id", newsList.CatId);
            return View(newsList);
        }

        // GET: NewsLists/Edit/5
        public async Task<IActionResult> Edit(int? id, int? sid)
        {
            if (sid != null && sid != 0)
            {
                TempData["newsid"] = sid;
                ViewData["newsid"] = sid;
            }
            if (id == null || _context.NewsLists == null)
            {
                return NotFound();
            }

            var newsList = await _context.NewsLists.FindAsync(id);

            ViewBag.Txtcontent = HttpUtility.HtmlDecode(newsList.Txtcontent);
            ViewBag.Txtmore = HttpUtility.HtmlDecode(newsList.Txtmore);
            if (newsList == null)
            {
                return NotFound();
            }
         
            return View(newsList);
        }

        // POST: NewsLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Txtname,Txtmore,Txtcontent,Txtdate,CatId,Txtdatetime")] NewsList newsList)
        {
            if (id != newsList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsList).Property(x => x.NewsCatId).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsListExists(newsList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { sid = TempData["newsid"] });
            }
         
            return View(newsList);
        }

        // GET: NewsLists/Delete/5
        public async Task<IActionResult> Delete(int? id,int? sid)
        {
            if (sid != null && sid != 0)
            {
                TempData["newsid"] = sid;
                ViewData["newsid"] = sid;
            }
            if (id == null || _context.NewsLists == null)
            {
                return NotFound();
            }

            var newsList = await _context.NewsLists
                //.Include(n => n.Cat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsList == null)
            {
                return NotFound();
            }

            return View(newsList);
        }

        // POST: NewsLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try{

           
            if (_context.NewsLists == null)
            {
                return Problem("Entity set 'edu_portal_dbContext.NewsLists'  is null.");
            }
            var newsList = await _context.NewsLists.FindAsync(id);
            if (newsList != null)
            {
                _context.NewsLists.Remove(newsList);
            }
            
            await _context.SaveChangesAsync();
            }catch (Exception ex) { }
            return RedirectToAction(nameof(Index), new { sid = TempData["newsid"] });
        }

        private bool NewsListExists(int id)
        {
          return (_context.NewsLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

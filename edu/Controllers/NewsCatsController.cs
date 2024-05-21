using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using edu.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using SQLitePCL;
using Microsoft.AspNetCore.Html;
using System.Web;

namespace edu.Controllers
{
    public class NewsCatsController : Controller
    {
        private readonly edu_portal_dbContext _context;

        public NewsCatsController(edu_portal_dbContext context)
        {
            _context = context;
        }

        public JsonResult All([DataSourceRequest] DataSourceRequest request)
        {
            var result = GetDirectory().ToTreeDataSourceResult(request,
                e => e.Id,
                e => e.Txtname,
                e => e
            );

            return Json(result);
        }
        private IEnumerable<NewsCat> GetDirectory()
        {
            return _context.NewsCats.ToList();
        }

        // GET: NewsCats
        public async Task<IActionResult> Index()
        {

            ViewData["menu"] = _context.NewsCats;
              return _context.NewsCats != null ? 
                          View(await _context.NewsCats.ToListAsync()) :
                          Problem("Entity set 'edu_portal_dbContext.NewsCats'  is null.");
        }
        public IActionResult Home()
        {
            return View();
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
        public async Task<IActionResult> Create([Bind("Id,Txtname,Vis,TxtOrd,Txttype,SubId,Txtmore,Txtcontent,TxtDate")] NewsCat newsCat)
        {
            if (ModelState.IsValid)
            {
            
                _context.Add(newsCat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsCat);
        }
        public IActionResult Createsub(int? sid)
        {
            if(sid!=null&&sid!=0)
            {
                TempData["sid"] = sid;
                ViewData["sid"] = sid;
            }

            return View();
        }

        // POST: NewsCats/Createsub
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Createsub(int sid, [Bind("Id,Txtname,Vis,TxtOrd,SubId,Txttype,Txtmore,Txtcontent,TxtDate")] NewsCat newsCat)
        {
            if (ModelState.IsValid)
            {
               
                newsCat.SubId = Convert.ToInt32(TempData["sid"]);
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
            var data = _context.NewsCats.FirstOrDefault(vs => vs.Id == id);
            ViewData["txtype"] = data.Txttype;
        
            var newsCat = await _context.NewsCats.FindAsync(id);
            ViewBag.Txtcontent = HttpUtility.HtmlDecode(newsCat.Txtcontent);
            ViewBag.Txtmore = HttpUtility.HtmlDecode(newsCat.Txtmore);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Txtname,Vis,TxtOrd,SubId,Txtmore,Txttype,Txtcontent,TxtDate")] NewsCat newsCat)
        {
            if (id != newsCat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsCat).Property(x => x.SubId).IsModified = false; 
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
            try { 
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
            }
            catch (Exception ex) { }
            return RedirectToAction(nameof(Index));
        }

        private bool NewsCatExists(int id)
        {
          return (_context.NewsCats?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

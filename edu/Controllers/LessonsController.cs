using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using edu.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Web;
using Kendo.Mvc.Extensions;
using System.Data;

namespace edu.Controllers
{
    public class LessonsController : Controller
    {
        private readonly edu_portal_dbContext _context;

        public LessonsController(edu_portal_dbContext context)
        {
            _context = context;
        }
       // GET: Lessons
     
        public IActionResult Index(string? sid)
        {
            var datalist = _context.Lessons.ToList();
            if (sid!=null)
            {
                datalist = _context.Lessons.Where(vs => vs.GId == Convert.ToInt32(sid)).ToList();
                HttpContext.Session.SetString("gid", sid);
                var gname = _context.GroupLessons.First(vs=>vs.Id==Convert.ToInt32(sid)).Name;
                HttpContext.Session.SetString("gname", gname);
                ViewBag.name = gname;
            }
            else
            {
                var gid = HttpContext.Session.GetString("gid");
                datalist = _context.Lessons.Where(vs => vs.GId == Convert.ToInt32(gid)).ToList();
                var gname = HttpContext.Session.GetString("gname");
                ViewBag.name = gname;

            }
           return View(datalist.ToList()) ;
        }

        public async Task<IActionResult> Indexstudent(int id)
        {
            if (id !=null)
            {
               // TempData["id"] = id;
              //  ViewBag.name = _context.GroupLessons.First(vs => vs.Id == Convert.ToInt32(TempData["id"])).Name.ToString();

            }
            return _context.Lessons != null ?
                          View(await _context.Lessons.Where(vs => vs.GId ==Convert.ToUInt32(id)).ToListAsync()) :
                        
                          //View(await _context.Lessons.ToListAsync()) :
                          Problem("Entity set 'edu_portal_dbContext.Lessons'  is null.");
        }

        public async Task<IActionResult> Indexstudentpub(int id)
        {
            if (id != null)
            {
                 TempData["id"] = id;
                 ViewBag.name = _context.GroupLessons.First(vs => vs.Id == Convert.ToInt32(TempData["id"])).Name.ToString();

            }
            return _context.Lessons != null ?
                          //   View(await _context.Lessons.Where(vs => vs.GId ==Convert.ToUInt32(_sid)).ToListAsync()) :
                          View(await _context.Lessons.Where(vs => vs.GId == Convert.ToUInt32(TempData["id"])).ToListAsync()) :

                        //  View(await _context.Lessons.ToListAsync()) :
                          Problem("Entity set 'edu_portal_dbContext.Lessons'  is null.");
        }

        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // GET: Lessons/Create
        public IActionResult Create()
        {
            if(TempData["id"]!=null)
            {
                ViewBag.name = _context.GroupLessons.First(vs => vs.Id == Convert.ToInt32(TempData["id"])).Name.ToString();

            }
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ViewMinut,Txtmore,Txtcontent,Txtwork,Txtvoice,Txtvideo,GId,TeacherId")] Lesson lesson)
        {

          

            if (ModelState.IsValid)
            {
                var gid = HttpContext.Session.GetString("gid");
                var id = User.FindFirst(ClaimTypes.Dns)?.Value;
                lesson.GId =Convert.ToInt32(gid);
                lesson.TeacherId =Convert.ToInt32(id);
                lesson.Txtvoice = "../../shared/UserFiles/images/" + User.FindFirst(ClaimTypes.Name)?.Value + "/"+ lesson.Txtvoice;
                lesson.Txtvideo = "../../shared/UserFiles/images/" + User.FindFirst(ClaimTypes.Name)?.Value + "/" + lesson.Txtvideo;

                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = TempData["id"] });
            }
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (TempData["id"] != null)
            {
                ViewBag.name = _context.GroupLessons.First(vs => vs.Id == Convert.ToInt32(TempData["id"])).Name.ToString();

            }
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesList = await _context.Lessons.FindAsync(id);
            ViewBag.Txtcontent = HttpUtility.HtmlDecode(lesList.Txtcontent);
            ViewBag.Txtmore = HttpUtility.HtmlDecode(lesList.Txtmore);
            ViewBag.Txtwork = HttpUtility.HtmlDecode(lesList.Txtwork);
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ViewMinut,Txtmore,Txtcontent,Txtwork,Txtvoice,Txtvideo,GId,TeacherId")] Lesson lesson)
        {
           


            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //   lesson.GId = Convert.ToInt32(TempData["id"]);

                    lesson.Txtvoice = "../../shared/UserFiles/images/" + User.FindFirst(ClaimTypes.Name)?.Value +"/"+ lesson.Txtvoice;
                    lesson.Txtvideo = "../../shared/UserFiles/images/" + User.FindFirst(ClaimTypes.Name)?.Value + "/" + lesson.Txtvideo;

                    _context.Update(lesson);
                    _context.Entry(lesson).Property(v => v.GId).IsModified = false;
                    _context.Entry(lesson).Property(v => v.TeacherId).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = TempData["id"] });
            }
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //if (TempData["id"] != null)
            //{
            //    ViewBag.name = _context.GroupLessons.First(vs => vs.Id == Convert.ToInt32(TempData["id"])).Name.ToString();

            //}

            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            if (_context.Lessons == null)
            {
                return Problem("Entity set 'edu_portal_dbContext.Lessons'  is null.");
            }
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = TempData["id"] });
        }

        private bool LessonExists(int id)
        {
          return (_context.Lessons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

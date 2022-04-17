using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace WebApplication1.Controllers
{
    public class TourismController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Tourism
        [Authorize]
        public async Task<ActionResult> Index(int? i)
        {
            List<Tourism> tourisms = await db.Tourism.ToListAsync();
            return View(tourisms.ToPagedList(i ?? 1, 5));
        }

        // GET: Tourism/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tourism tourism = await db.Tourism.FindAsync(id);
            if (tourism == null)
            {
                return HttpNotFound();
            }
            return View(tourism);
        }

        // GET: Tourism/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tourism/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Id,AttractionName,ImagePath,Description,Date,TourGuide")] Tourism tourism, HttpPostedFileBase ImageFile, DateTime date)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile!=null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    tourism.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                else
                {
                    tourism.ImagePath = "/Image/default.jpg";
                }

                tourism.Date = date;
                tourism.Creator = ControllerContext.HttpContext.User.Identity.Name;

                db.Tourism.Add(tourism);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tourism);
        }

        // GET: Tourism/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tourism tourism = await db.Tourism.FindAsync(id);
            if (tourism == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name==tourism.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin") 
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(tourism);
            }
            return RedirectToAction("Index");

        }

        // POST: Tourism/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AttractionName,ImagePath,Description,Date,TourGuide")] Tourism tourism, HttpPostedFileBase ImageFile, DateTime date)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    tourism.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                tourism.Date = date;
                db.Tourism.Add(tourism);
                db.Entry(tourism).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tourism);
        }

        // POST: TourGuide/Delete/
        [Authorize]
        public async Task<ActionResult> Delete(string ids)
        {
            List<string> idss = ids.Split(',').ToList();
            for (int i = 0; i < idss.Count - 1; i++)
            {
                Tourism tourism = await db.Tourism.FindAsync(Convert.ToInt32(idss.ElementAt(i)));
                if (!tourism.Creator.Equals(ControllerContext.HttpContext.User.Identity.Name) && !ControllerContext.HttpContext.User.IsInRole("Admin"))
                {
                    TempData["Message"] = "Invalid permissions!";
                    return RedirectToAction("Index");
                }
                foreach (var item in await db.Comment.Where(x => x.EventId == tourism.Id).ToListAsync())
                {
                    db.Comment.Remove(item);
                }
                db.Tourism.Remove(tourism);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

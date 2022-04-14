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
    public class SportEventsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: SportEvents
        [Authorize]
        public async Task<ActionResult> Index(int? i)
        {
            List<SportEvent> sportEvents = await db.SportEvent.ToListAsync();
            return View(sportEvents.ToPagedList(i??1,5));
        }

        // GET: SportEvents/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SportEvent sportEvent = await db.SportEvent.FindAsync(id);
            string eventName = sportEvent.GetType().Name;
            List<Comment> comments = await db.Comment.Where(x=>x.EventType==eventName).ToListAsync();
            if (sportEvent == null)
            {
                return HttpNotFound();
            }
            ViewModel viewModel = new ViewModel
            {
                SportEvent = sportEvent,
                Comment=comments
            };
            return View(viewModel);
        }

        // GET: SportEvents/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SportEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Id,EventName,ImagePath,Description,Date,Street")] SportEvent sportEvent, 
        HttpPostedFileBase ImageFile, DateTime date)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile!=null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    sportEvent.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                else
                {
                    sportEvent.ImagePath = "/Image/default.jpg";
                }

                sportEvent.Date = date;
                sportEvent.Creator = ControllerContext.HttpContext.User.Identity.Name;

                db.SportEvent.Add(sportEvent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sportEvent);
        }

        // GET: SportEvents/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SportEvent sportEvent = await db.SportEvent.FindAsync(id);
            if (sportEvent == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == sportEvent.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(sportEvent);
            }
            return RedirectToAction("Index");
        }

        // POST: SportEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EventName,ImagePath,Description,Date,Street")] SportEvent sportEvent  ,HttpPostedFileBase ImageFile, DateTime date)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    sportEvent.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                sportEvent.Date = date;
                db.Entry(sportEvent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sportEvent);
        }

        // GET: SportEvents/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SportEvent sportEvent = await db.SportEvent.FindAsync(id);
            if (sportEvent == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == sportEvent.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(sportEvent);
            }
            return RedirectToAction("Index");
        }

        // POST: SportEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SportEvent sportEvent = await db.SportEvent.FindAsync(id);
            foreach (var item in await db.Comment.Where(x => x.EventId == sportEvent.Id).ToListAsync())
            {
                db.Comment.Remove(item);
            }
            db.SportEvent.Remove(sportEvent);
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

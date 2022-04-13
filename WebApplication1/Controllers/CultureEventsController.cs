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
using System.Web.Security;
using PagedList.Mvc;

namespace WebApplication1.Controllers
{
    public class CultureEventsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: CultureEvents
        [Authorize]
        public async Task<ActionResult> Index(int? i)
        {
            List<CultureEvent> cultureEvents = await db.CultureEvent.ToListAsync();
            
            return View(cultureEvents.ToPagedList(i ?? 1, 5));
        }
        [Authorize]
        // GET: CultureEvents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureEvent cultureEvent = await db.CultureEvent.FindAsync(id);
            string eventName = cultureEvent.GetType().Name;
            List<Comment> comments = await db.Comment.Where(x => x.EventType == eventName).ToListAsync();
            if (cultureEvent == null)
            {
                return HttpNotFound();
            }
            ViewModel viewModel = new ViewModel();
            viewModel.CultureEvent = cultureEvent;
            viewModel.Comment = comments;

            return View(viewModel);
        }

        // GET: CultureEvents/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: CultureEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Id,EventName,ImagePath,Description,Date,PhoneNumber,EMail,Organizer,Street")] CultureEvent cultureEvent, HttpPostedFileBase ImageFile,DateTime date)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string extension = Path.GetExtension(ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                cultureEvent.ImagePath = "/Image/" + fileName;
                cultureEvent.Date = date;
                cultureEvent.Creator = ControllerContext.HttpContext.User.Identity.Name;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                ImageFile.SaveAs(fileName);
                db.CultureEvent.Add(cultureEvent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(cultureEvent);
        }

        // GET: CultureEvents/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureEvent cultureEvent = await db.CultureEvent.FindAsync(id);
            if (cultureEvent == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == cultureEvent.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(cultureEvent);
            }
            return RedirectToAction("Index");
        }

        // POST: CultureEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EventName,ImagePath,Description,Date,PhoneNumber,EMail,Organizer,Street")] CultureEvent cultureEvent, HttpPostedFileBase ImageFile, DateTime date)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    cultureEvent.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                cultureEvent.Date = date;
                db.Entry(cultureEvent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(cultureEvent);
        }

        // GET: CultureEvents/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CultureEvent cultureEvent = await db.CultureEvent.FindAsync(id);
            if (cultureEvent == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == cultureEvent.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(cultureEvent);
            }
            return RedirectToAction("Index");
        }

        // POST: CultureEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CultureEvent cultureEvent = await db.CultureEvent.FindAsync(id);
            foreach (var item in await db.Comment.Where(x => x.EventId == cultureEvent.Id).ToListAsync())
            {
                db.Comment.Remove(item);
            }
            db.CultureEvent.Remove(cultureEvent);
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

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
    public class SelfTaugthEventsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: SelfTaugthEvents
        [Authorize]
        public async Task<ActionResult> Index(int? i)
        {
            List<SelfTaugthEvent> selfTaugthEvents = await db.SelfTaugthEvent.ToListAsync();
            
            return View(selfTaugthEvents.ToPagedList(i??1,5));
        }

        // GET: SelfTaugthEvents/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SelfTaugthEvent selfTaugthEvent = await db.SelfTaugthEvent.FindAsync(id);
            string eventName = selfTaugthEvent.GetType().Name;
            List<Comment> comments = await db.Comment.Where(x => x.EventType == eventName).ToListAsync();
            if (selfTaugthEvent == null)
            {
                return HttpNotFound();
            }
            ViewModel viewModel = new ViewModel();
            viewModel.SelfTaugthEvent = selfTaugthEvent;
            viewModel.Comment = comments;

            return View(viewModel);
        }

        // GET: SelfTaugthEvents/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SelfTaugthEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Id,EventName,ImagePath,Description,Date,Activity,Street")] SelfTaugthEvent selfTaugthEvent, HttpPostedFileBase ImageFile, DateTime date,string description)
        {
            if (ModelState.IsValid)
            {

                string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string extension = Path.GetExtension(ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                selfTaugthEvent.ImagePath = "/Image/" + fileName;
                selfTaugthEvent.Date = date;
                selfTaugthEvent.Description = description;
                selfTaugthEvent.Creator = ControllerContext.HttpContext.User.Identity.Name;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                ImageFile.SaveAs(fileName);
                db.SelfTaugthEvent.Add(selfTaugthEvent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(selfTaugthEvent);
        }

        // GET: SelfTaugthEvents/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SelfTaugthEvent selfTaugthEvent = await db.SelfTaugthEvent.FindAsync(id);
            if (selfTaugthEvent == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == selfTaugthEvent.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(selfTaugthEvent);
            }
            return RedirectToAction("Index");
        }

        // POST: SelfTaugthEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EventName,ImagePath,Description,Date,Activity,Street")] SelfTaugthEvent selfTaugthEvent, HttpPostedFileBase ImageFile, DateTime date,string description)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile!=null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    selfTaugthEvent.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                selfTaugthEvent.Date = date;
                selfTaugthEvent.Description = description;

                db.Entry(selfTaugthEvent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(selfTaugthEvent);
        }

        // GET: SelfTaugthEvents/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SelfTaugthEvent selfTaugthEvent = await db.SelfTaugthEvent.FindAsync(id);
            if (selfTaugthEvent == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == selfTaugthEvent.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(selfTaugthEvent);
            }
            return RedirectToAction("Index");
        }

        // POST: SelfTaugthEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SelfTaugthEvent selfTaugthEvent = await db.SelfTaugthEvent.FindAsync(id);
            foreach (var item in await db.Comment.Where(x => x.EventId == selfTaugthEvent.Id).ToListAsync())
            {
                db.Comment.Remove(item);
            }
            db.SelfTaugthEvent.Remove(selfTaugthEvent);
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

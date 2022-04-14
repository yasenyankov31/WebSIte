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

namespace WebApplication1.Controllers
{
    public class SearchForVolunteersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: SearchForVolunteers
        [Authorize]
        public async Task<ActionResult> Index(int? i)
        {
            List<SearchForVolunteer> searchForVolunteers = await db.SearchForVolunteer.ToListAsync();

            return View(searchForVolunteers.ToPagedList(i ?? 1, 5));
        }

        // GET: SearchForVolunteers/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id,string username)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchForVolunteer searchForVolunteer = await db.SearchForVolunteer.FindAsync(id);
            if (searchForVolunteer == null)
            {
                return HttpNotFound();
            }
            ViewModel viewModel = new ViewModel();
            viewModel.SearchForVolunteer = searchForVolunteer;
            Volunteers volunteers = await db.Volunteers.Where(x => x.Username == username).FirstOrDefaultAsync();

            if (volunteers==null)
            {
                volunteers = new Volunteers
                {
                    Username = ""
                };
            }
            viewModel.VolunteersList = await db.Volunteers.Where(x=>x.EventId==id).ToListAsync();
            viewModel.Volunteers = volunteers;
            return View(viewModel);
        }

        // GET: SearchForVolunteers/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SearchForVolunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Id,EventName,ImagePath,Description,Date")] SearchForVolunteer searchForVolunteer, HttpPostedFileBase ImageFile, DateTime date)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile!=null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    searchForVolunteer.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                else
                {
                    searchForVolunteer.ImagePath = "/Image/default.jpg";
                }

                searchForVolunteer.Date = date;
                searchForVolunteer.Creator = ControllerContext.HttpContext.User.Identity.Name; 
                db.SearchForVolunteer.Add(searchForVolunteer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(searchForVolunteer);
        }

        // GET: SearchForVolunteers/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchForVolunteer searchForVolunteer = await db.SearchForVolunteer.FindAsync(id);
            if (searchForVolunteer == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == searchForVolunteer.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(searchForVolunteer);
            }
            return RedirectToAction("Index");
        }

        // POST: SearchForVolunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EventName,ImagePath,Description,Date")] SearchForVolunteer searchForVolunteer, HttpPostedFileBase ImageFile, DateTime date)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    searchForVolunteer.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    ImageFile.SaveAs(fileName);
                }
                searchForVolunteer.Date = date;
                db.Entry(searchForVolunteer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(searchForVolunteer);
        }

        // GET: SearchForVolunteers/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchForVolunteer searchForVolunteer = await db.SearchForVolunteer.FindAsync(id);
            if (searchForVolunteer == null)
            {
                return HttpNotFound();
            }
            if (ControllerContext.HttpContext.User.Identity.Name == searchForVolunteer.Creator
                || ControllerContext.HttpContext.User.IsInRole("Admin")
                || ControllerContext.HttpContext.User.IsInRole("Coadmin"))
            {
                return View(searchForVolunteer);
            }
            return RedirectToAction("Index");
        }

        // POST: SearchForVolunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SearchForVolunteer searchForVolunteer = await db.SearchForVolunteer.FindAsync(id);
            foreach (var item in await db.Volunteers.Where(x=>x.EventId==id).ToListAsync())
            {
                db.Volunteers.Remove(item);
            }
            db.SearchForVolunteer.Remove(searchForVolunteer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<ActionResult> AddVolunteer(int eventId,string username,string phonenumber)
        {
            Volunteers volunteers = new Volunteers();
            volunteers.EventId = eventId;
            volunteers.Username = username;
            volunteers.PhoneNumber = phonenumber;

            db.Volunteers.Add(volunteers);
            await db.SaveChangesAsync();

            return RedirectToAction("Details",new {id=eventId,username=username});
        }
        [Authorize]
        public async Task<ActionResult> RemoveVolunteer(int eventId, string username)
        {
            Volunteers volunteers = await db.Volunteers.Where(x=>x.Username==username).FirstOrDefaultAsync();
            db.Volunteers.Remove(volunteers);
            await db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = eventId, username = username });
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

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

namespace WebApplication1.Controllers
{
    public class CommentsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string comment)
        {
            string username = Request.Form["Username"];
            string eventId = Request.Form["EventId"];
            string eventName = Request.Form["EventName"];
            DateTime now = DateTime.Now;
            if (ModelState.IsValid)
            {
                Comment cm = new Comment
                {
                    EventId =Convert.ToInt32(eventId),
                    Username = username,
                    Date = now,
                    EventType=eventName,
                    Comments=comment

                };

                db.Comment.Add(cm);
                await db.SaveChangesAsync();
                return RedirectToAction("Details/"+eventId,eventName+"s");
            }

            return View(comment);
        }


        // POST: Comments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            string eventId = Request.Form["EventId"];
            string eventName = Request.Form["EventName"];
            Comment comment = await db.Comment.FindAsync(id);
            db.Comment.Remove(comment);
            await db.SaveChangesAsync();
            return RedirectToAction("Details/" + eventId, eventName + "s");
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

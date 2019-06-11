using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCG.Models;

namespace SCG.Controllers
{
  public class RsvpsController : Controller
  {
    private Entities db = new Entities();

    // GET: Rsvps
    public async Task<ActionResult> Index(int? id)
    {
      var rsvps = db.Rsvps.Include(r => r.Attendee).Where(r => r.AttendeeID == id).OrderByDescending(r => r.Year);
      ViewData["FormID"] = id;
      return PartialView(await rsvps.ToListAsync());
    }

    // GET: Rsvps/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Rsvp rsvp = await db.Rsvps.FindAsync(id);
      if (rsvp == null)
      {
        return HttpNotFound();
      }
      return View(rsvp);
    }

    // GET: Rsvps/Create
    public ActionResult Create()
    {
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName");
      return View();
    }

    // POST: Rsvps/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,AttendeeID,Year,Attending,Attended")] Rsvp rsvp)
    {
      if (ModelState.IsValid)
      {
        Attendee attendee = db.Attendees.Find(rsvp.AttendeeID);
        attendee.UpdatedAt = DateTime.Now;
        Project project = db.Projects.Find(attendee.ProjectID);
        project.UpdatedAt = DateTime.Now;
        rsvp.CreatedAt = DateTime.Now;
        rsvp.UpdatedAt = DateTime.Now;
        db.Entry(attendee).Property(a => a.UpdatedAt).IsModified = true;
        db.Entry(project).Property(p => p.UpdatedAt).IsModified = true;
        db.Rsvps.Add(rsvp);
        await db.SaveChangesAsync();
        return null;
      }

      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", rsvp.AttendeeID);
      return View(rsvp);
    }

    // GET: Rsvps/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Rsvp rsvp = await db.Rsvps.FindAsync(id);
      if (rsvp == null)
      {
        return HttpNotFound();
      }
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", rsvp.AttendeeID);
      return View(rsvp);
    }

    // POST: Rsvps/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,AttendeeID,Attended")] Rsvp rsvp)
    {
      if (ModelState.IsValid)
      {
        db.Entry(rsvp).State = EntityState.Modified;
        db.Entry(rsvp).Property(r => r.AttendeeID).IsModified = false;
        db.Entry(rsvp).Property(r => r.Year).IsModified = false;
        db.Entry(rsvp).Property(r => r.Attending).IsModified = false;
        db.Entry(rsvp).Property(r => r.CreatedAt).IsModified = false;
        rsvp.UpdatedAt = DateTime.Now;

        //Update Attendee and Project
        Attendee attendee = db.Attendees.Find(rsvp.AttendeeID);
        attendee.UpdatedAt = DateTime.Now;
        Project project = db.Projects.Find(attendee.ProjectID);
        project.UpdatedAt = DateTime.Now;
        db.Entry(attendee).Property(a => a.UpdatedAt).IsModified = true;
        db.Entry(project).Property(p => p.UpdatedAt).IsModified = true;

        await db.SaveChangesAsync();
        return null;
      }
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", rsvp.AttendeeID);
      return View(rsvp);
    }

    // GET: Rsvps/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Rsvp rsvp = await db.Rsvps.FindAsync(id);
      if (rsvp == null)
      {
        return HttpNotFound();
      }
      return View(rsvp);
    }

    // POST: Rsvps/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Rsvp rsvp = await db.Rsvps.FindAsync(id);
      db.Rsvps.Remove(rsvp);
      await db.SaveChangesAsync();
      return null;
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

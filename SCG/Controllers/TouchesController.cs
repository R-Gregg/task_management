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
  public class TouchesController : Controller
  {
    private Entities db = new Entities();

    // GET: Touches
    public async Task<ActionResult> Index(int? id)
    {
      var touches = db.Touches.Include(t => t.User).Include(t => t.Attendee).Where(t => t.AttendeeID == id).OrderByDescending(t => t.TouchedAt);
      ViewData["FormID"] = id;
      return PartialView(await touches.ToListAsync());
    }

    // GET: Touches/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Touch touch = await db.Touches.FindAsync(id);
      if (touch == null)
      {
        return HttpNotFound();
      }
      return View(touch);
    }

    // GET: Touches/Create
    public ActionResult Create()
    {
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName");
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName");
      return View();
    }

    // POST: Touches/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Description,AttendeeID,TouchedAt")] Touch touch)
    {
      if (ModelState.IsValid)
      {
        touch.CreatedBy = User.Identity.Name;
        touch.CreatedAt = DateTime.Now;
        touch.UpdatedAt = DateTime.Now;
        Attendee attendee = db.Attendees.Find(touch.AttendeeID);
        attendee.UpdatedAt = DateTime.Now;
        Project project = db.Projects.Find(attendee.ProjectID);
        project.UpdatedAt = DateTime.Now;
        db.Entry(attendee).Property(a => a.UpdatedAt).IsModified = true;
        db.Entry(project).Property(p => p.UpdatedAt).IsModified = true;
        db.Touches.Add(touch);
        await db.SaveChangesAsync();
        return null;
      }

      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", touch.CreatedBy);
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", touch.AttendeeID);
      return View(touch);
    }

    // GET: Touches/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Touch touch = await db.Touches.FindAsync(id);
      if (touch == null)
      {
        return HttpNotFound();
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", touch.CreatedBy);
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", touch.AttendeeID);
      return View(touch);
    }

    // POST: Touches/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Description,TouchedAt")] Touch touch)
    {
      if (ModelState.IsValid)
      {
        Attendee attendee = db.Attendees.Find(touch.AttendeeID);
        Project project = db.Projects.Find(attendee.ProjectID);
        attendee.UpdatedAt = DateTime.Now;
        project.UpdatedAt = DateTime.Now;
        touch.UpdatedAt = DateTime.Now;
        db.Entry(touch).State = EntityState.Modified;
        db.Entry(touch).Property(t => t.CreatedBy).IsModified = false;
        db.Entry(touch).Property(t => t.AttendeeID).IsModified = false;
        db.Entry(touch).Property(t => t.CreatedAt).IsModified = false;
        db.Entry(touch).Property(t => t.UpdatedAt).IsModified = false;

        await db.SaveChangesAsync();
        return null;
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", touch.CreatedBy);
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", touch.AttendeeID);
      return View(touch);
    }

    // GET: Touches/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Touch touch = await db.Touches.FindAsync(id);
      if (touch == null)
      {
        return HttpNotFound();
      }
      return View(touch);
    }

    // POST: Touches/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Touch touch = await db.Touches.FindAsync(id);
      db.Touches.Remove(touch);
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

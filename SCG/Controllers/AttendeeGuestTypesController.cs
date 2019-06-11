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
  public class AttendeeGuestTypesController : Controller
  {
    private Entities db = new Entities();

    // GET: AttendeeGuestTypes
    public async Task<ActionResult> Index()
    {
      var attendeeGuestTypes = db.AttendeeGuestTypes.Include(a => a.Attendee).Include(a => a.GuestType);
      return View(await attendeeGuestTypes.ToListAsync());
    }

    // GET: AttendeeGuestTypes/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AttendeeGuestType attendeeGuestType = await db.AttendeeGuestTypes.FindAsync(id);
      if (attendeeGuestType == null)
      {
        return HttpNotFound();
      }
      return View(attendeeGuestType);
    }

    // GET: AttendeeGuestTypes/Create
    public ActionResult Create(int? id)
    {
      var attendee = db.Attendees.Find(id);
      var used = db.AttendeeGuestTypes.Where(a => a.AttendeeID == id).Select(c => c.GuestTypeID);
      var types = db.GuestTypes.Where(u => !used.Contains(u.Id) && u.ProjectID == attendee.ProjectID).OrderBy(u => u.Name);
      var assigned = db.GuestTypes.Where(u => used.Contains(u.Id));
      ViewData["AssignedList"] = assigned;
      ViewData["FormID"] = id;
      ViewBag.GuestTypeID = new SelectList(types, "Id", "Name");
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName");
      return PartialView();
    }

    // POST: AttendeeGuestTypes/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,AttendeeID,GuestTypeID,CreatedAt,UpdatedAt")] AttendeeGuestType attendeeGuestType)
    {
      if (ModelState.IsValid)
      {
        db.AttendeeGuestTypes.Add(attendeeGuestType);
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }

      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", attendeeGuestType.AttendeeID);
      ViewBag.GuestTypeID = new SelectList(db.GuestTypes, "Id", "Name", attendeeGuestType.GuestTypeID);
      return View(attendeeGuestType);
    }

    // GET: AttendeeGuestTypes/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AttendeeGuestType attendeeGuestType = await db.AttendeeGuestTypes.FindAsync(id);
      if (attendeeGuestType == null)
      {
        return HttpNotFound();
      }
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", attendeeGuestType.AttendeeID);
      ViewBag.GuestTypeID = new SelectList(db.GuestTypes, "Id", "Name", attendeeGuestType.GuestTypeID);
      return View(attendeeGuestType);
    }

    // POST: AttendeeGuestTypes/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,AttendeeID,GuestTypeID,CreatedAt,UpdatedAt")] AttendeeGuestType attendeeGuestType)
    {
      if (ModelState.IsValid)
      {
        db.Entry(attendeeGuestType).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", attendeeGuestType.AttendeeID);
      ViewBag.GuestTypeID = new SelectList(db.GuestTypes, "Id", "Name", attendeeGuestType.GuestTypeID);
      return View(attendeeGuestType);
    }

    // GET: AttendeeGuestTypes/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AttendeeGuestType attendeeGuestType = await db.AttendeeGuestTypes.FindAsync(id);
      if (attendeeGuestType == null)
      {
        return HttpNotFound();
      }
      return View(attendeeGuestType);
    }

    // POST: AttendeeGuestTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      AttendeeGuestType attendeeGuestType = await db.AttendeeGuestTypes.FindAsync(id);
      db.AttendeeGuestTypes.Remove(attendeeGuestType);
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

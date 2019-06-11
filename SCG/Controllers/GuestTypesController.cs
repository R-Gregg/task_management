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
  public class GuestTypesController : Controller
  {
    private Entities db = new Entities();

    // GET: GuestTypes
    public async Task<ActionResult> Index()
    {
      var guestTypes = db.GuestTypes.Include(g => g.Project);
      return View(await guestTypes.ToListAsync());
    }

    // GET: GuestTypes/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      GuestType guestType = await db.GuestTypes.FindAsync(id);
      if (guestType == null)
      {
        return HttpNotFound();
      }
      return View(guestType);
    }

    // GET: GuestTypes/Create
    public ActionResult Create()
    {
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      return View();
    }

    // POST: GuestTypes/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> Create([Bind(Include = "Id,Name,ProjectID,Sticky")] GuestType guestType)
    {
      if (ModelState.IsValid)
      {
        guestType.CreatedAt = DateTime.Now;
        guestType.UpdatedAt = DateTime.Now;
        db.GuestTypes.Add(guestType);
        await db.SaveChangesAsync();
        return null;
      }
     
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", guestType.ProjectID);
      return View(guestType);
    }

    // GET: GuestTypes/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      GuestType guestType = await db.GuestTypes.FindAsync(id);
      if (guestType == null)
      {
        return HttpNotFound();
      }
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", guestType.ProjectID);
      return View(guestType);
    }

    // POST: GuestTypes/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ProjectID")] GuestType guestType)
    {
      if (ModelState.IsValid)
      {
        db.Entry(guestType).State = EntityState.Modified;
        db.Entry(guestType).Property(g => g.Sticky).IsModified = false;
        db.Entry(guestType).Property(g => g.CreatedAt).IsModified = false;
        guestType.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return null;
      }
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", guestType.ProjectID);
      return View(guestType);
    }

    // GET: GuestTypes/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      GuestType guestType = await db.GuestTypes.FindAsync(id);
      if (guestType == null)
      {
        return HttpNotFound();
      }
      return View(guestType);
    }

    // POST: GuestTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      GuestType guestType = await db.GuestTypes.FindAsync(id);
      db.AttendeeGuestTypes.RemoveRange(guestType.AttendeeGuestTypes);
      db.GuestTypes.Remove(guestType);
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

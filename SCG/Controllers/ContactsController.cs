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
  public class ContactsController : Controller
  {
    private Entities db = new Entities();

    // GET: Contacts
    public async Task<ActionResult> Index(int? id)
    {
      var contacts = db.Contacts.Include(c => c.User).Include(c => c.Attendee).Where(c => c.AttendeeID == id);
      ViewData["FormID"] = id;
      return PartialView(await contacts.ToListAsync());
    }

    // GET: Contacts/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Contact contact = await db.Contacts.FindAsync(id);
      if (contact == null)
      {
        return HttpNotFound();
      }
      return View(contact);
    }

    // GET: Contacts/Create
    public ActionResult Create(int? id)
    {
      var attendee = db.Attendees.Find(id);
      var project = db.Projects.Find(attendee.ProjectID);
      var shared = project.SharedProjects.Select(s => s.UserID);
      var used = db.Contacts.Where(c => c.AttendeeID == id).Select(c => c.UserID);
      var users = db.Users.Where(u => !used.Contains(u.Id) && (shared.Contains(u.Id) || u.Id == project.CreatedBy)).OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
      var assigned = db.Users.Where(u => used.Contains(u.Id));
      ViewData["AssignedList"] = assigned;
      ViewData["FormID"] = id;
      ViewBag.UserID = new SelectList(users, "Id", "FullName");
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName");
      return PartialView();
    }

    // POST: Contacts/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,AttendeeID,UserID,CreatedAt,UpdatedAt")] Contact contact)
    {
      if (ModelState.IsValid)
      {
        db.Contacts.Add(contact);
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }

      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", contact.UserID);
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", contact.AttendeeID);
      return null;
    }

    // GET: Contacts/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Contact contact = await db.Contacts.FindAsync(id);
      if (contact == null)
      {
        return HttpNotFound();
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", contact.UserID);
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", contact.AttendeeID);
      return View(contact);
    }

    // POST: Contacts/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,AttendeeID,UserID,CreatedAt,UpdatedAt")] Contact contact)
    {
      if (ModelState.IsValid)
      {
        db.Entry(contact).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", contact.UserID);
      ViewBag.AttendeeID = new SelectList(db.Attendees, "Id", "FirstName", contact.AttendeeID);
      return View(contact);
    }

    // GET: Contacts/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Contact contact = await db.Contacts.FindAsync(id);
      if (contact == null)
      {
        return HttpNotFound();
      }
      return View(contact);
    }

    // POST: Contacts/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Contact contact = await db.Contacts.FindAsync(id);
      db.Contacts.Remove(contact);
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

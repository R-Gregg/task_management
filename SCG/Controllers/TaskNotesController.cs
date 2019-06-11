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
  public class TaskNotesController : Controller
  {
    private Entities db = new Entities();

    // GET: TaskNotes
    public async Task<ActionResult> Index(int? id)
    {
      var task = db.Tasks.Find(id);
      var taskNotes = db.TaskNotes.Where(t => t.TaskID == task.Id).Include(t => t.User).Include(t => t.Task).OrderByDescending(t => t.CreatedAt);
      ViewData["TaskID"] = task.Id;
      return PartialView(await taskNotes.ToListAsync());
    }

    // GET: TaskNotes/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TaskNote taskNote = await db.TaskNotes.FindAsync(id);
      if (taskNote == null)
      {
        return HttpNotFound();
      }
      return View(taskNote);
    }

    // GET: TaskNotes/Create
    public ActionResult Create()
    {
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName");
      ViewBag.TaskID = new SelectList(db.Tasks, "Id", "Description");
      return View();
    }

    // POST: TaskNotes/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,TaskID,Description,Sticky")] TaskNote taskNote)
    {
      if (ModelState.IsValid)
      {
        taskNote.CreatedBy = User.Identity.Name;
        taskNote.CreatedAt = DateTime.Now;
        taskNote.UpdatedAt = DateTime.Now;
        db.TaskNotes.Add(taskNote);
        await db.SaveChangesAsync();
        return null;
      }

      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", taskNote.CreatedBy);
      ViewBag.TaskID = new SelectList(db.Tasks, "Id", "Description", taskNote.TaskID);
      return View(taskNote);
    }

    // GET: TaskNotes/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TaskNote taskNote = await db.TaskNotes.FindAsync(id);
      if (taskNote == null)
      {
        return HttpNotFound();
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", taskNote.CreatedBy);
      ViewBag.TaskID = new SelectList(db.Tasks, "Id", "Description", taskNote.TaskID);
      return View(taskNote);
    }

    // POST: TaskNotes/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Description,Sticky")] TaskNote taskNote)
    {
      if (ModelState.IsValid)
      {
        db.Entry(taskNote).State = System.Data.Entity.EntityState.Modified;
        db.Entry(taskNote).Property(t => t.TaskID).IsModified = false;
        db.Entry(taskNote).Property(t => t.CreatedBy).IsModified = false;
        db.Entry(taskNote).Property(t => t.CreatedAt).IsModified = false;
        taskNote.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", taskNote.CreatedBy);
      ViewBag.TaskID = new SelectList(db.Tasks, "Id", "Description", taskNote.TaskID);
      return View(taskNote);
    }

    // GET: TaskNotes/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TaskNote taskNote = await db.TaskNotes.FindAsync(id);
      if (taskNote == null)
      {
        return HttpNotFound();
      }
      return View(taskNote);
    }

    // POST: TaskNotes/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      TaskNote taskNote = await db.TaskNotes.FindAsync(id);
      db.TaskNotes.Remove(taskNote);
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

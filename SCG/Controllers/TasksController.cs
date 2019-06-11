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
  public class TasksController : Controller
  {
    private Entities db = new Entities();

    // GET: Tasks
    public async Task<ActionResult> Index(int? id)
    {
      Session["project"] = id;
      Session["project_view"] = "Tasks";
      var tasks = db.Tasks.Where(t => t.ProjectID == id).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Type).OrderBy(t => t.Order);
      var project = db.Projects.Find(id);
      ViewBag.Project = project;
      ViewData["Owner"] = project.User.Id == User.Identity.Name ? "True" : "False";
      ViewData["Schedule"] = project.Schedule == true ? "True" : "False";
      ViewData["Event"] = project.Event == true ? "True" : "False";
      return PartialView(await tasks.ToListAsync());
    }

    // GET: Tasks/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Models.Task task = await db.Tasks.FindAsync(id);
      if (task == null)
      {
        return HttpNotFound();
      }
      return View(task);
    }

    // GET: Tasks/Create
    public ActionResult Create()
    {
      var project = db.Projects.Find(Session["project"]);
      var shared = project.SharedProjects.Select(s => s.UserID);
      ViewBag.Project = project;
      ViewBag.PriorityID = new SelectList(project.Priorities, "Id", "Name");
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      ViewBag.TypeID = new SelectList(project.Types, "Id", "Name");
      ViewBag.AssignedTo = new SelectList(db.Users.Where(u => shared.Contains(u.Id) || u.Id == project.CreatedBy).OrderBy(u => u.FirstName), "Id", "FullName");
      return PartialView();
    }

    // POST: Tasks/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Description,PriorityID,TypeID,AssignedTo,DueDate")] Models.Task task)
    {
      if (ModelState.IsValid)
      {
        var project = await db.Projects.FindAsync(Session["project"]);
        task.ProjectID = project.Id;
        task.Order = project.Tasks.Count;
        task.CreatedBy = User.Identity.Name;
        task.CreatedAt = DateTime.Now;
        task.UpdatedAt = DateTime.Now;
        db.Tasks.Add(task);
        project.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }

      ViewBag.PriorityID = new SelectList(task.Project.Priorities, "Id", "Name");
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      ViewBag.TypeID = new SelectList(task.Project.Types, "Id", "Name");
      return RedirectToAction("Index", "Projects");
    }

    // GET: Tasks/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Models.Task task = await db.Tasks.FindAsync(id);
      if (task == null)
      {
        return HttpNotFound();
      }
      var shared = task.Project.SharedProjects.Select(s => s.UserID);
      ViewBag.PriorityID = new SelectList(task.Project.Priorities, "Id", "Name", task.PriorityID);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      ViewBag.TypeID = new SelectList(task.Project.Types, "Id", "Name", task.TypeID);
      ViewBag.AssignedTo = new SelectList(db.Users.Where(u => shared.Contains(u.Id) || u.Id == task.Project.CreatedBy).OrderBy(u => u.FirstName), "Id", "FullName", task.AssignedTo);
      return PartialView(task);
    }

    // POST: Tasks/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Description,PriorityID,TypeID,AssignedTo,DueDate")] Models.Task task)
    {
      if (ModelState.IsValid)
      {
        var project = await db.Projects.FindAsync(Session["project"]);
        db.Entry(task).State = EntityState.Modified;
        db.Entry(task).Property(t => t.ProjectID).IsModified = false;
        db.Entry(task).Property(t => t.Order).IsModified = false;
        db.Entry(task).Property(t => t.CreatedBy).IsModified = false;
        db.Entry(task).Property(t => t.CreatedAt).IsModified = false;
        task.UpdatedAt = DateTime.Now;
        project.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }
      ViewBag.PriorityID = new SelectList(task.Project.Priorities, "Id", "Name");
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      ViewBag.TypeID = new SelectList(task.Project.Types, "Id", "Name");
      return RedirectToAction("Index", "Projects");
    }

    public async Task<ActionResult> RowEdit(int id)
    {
      Models.Task task = await db.Tasks.FindAsync(id);
      if (task == null)
      {
        return HttpNotFound();
      }

      var shared = task.Project.SharedProjects.Select(s => s.UserID);
      ViewBag.PriorityID = new SelectList(task.Project.Priorities, "Id", "Name", task.PriorityID);
      ViewBag.TypeID = new SelectList(task.Project.Types, "Id", "Name", task.TypeID);
      ViewBag.AssignedTo = new SelectList(db.Users.Where(u => shared.Contains(u.Id) || u.Id == task.Project.CreatedBy).OrderBy(u => u.FirstName), "Id", "FullName", task.AssignedTo);

      return PartialView(task);
    }

    [HttpPost]
    public ActionResult RowEdit([Bind(Include = "Id,Description,PriorityID,TypeID,AssignedTo,DueDate")] Models.Task task)
    {
      if (ModelState.IsValid)
      {
        var project = db.Projects.Find(Session["project"]);
        db.Tasks.Add(task);
        var entry = db.Entry(task);
        entry.State = EntityState.Unchanged;
        entry.Property(p => p.Description).IsModified = true;
        entry.Property(p => p.PriorityID).IsModified = true;
        entry.Property(p => p.TypeID).IsModified = true;
        entry.Property(p => p.AssignedTo).IsModified = true;
        entry.Property(p => p.DueDate).IsModified = true;
        task.UpdatedAt = DateTime.Now;
        project.UpdatedAt = DateTime.Now;
        db.SaveChanges();
        return new HttpStatusCodeResult(200);
      }
      return null;
    }

    public async Task<ActionResult> ShowRow(int id)
    {
      Models.Task task = await db.Tasks.FindAsync(id);
      if (task == null)
      {
        return HttpNotFound();
      }

      return PartialView(task);
    }

    [HttpPost]
    public async Task<ActionResult> OrderEdit([Bind(Include = "Id,Order")] Models.Task task)
    {
      if (ModelState.IsValid)
      {
        db.Tasks.Add(task);
        var entry = db.Entry(task);
        entry.State = EntityState.Unchanged;
        entry.Property(p => p.Order).IsModified = true;
        await db.SaveChangesAsync();
      }
      return null;
    }

    // GET: Tasks/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Models.Task task = await db.Tasks.FindAsync(id);
      if (task == null)
      {
        return HttpNotFound();
      }
      return PartialView(task);
    }

    // POST: Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Models.Task task = await db.Tasks.FindAsync(id);
      db.TaskNotes.RemoveRange(task.TaskNotes);
      db.Tasks.Remove(task);
      await db.SaveChangesAsync();
      return RedirectToAction("Index", "Projects");
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

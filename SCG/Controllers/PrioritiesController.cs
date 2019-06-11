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
  public class PrioritiesController : Controller
  {
    private Entities db = new Entities();

    // GET: Priorities
    public async Task<ActionResult> Index()
    {
      var priorities = db.Priorities.Include(p => p.Project);
      return View(await priorities.ToListAsync());
    }

    // GET: Priorities/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Priority priority = await db.Priorities.FindAsync(id);
      if (priority == null)
      {
        return HttpNotFound();
      }
      return View(priority);
    }

    // GET: Priorities/Create
    public ActionResult Create()
    {
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      return View();
    }

    // POST: Priorities/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> Create([Bind(Include = "Id,Name,ProjectID")] Priority priority)
    {
      if (ModelState.IsValid)
      {
        priority.CreatedAt = DateTime.Now;
        priority.UpdatedAt = DateTime.Now;
        db.Priorities.Add(priority);
        await db.SaveChangesAsync();
        return null;
      }

      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", priority.ProjectID);
      return View(priority);
    }

    // GET: Priorities/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Priority priority = await db.Priorities.FindAsync(id);
      if (priority == null)
      {
        return HttpNotFound();
      }
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", priority.ProjectID);
      return View(priority);
    }

    // POST: Priorities/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ProjectID")] Priority priority)
    {
      if (ModelState.IsValid)
      {
        db.Entry(priority).State = System.Data.Entity.EntityState.Modified;
        db.Entry(priority).Property(p => p.Color).IsModified = false;
        db.Entry(priority).Property(p => p.Sticky).IsModified = false;
        db.Entry(priority).Property(p => p.CreatedAt).IsModified = false;
        priority.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return null;
      }
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", priority.ProjectID);
      return View(priority);
    }

    // GET: Priorities/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Priority priority = await db.Priorities.FindAsync(id);
      if (priority == null)
      {
        return HttpNotFound();
      }
      return View(priority);
    }

    // POST: Priorities/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Priority priority = await db.Priorities.FindAsync(id);
      var project = priority.Project;
      var sticky = project.Priorities.Where(t => t.Sticky == true).First().Id;
      var tasks = project.Tasks.Where(t => t.PriorityID == id).ToList();
      tasks.ForEach(t => t.PriorityID = sticky);
      db.Priorities.Remove(priority);
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

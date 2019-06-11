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
  public class SharedProjectsController : Controller
  {
    private Entities db = new Entities();

    // GET: SharedProjects
    public async Task<ActionResult> Index()
    {
      var sharedProjects = db.SharedProjects.Include(s => s.User).Include(s => s.Project);
      return View(await sharedProjects.ToListAsync());
    }

    // GET: SharedProjects/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      SharedProject sharedProject = await db.SharedProjects.FindAsync(id);
      if (sharedProject == null)
      {
        return HttpNotFound();
      }
      return View(sharedProject);
    }

    // GET: SharedProjects/Create
    public ActionResult Create()
    {
      ViewBag.UserID = new SelectList(db.Users, "Id", "Email");
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      return View();
    }

    // POST: SharedProjects/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> Create([Bind(Include = "Id,UserID,ProjectID")] SharedProject sharedProject)
    {
      if (ModelState.IsValid)
      {
        sharedProject.CreatedAt = DateTime.Now;
        sharedProject.UpdatedAt = DateTime.Now;
        db.SharedProjects.Add(sharedProject);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.UserID = new SelectList(db.Users, "Id", "Email", sharedProject.UserID);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", sharedProject.ProjectID);
      return View(sharedProject);
    }

    // GET: SharedProjects/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      SharedProject sharedProject = await db.SharedProjects.FindAsync(id);
      if (sharedProject == null)
      {
        return HttpNotFound();
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "Email", sharedProject.UserID);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", sharedProject.ProjectID);
      return View(sharedProject);
    }

    // POST: SharedProjects/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,UserID,ProjectID,CreatedAt,UpdatedAt")] SharedProject sharedProject)
    {
      if (ModelState.IsValid)
      {
        db.Entry(sharedProject).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "Email", sharedProject.UserID);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", sharedProject.ProjectID);
      return View(sharedProject);
    }

    // GET: SharedProjects/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      SharedProject sharedProject = await db.SharedProjects.FindAsync(id);
      if (sharedProject == null)
      {
        return HttpNotFound();
      }
      return View(sharedProject);
    }

    // POST: SharedProjects/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      SharedProject sharedProject = await db.SharedProjects.FindAsync(id);
      db.SharedProjects.Remove(sharedProject);
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

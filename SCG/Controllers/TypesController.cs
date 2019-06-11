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
  public class TypesController : Controller
  {
    private Entities db = new Entities();

    // GET: Types
    public async Task<ActionResult> Index()
    {
      var types = db.Types.Include(t => t.Project);
      return View(await types.ToListAsync());
    }

    // GET: Types/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Models.Type type = await db.Types.FindAsync(id);
      if (type == null)
      {
        return HttpNotFound();
      }
      return View(type);
    }

    // GET: Types/Create
    public ActionResult Create()
    {
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      return View();
    }

    // POST: Types/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> Create([Bind(Include = "Id,Name,ProjectID,Color")] Models.Type type)
    {
      if (ModelState.IsValid)
      {
        type.CreatedAt = DateTime.Now;
        type.UpdatedAt = DateTime.Now;
        db.Types.Add(type);
        db.Entry(type).Property(t => t.Color).IsModified = false;
        db.Entry(type).Property(t => t.Sticky).IsModified = false;
        await db.SaveChangesAsync();
        return null;
      }

      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", type.ProjectID);
      return null;
    }

    // GET: Types/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Models.Type type = await db.Types.FindAsync(id);
      if (type == null)
      {
        return HttpNotFound();
      }
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", type.ProjectID);
      return View(type);
    }

    // POST: Types/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ProjectID,Color")] Models.Type type)
    {
      if (ModelState.IsValid)
      {
        db.Entry(type).State = EntityState.Modified;
        db.Entry(type).Property(t => t.Sticky).IsModified = false;
        db.Entry(type).Property(t => t.CreatedAt).IsModified = false;
        type.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return null;
      }
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", type.ProjectID);
      return null;
    }

    // GET: Types/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Models.Type type = await db.Types.FindAsync(id);
      if (type == null)
      {
        return HttpNotFound();
      }
      return View(type);
    }

    // POST: Types/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Models.Type type = await db.Types.FindAsync(id);
      var project = type.Project;
      var sticky = project.Types.Where(t => t.Sticky == true).First().Id;
      var tasks = project.Tasks.Where(t => t.TypeID == id).ToList();
      tasks.ForEach(t => t.TypeID = sticky);
      db.Types.Remove(type);
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

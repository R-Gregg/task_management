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
  [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
  public class PagesController : Controller
  {
    private Entities db = new Entities();

    // GET: Pages
    public async Task<ActionResult> Index()
    {
      var pages = db.Pages.Include(p => p.User).Include(p => p.Department).OrderBy(p => p.Department.Name).ThenBy(p => p.Name);
      return View(await pages.ToListAsync());
    }

    // GET: Pages/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Page page = await db.Pages.FindAsync(id);
      if (page == null)
      {
        return HttpNotFound();
      }
      return View(page);
    }

    // GET: Pages/Create
    public ActionResult Create()
    {
      ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Email");
      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name");
      return PartialView();
    }

    // POST: Pages/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    public async Task<ActionResult> Create([Bind(Include = "Id,DepartmentID,Name,Text")] Page page)
    {
      if (ModelState.IsValid)
      {
        page.UpdatedBy = User.Identity.Name;
        page.UpdatedAt = DateTime.Now;
        db.Pages.Add(page);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Email", page.UpdatedBy);
      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name", page.DepartmentID);
      return View(page);
    }

    // GET: Pages/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Page page = await db.Pages.FindAsync(id);
      if (page == null)
      {
        return HttpNotFound();
      }
      ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Email", page.UpdatedBy);
      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name", page.DepartmentID);
      return PartialView(page);
    }

    // POST: Pages/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    public async Task<ActionResult> Edit([Bind(Include = "Id,DepartmentID,Name,Text,UpdatedBy,UpdatedAt")] Page page)
    {
      if (ModelState.IsValid)
      {
        db.Entry(page).State = EntityState.Modified;
        db.Entry(page).Property(p => p.DepartmentID).IsModified = false;
        db.Entry(page).Property(p => p.Name).IsModified = false;
        page.UpdatedBy = User.Identity.Name;
        page.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Email", page.UpdatedBy);
      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name", page.DepartmentID);
      return View(page);
    }

    // GET: Pages/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Page page = await db.Pages.FindAsync(id);
      if (page == null)
      {
        return HttpNotFound();
      }
      return View(page);
    }

    // POST: Pages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Page page = await db.Pages.FindAsync(id);
      db.Pages.Remove(page);
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

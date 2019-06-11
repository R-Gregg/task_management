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
  public class UserRolesController : Controller
  {
    private Entities db = new Entities();

    // GET: UserRoles
    public async Task<ActionResult> Index()
    {
      var userRoles = db.UserRoles.Include(u => u.Role).Include(u => u.User);
      return View(await userRoles.ToListAsync());
    }

    // GET: UserRoles/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserRole userRole = await db.UserRoles.FindAsync(id);
      if (userRole == null)
      {
        return HttpNotFound();
      }
      return View(userRole);
    }

    // GET: UserRoles/Create
    public ActionResult Create()
    {
      ViewBag.RoleID = new SelectList(db.Roles, "Id", "Name");
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid");
      return View();
    }

    // POST: UserRoles/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,UserID,RoleID")] UserRole userRole)
    {
      if (ModelState.IsValid)
      {
        db.UserRoles.Add(userRole);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.RoleID = new SelectList(db.Roles, "Id", "Name", userRole.RoleID);
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", userRole.UserID);
      return View(userRole);
    }

    // GET: UserRoles/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserRole userRole = await db.UserRoles.FindAsync(id);
      if (userRole == null)
      {
        return HttpNotFound();
      }
      ViewBag.RoleID = new SelectList(db.Roles, "Id", "Name", userRole.RoleID);
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", userRole.UserID);
      return View(userRole);
    }

    // POST: UserRoles/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,UserID,RoleID")] UserRole userRole)
    {
      if (ModelState.IsValid)
      {
        db.Entry(userRole).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.RoleID = new SelectList(db.Roles, "Id", "Name", userRole.RoleID);
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", userRole.UserID);
      return View(userRole);
    }

    // GET: UserRoles/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserRole userRole = await db.UserRoles.FindAsync(id);
      if (userRole == null)
      {
        return HttpNotFound();
      }
      return View(userRole);
    }

    // POST: UserRoles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      UserRole userRole = await db.UserRoles.FindAsync(id);
      db.UserRoles.Remove(userRole);
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

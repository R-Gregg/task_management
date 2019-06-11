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
  public class UsersController : Controller
  {
    private Entities db = new Entities();

    // GET: Users
    public async Task<ActionResult> Index()
    {
      var users = db.Users.Include(u => u.Department).Include(u => u.Location);
      return View(await users.ToListAsync());
    }

    // GET: Users/Details/5
    public async Task<ActionResult> Details(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = await db.Users.FindAsync("SCG-CPA\\" + id);
      if (user == null)
      {
        return HttpNotFound();
      }
      return View(user);
    }

    // GET: Users/Create
    public ActionResult Create()
    {
      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name");
      ViewBag.LocationID = new SelectList(db.Locations, "Id", "Name");
      return View();
    }

    // POST: Users/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Guid,FirstName,LastName,Email,PhoneNumber,Extension,DepartmentID,LocationID,Bio,Family,Birthday,C0_5,C6_12,C13_17,C18")] User user)
    {
      if (ModelState.IsValid)
      {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name", user.DepartmentID);
      ViewBag.LocationID = new SelectList(db.Locations, "Id", "Name", user.LocationID);
      return View(user);
    }

    // GET: Users/Edit/5
    public async Task<ActionResult> Edit(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = await db.Users.FindAsync("SCG-CPA\\" + id);
      if (user == null)
      {
        return HttpNotFound();
      }
      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name", user.DepartmentID);
      ViewBag.LocationID = new SelectList(db.Locations, "Id", "Name", user.LocationID);
      return View(user);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Guid,FirstName,LastName,Email,PhoneNumber,Extension,DepartmentID,LocationID,Bio,Family,Birthday,C0_5,C6_12,C13_17,C18")] User user)
    {
      if (ModelState.IsValid)
      {
        db.Entry(user).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "Name", user.DepartmentID);
      ViewBag.LocationID = new SelectList(db.Locations, "Id", "Name", user.LocationID);
      return View(user);
    }

    // GET: Users/Delete/5
    public async Task<ActionResult> Delete(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = await db.Users.FindAsync("SCG-CPA\\" + id);
      if (user == null)
      {
        return HttpNotFound();
      }
      return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(string id)
    {
      User user = await db.Users.FindAsync("SCG-CPA\\" + id);
      db.Users.Remove(user);
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

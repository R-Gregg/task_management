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
  public class EngageTypesController : Controller
  {
    private Entities db = new Entities();

    // GET: EngageTypes
    public async Task<ActionResult> Index()
    {
      return View(await db.EngageTypes.ToListAsync());
    }

    // GET: EngageTypes/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EngageType engageType = await db.EngageTypes.FindAsync(id);
      if (engageType == null)
      {
        return HttpNotFound();
      }
      return View(engageType);
    }

    // GET: EngageTypes/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: EngageTypes/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Name")] EngageType engageType)
    {
      if (ModelState.IsValid)
      {
        db.EngageTypes.Add(engageType);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      return View(engageType);
    }

    // GET: EngageTypes/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EngageType engageType = await db.EngageTypes.FindAsync(id);
      if (engageType == null)
      {
        return HttpNotFound();
      }
      return View(engageType);
    }

    // POST: EngageTypes/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] EngageType engageType)
    {
      if (ModelState.IsValid)
      {
        db.Entry(engageType).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(engageType);
    }

    // GET: EngageTypes/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EngageType engageType = await db.EngageTypes.FindAsync(id);
      if (engageType == null)
      {
        return HttpNotFound();
      }
      return View(engageType);
    }

    // POST: EngageTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      EngageType engageType = await db.EngageTypes.FindAsync(id);
      db.EngageTypes.Remove(engageType);
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

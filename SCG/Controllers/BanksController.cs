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
  public class BanksController : Controller
  {
    private Entities db = new Entities();

    // GET: Banks
    public async Task<ActionResult> Index()
    {
      return View(await db.Banks.ToListAsync());
    }

    // GET: Banks/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Bank bank = await db.Banks.FindAsync(id);
      if (bank == null)
      {
        return HttpNotFound();
      }
      return View(bank);
    }

    // GET: Banks/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: Banks/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Name,ClientNumber,Address,Address2,City,State,Zip,Contact,ContactEmail,Regulator,AssetSize,Notes,CreatedAt,UpdatedAt")] Bank bank)
    {
      if (ModelState.IsValid)
      {
        db.Banks.Add(bank);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      return View(bank);
    }

    // GET: Banks/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Bank bank = await db.Banks.FindAsync(id);
      if (bank == null)
      {
        return HttpNotFound();
      }
      return View(bank);
    }

    // POST: Banks/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ClientNumber,Address,Address2,City,State,Zip,Contact,ContactEmail,Regulator,AssetSize,Notes,CreatedAt,UpdatedAt")] Bank bank)
    {
      if (ModelState.IsValid)
      {
        db.Entry(bank).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(bank);
    }

    // GET: Banks/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Bank bank = await db.Banks.FindAsync(id);
      if (bank == null)
      {
        return HttpNotFound();
      }
      return View(bank);
    }

    // POST: Banks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Bank bank = await db.Banks.FindAsync(id);
      db.Banks.Remove(bank);
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

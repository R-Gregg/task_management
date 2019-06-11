using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCG.Models;

namespace SCG.Controllers
{
  public class MessageOfTheDaysController : Controller
  {
    private Entities db = new Entities();

    // GET: MessageOfTheDays
    public ActionResult Index()
    {
      var messageOfTheDays = db.MessageOfTheDays.Include(m => m.User);
      return View(messageOfTheDays.ToList());
    }

    // GET: MessageOfTheDays/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      MessageOfTheDay messageOfTheDay = db.MessageOfTheDays.Find(id);
      if (messageOfTheDay == null)
      {
        return HttpNotFound();
      }
      return View(messageOfTheDay);
    }

    // GET: MessageOfTheDays/Create
    public ActionResult Create()
    {
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Guid");
      return PartialView();
    }

    // POST: MessageOfTheDays/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Title,Description,CreatedAt,UpdatedAt,CreatedBy")] MessageOfTheDay messageOfTheDay)
    {
      if (ModelState.IsValid)
      {
        db.MessageOfTheDays.Add(messageOfTheDay);
        messageOfTheDay.CreatedBy = User.Identity.Name;
        messageOfTheDay.CreatedAt = DateTime.Now;
        messageOfTheDay.UpdatedAt = DateTime.Now;
        db.SaveChanges();
        return RedirectToAction("MOTD", "Marketing");
      }

      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Guid", messageOfTheDay.CreatedBy);
      return View(messageOfTheDay);
    }

    // GET: MessageOfTheDays/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      MessageOfTheDay messageOfTheDay = db.MessageOfTheDays.Find(id);
      if (messageOfTheDay == null)
      {
        return HttpNotFound();
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Guid", messageOfTheDay.CreatedBy);
      return PartialView(messageOfTheDay);
    }

    // POST: MessageOfTheDays/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Title,Description,CreatedAt,UpdatedAt,CreatedBy")] MessageOfTheDay messageOfTheDay)
    {
      if (ModelState.IsValid)
      {
        db.Entry(messageOfTheDay).State = EntityState.Modified;
        db.Entry(messageOfTheDay).Property(m => m.CreatedAt).IsModified = false;
        db.Entry(messageOfTheDay).Property(m => m.CreatedBy).IsModified = false;
        messageOfTheDay.UpdatedAt = DateTime.Now;
        db.SaveChanges();
        return RedirectToAction("MOTD", "Marketing");
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Guid", messageOfTheDay.CreatedBy);
      return View(messageOfTheDay);
    }

    // GET: MessageOfTheDays/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      MessageOfTheDay messageOfTheDay = db.MessageOfTheDays.Find(id);
      if (messageOfTheDay == null)
      {
        return HttpNotFound();
      }
      return View(messageOfTheDay);
    }

    // POST: MessageOfTheDays/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      MessageOfTheDay messageOfTheDay = db.MessageOfTheDays.Find(id);
      db.MessageOfTheDays.Remove(messageOfTheDay);
      db.SaveChanges();
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

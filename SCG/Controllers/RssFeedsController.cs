using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCG.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SCG.Controllers
{
  public class RssFeedsController : Controller
  {
    private Entities db = new Entities();

    // GET: RssFeeds
    public ActionResult Index()
    {
      var rssFeeds = db.RssFeeds.Include(r => r.User);
      return View(rssFeeds.ToList());
    }

    // GET: RssFeeds/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      RssFeed rssFeed = db.RssFeeds.Find(id);
      if (rssFeed == null)
      {
        return HttpNotFound();
      }
      return View(rssFeed);
    }

    // GET: RssFeeds/Create
    public ActionResult Create()
    {
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid");
      return View();
    }

    // POST: RssFeeds/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Title,Url,Order,UserID,Display")] RssFeed rssFeed)
    {
      if (ModelState.IsValid)
      {
        db.RssFeeds.Add(rssFeed);
        db.SaveChanges();
        return RedirectToAction("Index");
      }

      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", rssFeed.UserID);
      return View(rssFeed);
    }

    // GET: RssFeeds/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      RssFeed rssFeed = db.RssFeeds.Find(id);
      if (rssFeed == null)
      {
        return HttpNotFound();
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", rssFeed.UserID);
      return View(rssFeed);
    }

    // POST: RssFeeds/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public ActionResult Edit([Bind(Include = "Id,Title,Url,Order,UserID,Display")] RssFeed rssFeed)
    {
      if (ModelState.IsValid)
      {
        db.Entry(rssFeed).State = EntityState.Modified;
        db.Entry(rssFeed).Property(r => r.Changeable).IsModified = false;
        db.Entry(rssFeed).Property(r => r.UserID).IsModified = false;
        db.Entry(rssFeed).Property(r => r.Order).IsModified = false;
        db.SaveChanges();
        return null;
      }
      ViewBag.UserID = new SelectList(db.Users, "Id", "FullName", rssFeed.UserID);
      return null;
    }

    [HttpPost]
    public async System.Threading.Tasks.Task<ActionResult> OrderEdit([Bind(Include = "Id,Order")] RssFeed rssFeed)
    {
      if (ModelState.IsValid)
      {
        var currentFeed = db.RssFeeds.Find(rssFeed.Id);
        if (currentFeed == null)
        {
          return HttpNotFound();
        }
        currentFeed.Order = rssFeed.Order;
        db.Entry(currentFeed).State = EntityState.Modified;
        db.Entry(currentFeed).Property(r => r.Title).IsModified = false;
        db.Entry(currentFeed).Property(r => r.Url).IsModified = false;
        db.Entry(currentFeed).Property(r => r.UserID).IsModified = false;
        try
        {
          await db.SaveChangesAsync();
        }
        catch (DbEntityValidationException dbEx)
        {
          foreach (var validationErrors in dbEx.EntityValidationErrors)
          {
            foreach (var validationError in validationErrors.ValidationErrors)
            {
              Trace.TraceInformation(
                    "Class: {0}, Property: {1}, Error: {2}",
                    validationErrors.Entry.Entity.GetType().FullName,
                    validationError.PropertyName,
                    validationError.ErrorMessage);
            }
          }
        }
      }
      return null;
    }

    // GET: RssFeeds/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      RssFeed rssFeed = db.RssFeeds.Find(id);
      if (rssFeed == null)
      {
        return HttpNotFound();
      }
      return View(rssFeed);
    }

    // POST: RssFeeds/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      RssFeed rssFeed = db.RssFeeds.Find(id);
      db.RssFeeds.Remove(rssFeed);
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

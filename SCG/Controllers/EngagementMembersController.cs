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
  public class EngagementMembersController : Controller
  {
    private Entities db = new Entities();

    // GET: EngagementMembers
    public ActionResult Index()
    {
      var engagementMembers = db.EngagementMembers.Include(e => e.Engagement).Include(e => e.User);
      return View(engagementMembers.ToList());
    }

    // GET: EngagementMembers/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EngagementMember engagementMember = db.EngagementMembers.Find(id);
      if (engagementMember == null)
      {
        return HttpNotFound();
      }
      return View(engagementMember);
    }

    // GET: EngagementMembers/Create
    public ActionResult Create()
    {
      ViewBag.EngagementID = new SelectList(db.Engagements, "Id", "Location");
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid");
      return View();
    }

    // POST: EngagementMembers/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public ActionResult Create([Bind(Include = "Id,EngagementID,UserID,Role,Leader")] EngagementMember engagementMember)
    {
      if (ModelState.IsValid)
      {
        db.EngagementMembers.Add(engagementMember);
        db.SaveChanges();
        return null;
      }

      ViewBag.EngagementID = new SelectList(db.Engagements, "Id", "Location", engagementMember.EngagementID);
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", engagementMember.UserID);
      return View(engagementMember);
    }

    // GET: EngagementMembers/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EngagementMember engagementMember = db.EngagementMembers.Find(id);
      if (engagementMember == null)
      {
        return HttpNotFound();
      }
      ViewBag.EngagementID = new SelectList(db.Engagements, "Id", "Location", engagementMember.EngagementID);
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", engagementMember.UserID);
      return View(engagementMember);
    }

    // POST: EngagementMembers/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public ActionResult Edit([Bind(Include = "Id,EngagementID,UserID,Role,Leader")] EngagementMember engagementMember)
    {
      if (ModelState.IsValid)
      {
        db.Entry(engagementMember).State = EntityState.Modified;
        db.SaveChanges();
        return null;
      }
      ViewBag.EngagementID = new SelectList(db.Engagements, "Id", "Location", engagementMember.EngagementID);
      ViewBag.UserID = new SelectList(db.Users, "Id", "Guid", engagementMember.UserID);
      return View(engagementMember);
    }

    // GET: EngagementMembers/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EngagementMember engagementMember = db.EngagementMembers.Find(id);
      if (engagementMember == null)
      {
        return HttpNotFound();
      }
      return View(engagementMember);
    }

    // POST: EngagementMembers/Delete/5
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      EngagementMember engagementMember = db.EngagementMembers.Find(id);
      db.EngagementMembers.Remove(engagementMember);
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

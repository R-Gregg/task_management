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
using System.Text.RegularExpressions;

namespace SCG.Controllers
{
  public class AttendeesController : Controller
  {
    private Entities db = new Entities();

    // GET: Attendees
    public ActionResult Index(int? id, string sort, string contact, int guestType = 0)
    {
      var project = db.Projects.Find(id);
      if (project.Event == false)
      {
        return RedirectToAction("Index", "Tasks", new { id = id });
      }
      var shared = project.SharedProjects.Select(s => s.UserID);

      //List of Users for Filtering
      var users = db.Users.Where(u => shared.Contains(u.Id) || u.Id == project.CreatedBy).OrderBy(u => u.FirstName);
      ViewBag.UserID = new SelectList(users, "Id", "FullName", contact);
      ViewBag.GuestTypeID = new SelectList(project.GuestTypes.OrderBy(g => g.Name), "Id", "Name", guestType);

      IEnumerable<Attendee> attendees;

      if (string.IsNullOrEmpty(contact))
      {
        //Grab all Attendees
        attendees = project.Attendees.Where(a => a.ProjectID == id);
      }
      else
      {
        //If searching by Contact, grab Attendees with that Contact
        var contacts = db.Contacts.Where(c => c.UserID == contact).Select(c => c.AttendeeID);
        attendees = project.Attendees.Where(a => contacts.Contains(a.Id));
      }

      if (guestType != 0)
      {
        var gt = db.AttendeeGuestTypes.Where(g => g.GuestTypeID == guestType).Select(g => g.AttendeeID);
        attendees = attendees.Where(a => gt.Contains(a.Id));
      }

      //Current Sort
      ViewBag.Sort = sort;

      //Set Default Sort
      ViewBag.LName = "l_asc";
      ViewBag.FName = "f_asc";
      ViewBag.Company = "c_asc";
      ViewBag.Email = "e_asc";
      ViewBag.User = "u_asc"; //Contacts Sorting
      ViewBag.Guest = "g_asc";
      ViewBag.Attending = "a_asc";
      ViewBag.Date = "d_asc";
      //Sorting Function
      switch (sort)
      {
        case "l_asc":
          attendees = attendees.OrderBy(a => a.LastName);
          ViewBag.LName = "l_desc";
          break;
        case "l_desc":
          attendees = attendees.OrderByDescending(a => a.LastName);
          ViewBag.LName = "l_asc";
          break;
        case "f_asc":
          attendees = attendees.OrderBy(a => a.FirstName);
          ViewBag.FName = "f_desc";
          break;
        case "f_desc":
          attendees = attendees.OrderByDescending(a => a.FirstName);
          ViewBag.FName = "f_asc";
          break;
        case "c_asc":
          attendees = attendees.OrderBy(a => a.Company);
          ViewBag.Company = "c_desc";
          break;
        case "c_desc":
          attendees = attendees.OrderByDescending(a => a.Company);
          ViewBag.Company = "c_asc";
          break;
        case "e_asc":
          attendees = attendees.OrderBy(a => a.Email);
          ViewBag.Email = "e_desc";
          break;
        case "e_desc":
          attendees = attendees.OrderByDescending(a => a.Email);
          ViewBag.Email = "e_asc";
          break;
        case "u_asc":
          attendees = attendees.OrderBy(a => a.Contacts.Select(c => c.User.FirstName).FirstOrDefault());
          ViewBag.User = "u_desc";
          break;
        case "u_desc":
          attendees = attendees.OrderByDescending(a => a.Contacts.Select(c => c.User.FirstName).FirstOrDefault());
          ViewBag.User = "u_asc";
          break;
        case "g_asc":
          attendees = attendees.OrderBy(g => g.AttendeeGuestTypes.Select(gt => gt.GuestType.Name).FirstOrDefault());
          ViewBag.Guest = "g_desc";
          break;
        case "g_desc":
          attendees = attendees.OrderByDescending(g => g.AttendeeGuestTypes.Select(gt => gt.GuestType.Name).FirstOrDefault());
          ViewBag.Guest = "g_asc";
          break;
        case "a_asc":
          attendees = attendees.OrderBy(a => a.Rsvps.Where(r => r.Year == 2018).FirstOrDefault().Attending);
          ViewBag.Attending = "a_desc";
          break;
        case "a_desc":
          attendees = attendees.OrderByDescending(a => a.Rsvps.Where(r => r.Year == 2018).FirstOrDefault().Attending);
          ViewBag.Attending = "a_asc";
          break;
        case "d_asc":
          attendees = attendees.OrderBy(a => a.UpdatedAt);
          ViewBag.Date = "d_desc";
          break;
        case "d_desc":
          attendees = attendees.OrderByDescending(a => a.UpdatedAt);
          ViewBag.Date = "d_asc";
          break;
        default:
          attendees = attendees.OrderBy(a => a.LastName);
          break;
      }

      Session["project"] = id;
      Session["project_view"] = "Attendees";
      ViewBag.ProjectName = project.Name + " - Attendees";
      ViewBag.SelectedContact = Regex.Escape(contact + "");
      ViewBag.GuestType = guestType;
      ViewData["Owner"] = project.User.Id == User.Identity.Name ? "True" : "False";
      ViewData["Schedule"] = project.Schedule == true ? "True" : "False";
      ViewData["Event"] = project.Event == true ? "True" : "False";

      return PartialView(attendees);
    }

    // GET: Attendees/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Attendee attendee = await db.Attendees.FindAsync(id);
      if (attendee == null)
      {
        return HttpNotFound();
      }
      return View(attendee);
    }

    // GET: Attendees/Create
    public ActionResult Create()
    {
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      return PartialView();
    }

    // POST: Attendees/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,Company,PhoneNumber,Email")] Attendee attendee)
    {
      if (ModelState.IsValid)
      {
        attendee.ProjectID = (int)Session["project"];
        attendee.CreatedBy = User.Identity.Name;
        attendee.CreatedAt = DateTime.Now;
        attendee.UpdatedAt = DateTime.Now;
        db.Attendees.Add(attendee);
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }

      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", attendee.CreatedBy);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", attendee.ProjectID);
      return View(attendee);
    }

    // GET: Attendees/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Attendee attendee = await db.Attendees.FindAsync(id);
      if (attendee == null)
      {
        return HttpNotFound();
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", attendee.CreatedBy);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", attendee.ProjectID);
      return PartialView(attendee);
    }

    // POST: Attendees/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,Company,PhoneNumber,Email")] Attendee attendee)
    {
      if (ModelState.IsValid)
      {
        db.Entry(attendee).State = EntityState.Modified;
        db.Entry(attendee).Property(t => t.ProjectID).IsModified = false;
        db.Entry(attendee).Property(t => t.CreatedBy).IsModified = false;
        db.Entry(attendee).Property(t => t.CreatedAt).IsModified = false;
        attendee.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", attendee.CreatedBy);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", attendee.ProjectID);
      return View(attendee);
    }

    // GET: Attendees/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Attendee attendee = await db.Attendees.FindAsync(id);
      if (attendee == null)
      {
        return HttpNotFound();
      }
      return PartialView(attendee);
    }

    // POST: Attendees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Attendee attendee = await db.Attendees.FindAsync(id);
      db.AttendeeGuestTypes.RemoveRange(attendee.AttendeeGuestTypes);
      db.Contacts.RemoveRange(attendee.Contacts);
      db.Touches.RemoveRange(attendee.Touches);
      db.Rsvps.RemoveRange(attendee.Rsvps);
      db.Attendees.Remove(attendee);
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

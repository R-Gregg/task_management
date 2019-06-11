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
using System.IO;

namespace SCG.Controllers
{
  public class ProjectsController : Controller
  {
    private Entities db = new Entities();

    // GET: Projects
    [Authorize]
    public async Task<ActionResult> Index(int? c)
    {
      if (c == 1)
      {
        Session["project"] = 0;
      }
      var userID = User.Identity.Name;
      var projects = db.Projects.Where(p => p.CreatedBy == userID).Include(p => p.User).OrderByDescending(p => p.UpdatedAt);
      var shared = db.SharedProjects.Where(s => s.UserID == userID).Select(s => s.ProjectID);
      var sharedProjects = db.Projects.Where(p => shared.Contains(p.Id)).OrderByDescending(p => p.UpdatedAt);
      ViewData["SharedProjects"] = sharedProjects;
      ViewBag.Tutorial = true;
      return View(await projects.ToListAsync());
    }

    // GET: Projects/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Project project = await db.Projects.FindAsync(id);
      if (project == null)
      {
        return HttpNotFound();
      }
      return View(project);
    }

    // GET: Projects/Create
    public ActionResult Create()
    {
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
      return PartialView();
    }

    // POST: Projects/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Name,Year,Schedule,Event,Assign,DueDates")] Project project)
    {
      if (ModelState.IsValid)
      {
        project.CreatedBy = User.Identity.Name;
        project.CreatedAt = DateTime.Now;
        project.UpdatedAt = DateTime.Now;
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        CreateAttributes(project.Id);
      }

      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", project.CreatedBy);
      return RedirectToAction("Index");
    }

    public ActionResult CreateAttributes(int? id)
    {
      var i = (int)id;
      Priority priority1 = new Priority
      {
        Name = "High",
        ProjectID = i,
        Color = "red",
        Sticky = true
      };

      Priority priority2 = new Priority
      {
        Name = "Medium",
        ProjectID = i,
        Color = "orange"
      };

      Priority priority3 = new Priority
      {
        Name = "Low",
        ProjectID = i,
        Color = "yellowgreen"
      };

      Models.Type type1 = new Models.Type
      {
        Name = "Todo",
        ProjectID = i,
        Sticky = true
      };

      Models.Type type2 = new Models.Type
      {
        Name = "In Progress",
        ProjectID = i
      };

      Models.Type type3 = new Models.Type
      {
        Name = "Completed",
        ProjectID = i,
        Color = "#b1ffb1"
      };

      GuestType gt1 = new GuestType
      {
        Name = "Prospect",
        ProjectID = i,
        Sticky = true
      };

      db.Priorities.Add(priority1);
      db.Priorities.Add(priority2);
      db.Priorities.Add(priority3);
      db.Types.Add(type1);
      db.Types.Add(type2);
      db.Types.Add(type3);
      db.GuestTypes.Add(gt1);

      db.SaveChanges();

      Session["project"] = id;
      return RedirectToAction("Index");
    }

    // GET: Projects/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Project project = await db.Projects.FindAsync(id);
      if (project == null)
      {
        return HttpNotFound();
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", project.CreatedBy);
      return PartialView(project);
    }

    // POST: Projects/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Year,Schedule,Event,Assign,DueDates")] Project project)
    {
      if (ModelState.IsValid)
      {
        db.Entry(project).State = EntityState.Modified;
        db.Entry(project).Property(p => p.CreatedBy).IsModified = false;
        db.Entry(project).Property(p => p.CreatedAt).IsModified = false;
        project.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", project.CreatedBy);
      return View(project);
    }

    // GET: Projects/Settings/5
    // Popup for project settings
    [HttpGet]
    public ActionResult Settings(int? id)
    {
      var project = db.Projects.Find(id);
      var shared = project.SharedProjects.Select(s => s.UserID);
      var sharedUsers = db.Users.Where(a => shared.Contains(a.Id)).OrderBy(a => a.FirstName);
      var unsharedUsers = db.Users.Where(a => !shared.Contains(a.Id) && a.Id != project.CreatedBy && a.Status == true).OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
      ViewData["Shared"] = sharedUsers;
      ViewData["Unshared"] = unsharedUsers;
      ViewData["ProjectID"] = id;
      return PartialView(project);
    }

    // GET: Projects/SetSession
    // Sets Project Session and redirects back to Projects/Index
    [HttpGet]
    public ActionResult SetSession(int? id)
    {
      Session["project"] = id;
      return RedirectToAction("Index");
    }

    public ActionResult ImportAttendees()
    {
      var project = db.Projects.Find(8);
      using (var reader = new StreamReader(@"C:\Users\ron.gregg\Documents\2018_PBLF_Invite_List.csv"))
      {
        while (!reader.EndOfStream)
        {
          var line = reader.ReadLine();
          var values = line.Split(',');

          Attendee attendee = new Attendee
          {
            ProjectID = project.Id,
            FirstName = values[0].Replace("|", ","),
            LastName = values[1].Replace("|", ","),
            Company = values[2].Replace("|", ","),
            Email = values[4].Replace("|", ",")
          };

          db.Attendees.Add(attendee);

          var attendeeId = attendee.Id;

          var contacts = values[3];
          foreach (var contact in contacts.Split('|').ToList())
          {
            if (contact != "Internal" && contact != "")
            {
              var userId = db.Users.Where(u => u.LastName == contact).Select(u => u.Id).FirstOrDefault();

              if (userId != null)
              {
                Contact con = new Contact
                {
                  AttendeeID = attendeeId,
                  UserID = userId
                };

                db.Contacts.Add(con);
              }
            }
          }

          var gtname = values[5];
          var gt = db.GuestTypes.Where(g => g.Name == gtname).FirstOrDefault();
          if (gt != null)
          {
            AttendeeGuestType guestType = new AttendeeGuestType
            {
              AttendeeID = attendeeId,
              GuestTypeID = gt.Id
            };

            db.AttendeeGuestTypes.Add(guestType);

          }

          var rsvps = values[10];
          var rsvpsList = rsvps.Split('|').ToList();
          foreach (var rsvp in rsvpsList)
          {
            if (rsvp != "")
            {
              var year = Int32.Parse(rsvp);
              var attended = values[11].Replace("|", ",");
              var noshow = values[12].Replace("|", ",");
              var attendance = "";

              if (attended.Contains(rsvp))
              {
                attendance = "Both";
              }
              else if (noshow.Contains(rsvp))
              {
                attendance = "No Show";
              }

              Rsvp r = new Rsvp
              {
                AttendeeID = attendeeId,
                Year = year,
                Attending = "Both",
                Attended = attendance,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
              };

              db.Rsvps.Add(r);
            }
          }
          //db.SaveChanges();
        }
      }
      return RedirectToAction("Index");
    }

    // GET: Projects/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Project project = await db.Projects.FindAsync(id);
      if (project == null)
      {
        return HttpNotFound();
      }
      return PartialView(project);
    }

    // POST: Projects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Project project = await db.Projects.FindAsync(id);
      db.Priorities.RemoveRange(project.Priorities);
      db.Types.RemoveRange(project.Types);
      db.GuestTypes.RemoveRange(project.GuestTypes);
      db.FavoriteProjects.RemoveRange(project.FavoriteProjects);
      db.SharedProjects.RemoveRange(project.SharedProjects);
      db.Attendees.RemoveRange(project.Attendees);
      db.KeyPersons.RemoveRange(project.KeyPersons);
      db.Projects.Remove(project);
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

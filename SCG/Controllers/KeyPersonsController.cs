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
  public class KeyPersonsController : Controller
  {
    private Entities db = new Entities();

    // GET: KeyPersons
    public async Task<ActionResult> Index(int? id, string sort)
    {
      var project = db.Projects.Find(id);
      if (project.Event == false)
      {
        return RedirectToAction("Index", "Tasks", new { id = id });
      }
      var keyPersons = db.KeyPersons.Where(k => k.ProjectID == id).Include(k => k.User).Include(k => k.Project);

      //Current Sort
      ViewBag.Sort = sort;

      //Set Default Sort
      ViewBag.LName = "l_asc";
      ViewBag.FName = "f_asc";
      ViewBag.Company = "c_asc";
      ViewBag.Email = "e_asc";
      ViewBag.KeyTitle = "t_asc";
      ViewBag.Role = "r_asc";
      ViewBag.Date = "d_asc";
      //Sorting Function
      switch (sort)
      {
        case "l_asc":
          keyPersons = keyPersons.OrderBy(k => k.LastName);
          ViewBag.LName = "l_desc";
          break;
        case "l_desc":
          keyPersons = keyPersons.OrderByDescending(k => k.LastName);
          ViewBag.LName = "l_asc";
          break;
        case "f_asc":
          keyPersons = keyPersons.OrderBy(k => k.FirstName);
          ViewBag.FName = "f_desc";
          break;
        case "f_desc":
          keyPersons = keyPersons.OrderByDescending(k => k.FirstName);
          ViewBag.FName = "f_asc";
          break;
        case "c_asc":
          keyPersons = keyPersons.OrderBy(k => k.Company);
          ViewBag.Company = "c_desc";
          break;
        case "c_desc":
          keyPersons = keyPersons.OrderByDescending(k => k.Company);
          ViewBag.Company = "c_asc";
          break;
        case "e_asc":
          keyPersons = keyPersons.OrderBy(k => k.Email);
          ViewBag.Email = "e_desc";
          break;
        case "e_desc":
          keyPersons = keyPersons.OrderByDescending(k => k.Email);
          ViewBag.Email = "e_asc";
          break;
        case "t_asc":
          keyPersons = keyPersons.OrderBy(k => k.Title);
          ViewBag.KeyTitle = "t_desc";
          break;
        case "t_desc":
          keyPersons = keyPersons.OrderByDescending(k => k.Title);
          ViewBag.KeyTitle = "t_asc";
          break;
        case "k_asc":
          keyPersons = keyPersons.OrderBy(k => k.Role);
          ViewBag.Role = "r_desc";
          break;
        case "k_desc":
          keyPersons = keyPersons.OrderByDescending(k => k.Role);
          ViewBag.Role = "r_asc";
          break;
        case "d_asc":
          keyPersons = keyPersons.OrderBy(k => k.UpdatedAt);
          ViewBag.Date = "d_desc";
          break;
        case "d_desc":
          keyPersons = keyPersons.OrderByDescending(k => k.UpdatedAt);
          ViewBag.Date = "d_asc";
          break;
        default:
          keyPersons = keyPersons.OrderBy(k => k.LastName);
          break;
      }

      Session["project"] = id;
      Session["project_view"] = "KeyPersons";
      ViewBag.ProjectName = project.Name + " - Key People";
      ViewData["Owner"] = project.User.Id == User.Identity.Name ? "True" : "False";
      ViewData["Schedule"] = project.Schedule == true ? "True" : "False";
      ViewData["Event"] = project.Event == true ? "True" : "False";

      return PartialView(await keyPersons.ToListAsync());
    }

    // GET: KeyPersons/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      KeyPerson keyPerson = await db.KeyPersons.FindAsync(id);
      if (keyPerson == null)
      {
        return HttpNotFound();
      }
      return View(keyPerson);
    }

    // GET: KeyPersons/Create
    public ActionResult Create()
    {
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Email");
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
      return PartialView();
    }

    // POST: KeyPersons/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,Company,PhoneNumber,Email,Title,Role")] KeyPerson keyPerson)
    {
      if (ModelState.IsValid)
      {
        keyPerson.ProjectID = (int)Session["project"];
        keyPerson.CreatedBy = User.Identity.Name;
        keyPerson.CreatedAt = DateTime.Now;
        keyPerson.UpdatedAt = DateTime.Now;
        db.KeyPersons.Add(keyPerson);
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }

      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Email", keyPerson.CreatedBy);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", keyPerson.ProjectID);
      return View(keyPerson);
    }

    // GET: KeyPersons/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      KeyPerson keyPerson = await db.KeyPersons.FindAsync(id);
      if (keyPerson == null)
      {
        return HttpNotFound();
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Email", keyPerson.CreatedBy);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", keyPerson.ProjectID);
      return PartialView(keyPerson);
    }

    // POST: KeyPersons/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,Company,PhoneNumber,Email,Title,Role")] KeyPerson keyPerson)
    {
      if (ModelState.IsValid)
      {
        db.Entry(keyPerson).State = EntityState.Modified;
        db.Entry(keyPerson).Property(k => k.ProjectID).IsModified = false;
        db.Entry(keyPerson).Property(k => k.CreatedBy).IsModified = false;
        db.Entry(keyPerson).Property(k => k.CreatedAt).IsModified = false;
        keyPerson.UpdatedAt = DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Projects");
      }
      ViewBag.CreatedBy = new SelectList(db.Users, "Id", "Email", keyPerson.CreatedBy);
      ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", keyPerson.ProjectID);
      return View(keyPerson);
    }

    // GET: KeyPersons/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      KeyPerson keyPerson = await db.KeyPersons.FindAsync(id);
      if (keyPerson == null)
      {
        return HttpNotFound();
      }
      return View(keyPerson);
    }

    // POST: KeyPersons/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      KeyPerson keyPerson = await db.KeyPersons.FindAsync(id);
      db.KeyPersons.Remove(keyPerson);
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

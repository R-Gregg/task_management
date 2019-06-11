using SCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCG.Controllers
{
  public class AuditController : Controller
  {
    private Entities db = new Entities();
    // GET: Audit
    public ActionResult Index()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Audit").First();
        var page = department.Pages.Where(p => p.Name == "Index").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }
  }
}
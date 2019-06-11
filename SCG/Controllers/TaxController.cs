using SCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCG.Controllers
{
  public class TaxController : Controller
  {
    private Entities db = new Entities();
    // GET: Tax
    public ActionResult Index()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Tax").First();
        var page = department.Pages.Where(p => p.Name == "Index").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult PoliciesProcedures()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Tax").First();
        var page = department.Pages.Where(p => p.Name == "Policies/Procedures").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult Worksheets()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Tax").First();
        var page = department.Pages.Where(p => p.Name == "Worksheets").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult Form1040()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Tax").First();
        var page = department.Pages.Where(p => p.Name == "Form 1040").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult CPEOpportunities()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Tax").First();
        var page = department.Pages.Where(p => p.Name == "CPE Opportunities").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }
    public ActionResult AdditionalResources()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Tax").First();
        var page = department.Pages.Where(p => p.Name == "Additional Resources").FirstOrDefault();
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
using SCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCG.Controllers
{
  public class HRController : Controller
  {
    private Entities db = new Entities();
    // GET: HR
    public ActionResult Index()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "HR").First();
        var page = department.Pages.Where(p => p.Name == "Index").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult Forms()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "HR").First();
        var page = department.Pages.Where(p => p.Name == "Forms").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult InformativeLinks()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "HR").First();
        var page = department.Pages.Where(p => p.Name == "Informative Links").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult PersonnelPolicies()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "HR").First();
        var page = department.Pages.Where(p => p.Name == "Personnel Policies").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult EmployeeBenefits()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "HR").First();
        var page = department.Pages.Where(p => p.Name == "Employee Benefits").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult EmployeeDiscounts()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "HR").First();
        var page = department.Pages.Where(p => p.Name == "Employee Discounts").FirstOrDefault();
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
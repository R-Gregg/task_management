using SCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCG.Controllers
{
  public class MarketingController : Controller
  {
    private Entities db = new Entities();
    // GET: Marketing
    public ActionResult Index()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Marketing").First();
        var page = department.Pages.Where(p => p.Name == "Index").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult EmployeeResources()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Marketing").First();
        var page = department.Pages.Where(p => p.Name == "Employee Resources").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult MarketingMaterials()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Marketing").First();
        var page = department.Pages.Where(p => p.Name == "Marketing Materials").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    public ActionResult SocialMedia()
    {
      try
      {
        var department = db.Departments.Where(d => d.Name == "Marketing").First();
        var page = department.Pages.Where(p => p.Name == "Social Media").FirstOrDefault();
        return View(page);
      }
      catch (Exception e)
      {
        ViewBag.Error = e.Message;
        return View("Error");
      }
    }

    [Authorize(Roles = "SCG-CPA\\Intranet Admins")]
    public ActionResult MOTD()
    {
      var messages = db.MessageOfTheDays.OrderByDescending(m => m.Id);
      return View(messages);
    }
  }
}
using SCG.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SCG.Controllers
{
  public class ITController : Controller
  {
    private Entities db = new Entities();

    // GET: IT
    public ActionResult Index()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Index").FirstOrDefault();

      return View(page);
    }

    public ActionResult Inventory()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Printers").FirstOrDefault();

      return View(page);
    }

    public ActionResult ServerMap()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Server Map").FirstOrDefault();

      return View(page);
    }

    public ActionResult MappedDrives()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Mapped Drives").FirstOrDefault();

      return View(page);
    }

    public ActionResult ComputerInventory()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Computer Inventory").FirstOrDefault();

      return View(page);
    }

    public ActionResult ServerInventory()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Server Inventory").FirstOrDefault();

      return View(page);
    }

    public ActionResult ProgramLocations()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Program Locations").FirstOrDefault();

      return View(page);
    }

    public ActionResult HelpfulLinks()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Helpful Links").FirstOrDefault();

      return View(page);
    }

    public ActionResult IPAddresses()
    {
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Pensacola IP Addresses").FirstOrDefault();
      return View(page);
    }

    public ActionResult GetPage(string t)
    {
      t = t ?? "Pensacola IP Addresses";
      var departmentID = db.Departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == t).FirstOrDefault();
      return PartialView(page);
    }

    public ActionResult Network()
    {
      //ArrayList strings = new ArrayList();
      //bool pingable = false;

      //using (Ping pinger = new Ping())
      //{
      //  try
      //  {
      //    string ipBase = "192.168.1.";
      //    for (int i = 1; i < 255; i++)
      //    {
      //      PingReply reply = await pinger.SendPingAsync(ipBase + i.ToString(), 100);
      //      pingable = reply.Status == IPStatus.Success;
      //      if (pingable == true)
      //      {
      //        try
      //        {
      //          string ip = ipBase + i.ToString();
      //          IPHostEntry hostEntry;
      //          hostEntry = Dns.GetHostEntry(ip);
      //          strings.Add("Host Name: -" + hostEntry.HostName + "- IP Address: " + ipBase + i.ToString());
      //        }
      //        catch (Exception e)
      //        {
      //          strings.Add("IP Address: " + ipBase + i.ToString());
      //        }
      //      }
      //    }
      //  }
      //  catch (PingException e)
      //  {
      //    pingable = false;
      //    throw e;
      //  }
      //}
      //ViewBag.Strings = strings;
      return View();
    }

    public ActionResult Admin()
    {
      var path = Server.MapPath("~/assets/IT/");
      var dir = new DirectoryInfo(path);
      var files = dir.EnumerateFiles().Select(f => f.Name);

      return View(files);
    }

    public ActionResult UploadFile(HttpPostedFileBase file)
    {
      var path = Path.Combine(Server.MapPath("~/assets/IT/"), file.FileName);
      var data = new byte[file.ContentLength];
      file.InputStream.Read(data, 0, file.ContentLength);

      using (var sw = new FileStream(path, FileMode.Create))
      {
        sw.Write(data, 0, data.Length);
      }

      return RedirectToAction("Admin");
    }
  }
}
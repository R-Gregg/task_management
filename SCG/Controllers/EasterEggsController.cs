using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCG.Controllers
{
    public class EasterEggsController : Controller
    {
        // GET: EasterEggs
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DancingLee()
        {
            return PartialView();
        }
        public ActionResult SuperLee()
        {
            return PartialView();
        }
        public ActionResult DancingJayme()
        {
            return PartialView();
        }
    }
}
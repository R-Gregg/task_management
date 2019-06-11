using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCG.Controllers
{
    public class TwoMinutesController : Controller
    {
        // GET: TwoMinutes
        public ActionResult Index()
        {
            var scgurl = "http://www.saltmarshcpa.com/2-minutes/";
            var scgweb = new HtmlWeb();
            var scgdoc = scgweb.Load(scgurl);

            HtmlNode[] scgnodes = scgdoc.DocumentNode.SelectNodes("//*[contains(@id, 'Bdy-Z1')]").ToArray();
            ViewBag.scgNodes = scgnodes;

            return View();
        }

        public ActionResult TwoMinutes()
        {

            var vidurl = "http://www.saltmarshcpa.com/2-minutes/";
            var vidweb = new HtmlWeb();
            var viddoc = vidweb.Load(vidurl);

            HtmlNode[] vidnodes = viddoc.DocumentNode.SelectNodes("//*[contains(@id, 'Bdy-Z1')]//video").ToArray();
            ViewBag.vidNodes = vidnodes;

            return PartialView();
        }
    }
}
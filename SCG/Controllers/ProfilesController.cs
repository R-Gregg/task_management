using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCG.Models;
using System.Web.Helpers;

namespace SCG.Controllers
{
    public class ProfilesController : Controller
    {
        private Entities db = new Entities();

        // GET: Profiles
        public ActionResult Index()
        {
            //List<User> users = new List<User>();

            //PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            //GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "Intranet Users");

            //if (group != null)
            //{
            //    foreach (Principal p in group.GetMembers())
            //    {
            //        if (p is UserPrincipal aUser && !aUser.IsAccountLockedOut())
            //        {
            //            var user = db.Users.Find("SCG-CPA\\" + aUser.SamAccountName);
            //            if (user != null)
            //            {
            //                users.Add(user);
            //            }
            //        }
            //    }
            //}

            //ViewBag.Users = users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);
            return View();

        }
        public ActionResult Roster()
        {
            List<User> users = new List<User>();

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "Intranet Users");

            if (group != null)
            {
                foreach (Principal p in group.GetMembers())
                {
                    if (p is UserPrincipal aUser && !aUser.IsAccountLockedOut())
                    {
                        var user = db.Users.Find("SCG-CPA\\" + aUser.SamAccountName);
                        if (user != null)
                        {
                            users.Add(user);
                        }
                    }
                }
            }

            ViewBag.Users = users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);
            return PartialView();

        }

        // GET: Profiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: Profiles/Create
        public ActionResult Create()
        {
            ViewBag.pro__User_ID = new SelectList(db.Users, "Id", "Guid");
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pro__ID,pro__First_Name,pro__Last_Name,pro__Creds,pro__Title,pro__Phone,pro__Phone_Ext,pro__Email,pro__Shareholder,pro__Areas,pro__Bio,pro__Education,pro__Certifications,pro__Professional_Affiliations,pro__Community_Affiliations,pro__Spouse,pro__Kids,pro__Anniv_Month,pro__Anniv_Year,pro__Photo,pro__Original_Width,pro__Original_Height,pro__File_Resume,pro__File_VCard,pro__LinkedIn,pro__URL,pro__Meta_Title,pro__Meta_Description,pro__Meta_Keywords,pro__User_ID,pro__Status,Date_Created,Date_Created_IP,Last_Updated,Last_Updated_IP")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.pro__User_ID = new SelectList(db.Users, "Id", "Guid", profile.pro__User_ID);
            return View(profile);
        }

        // GET: Profiles/Edit/5
        public ActionResult Edit()
        {
            var id_nme = User.Identity.Name;

            if (id_nme == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var profile = db.Profiles.Where(p => p.pro__User_ID == id_nme).FirstOrDefault();
            // var profile = db.Profiles.Where(p => p.pro__ID == 4).FirstOrDefault();
            if (profile == null)
            {
                return RedirectToAction("Index");
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        //        public ActionResult Edit([Bind(Include = "pro__ID,pro__First_Name,pro__Last_Name,pro__Creds,pro__Title,pro__Phone,pro__Phone_Ext,pro__Email,pro__Shareholder,pro__Areas,pro__Bio,pro__Education,pro__Certifications,pro__Professional_Affiliations,pro__Community_Affiliations,pro__Spouse,pro__Kids,pro__Anniv_Month,pro__Anniv_Year,pro__Photo,pro__Original_Width,pro__Original_Height,pro__File_Resume,pro__File_VCard,pro__LinkedIn,pro__URL,pro__Meta_Title,pro__Meta_Description,pro__Meta_Keywords,pro__User_ID,pro__Status,Date_Created,Date_Created_IP,Last_Updated,Last_Updated_IP")] Profile profile)
        public ActionResult Edit(FormCollection FC, Profile profile, HttpPostedFileBase pro__Photo)
        {

            var FName = "";

            if (pro__Photo != null)
            {
                if (pro__Photo.ContentLength > 0)
                {
                    WebImage ImgStream = new WebImage(pro__Photo.InputStream);

                    // Image Resize
                    int MaxWidth = 225;     // Set Max Width
                    int MaxHeight = 330;    // Set Max Height

                    int OriginalWidth = ImgStream.Width;
                    int OriginalHeight = ImgStream.Height;

                    int NewWidth = MaxWidth;
                    int NewHeight = MaxHeight;

                    if (OriginalWidth > MaxWidth)
                    {
                        NewWidth = MaxWidth;
                        NewHeight = (OriginalHeight * NewWidth) / OriginalWidth;

                        if (NewHeight > MaxHeight)
                        {
                            NewHeight = MaxHeight;
                            NewWidth = (OriginalWidth * NewHeight) / OriginalHeight;
                        }

                        ImgStream.Resize(NewWidth, NewHeight);
                    }


                    // Create Random Name
                    FName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(pro__Photo.FileName);

                    // Save Image
                    ImgStream.Save(System.IO.Path.Combine(Server.MapPath("~/assets/Employees/"), FName));

                    profile.pro__Photo = FName;

                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.pro__User_ID = new SelectList(db.Users, "Id", "Guid", profile.pro__User_ID);
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeletePhoto()
        {

            var id_nme = User.Identity.Name;
            Profile profile = db.Profiles.Where(p => p.pro__User_ID == id_nme).FirstOrDefault();

            string ImagePath = System.IO.Path.Combine(Server.MapPath("~/assets/Employees/"), profile.pro__Photo);
            if (System.IO.File.Exists(ImagePath))
            {
                System.IO.File.Delete(ImagePath);
            }

            profile.pro__Photo = null;
            db.SaveChanges();

            ViewBag.Results = "Complete";

            return PartialView();
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

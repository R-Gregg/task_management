using SCG.Models;
using System;
using System.Collections;
using System.Data.Entity;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Graph;
using System.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using HtmlAgilityPack;

namespace SCG.Controllers
{
  public class AzureAuthenticationProvider : IAuthenticationProvider
  {
    public async System.Threading.Tasks.Task AuthenticateRequestAsync(HttpRequestMessage request)
    {
      var clientId = ConfigurationManager.AppSettings["ida:ClientId"];
      var clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
      var redirectUri = ConfigurationManager.AppSettings["ida:Domain"];

      AuthenticationContext authContext = new AuthenticationContext("https://login.microsoftonline.com/saltmarshcpa.onmicrosoft.com/oauth2/authorize");
      ClientCredential creds = new ClientCredential(clientId, clientSecret);
      AuthenticationResult authResult = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", creds);

      request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken);
    }
  }
  public class HomeController : Controller
  {
    private Entities db = new Entities();
    public ActionResult Index()
    {
      // Get Current Users AD
      //WindowsIdentity current = (WindowsIdentity)User.Identity;
      //DirectoryEntry userObject = new DirectoryEntry("LDAP://<SID=" + current.User.ToString() + ">");
      //ViewBag.Current = current;
      //ViewBag.User = userObject;
      //var verified = CheckUser();

      //if (verified == false)
      //{
      //  return RedirectToAction("UpdateUser");
      //}

      var user = db.Users.Find(User.Identity.Name);
      if (user != null)
      {
        ViewBag.Feeds = user.RssFeeds.Where(r => r.Display == true).OrderBy(r => r.Order);
        //ViewBag.Office = user.Location.Name;
      }
      ViewBag.MOTD = db.MessageOfTheDays.OrderByDescending(m => m.Id).First();
      ViewBag.Tutorial = true;

      return View();
    }

    [OutputCache(Duration = 900, VaryByParam = "url")]
    public ActionResult GetRss(string url)
    {
      var reader = XmlReader.Create(url);
      var rssFeed = SyndicationFeed.Load(reader);
      reader.Close();
      List<SyndicationItem> rssFeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in rssFeed.Items)
      {
        rssFeeds.Add(item);
      }
      ViewBag.Feeds = rssFeeds;

      return PartialView();
    }

    public ActionResult Growth()
    {
      var departmentID = db.Departments.Where(d => d.Name == "Admin").Select(d => d.Id).First();
      var page = db.Pages.Where(p => p.DepartmentID == departmentID && p.Name == "Growth").FirstOrDefault();

      return View(page);
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";
      ArrayList allObjects = new ArrayList();
      ArrayList allRoles = new ArrayList();
      ArrayList users = new ArrayList();
      try
      {
        //Get all high level directories in the domain
        var OuDn = "DC=scg-cpa,DC=net";
        DirectoryEntry directoryObject = new DirectoryEntry("LDAP://" + OuDn);
        ViewBag.Directory = directoryObject;
        foreach (DirectoryEntry child in directoryObject.Children)
        {
          string childPath = child.Path.ToString();
          allObjects.Add(childPath.Remove(0, 7));
          child.Close();
          child.Dispose();
        }
        directoryObject.Close();
        directoryObject.Dispose();

        //Get all directories inside Users (Laptop) in the domain
        OuDn = "OU=Users (Desktop),DC=scg-cpa,DC=net";
        DirectoryEntry directoryNewObject = new DirectoryEntry("LDAP://" + OuDn);
        foreach (DirectoryEntry child in directoryNewObject.Children)
        {
          string childPath = child.Path.ToString();
          users.Add(childPath.Remove(0, 7));
          child.Close();
          child.Dispose();
        }
        directoryNewObject.Close();
        directoryNewObject.Dispose();
      }
      catch (DirectoryServicesCOMException e)
      {
        Console.WriteLine("An Error Occurred: " + e.Message.ToString());
      }

      //Get All User Security Roles in the domain
      using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
      {
        GroupPrincipal qbeGroup = new GroupPrincipal(ctx);
        qbeGroup.IsSecurityGroup = true;
        PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);

        foreach (var found in srch.FindAll())
        {
          allRoles.Add(found.Name);
        }
      }
      ViewBag.Objects = allObjects;
      ViewBag.Roles = allRoles;
      ViewBag.NewObjects = users;
      return View();
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";
      //Connection to AD
      DirectoryEntry de = new DirectoryEntry
      {
        Path = "LDAP://DC=scg-cpa,DC=net",
        AuthenticationType = AuthenticationTypes.Secure
      };

      //Searcher for AD
      DirectorySearcher deSearch = new DirectorySearcher
      {
        SearchRoot = de,
        Filter = "(&(objectClass=Users (Administrators)) (cn=ron.gregg))"
      };

      System.DirectoryServices.SearchResult result = deSearch.FindOne();

      //Get specific user properties
      DirectoryEntry deUser = new DirectoryEntry("LDAP://CN=Ron Gregg,OU=Users (Administrators),DC=scg-cpa,DC=net");
      ViewBag.Result = deUser.Properties["cn"].Value.ToString();
      ViewBag.OtherResult = deUser.Properties["sn"].Value.ToString();
      ViewBag.NewResult = deUser.Properties["physicalDeliveryOfficeName"].Value.ToString();
      ArrayList allObjects = new ArrayList();
      foreach (string strAttrName in deUser.Properties.PropertyNames)
      {
        //Add user property names to ArrayList
        allObjects.Add(strAttrName);
      }
      deUser.Close();
      ViewBag.Objects = allObjects;
      ViewBag.Roles = Roles.GetRolesForUser();
      return View();
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult Test()
    {
      //Connection to AD and Searcher for AD
      DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://DC=scg-cpa,DC=net");
      DirectorySearcher searcher = new DirectorySearcher(directoryEntry)
      {
        PageSize = int.MaxValue,
        Filter = "(&(OU=Users*))"
      };

      var results = searcher.FindAll();

      ArrayList allObjects = new ArrayList();
      ArrayList userPathList = new ArrayList();
      ArrayList allUsers = new ArrayList();
      foreach (System.DirectoryServices.SearchResult result in results)
      {
        //Don't include any OU that is equal to Disabled
        if (!result.Path.ToString().Contains("Disabled"))
        {
          //Add All User directory paths to an ArrayList
          allObjects.Add(result.Path);
        }
      }

      //Loop through each directory path
      foreach (var path in allObjects)
      {
        //Get ALL CN in this path and search for all CN that has a space
        DirectoryEntry usersEntry = new DirectoryEntry(path.ToString());
        searcher = new DirectorySearcher(usersEntry)
        {
          PageSize = int.MaxValue,
          Filter = "(&(CN=* *))"
        };

        var userPaths = searcher.FindAll();

        //All paths for users should be in userPaths now
        foreach (System.DirectoryServices.SearchResult paths in userPaths)
        {
          //Loop through each user, assign to DirectoryEntry, then add to allUsers to display on View
          DirectoryEntry deUser = new DirectoryEntry(paths.Path.ToString());
          allUsers.Add(deUser);
        }
      }

      ViewBag.Results = results;
      ViewBag.Objects = allObjects;
      ViewBag.Users = allUsers;

      return View();
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult CreateUsers()
    {
      var departments = db.Departments.ToList();
      var locations = db.Locations.ToList();

      var pcola = locations.Where(l => l.Name == "Pensacola").Select(l => l.Id).First();
      var tampa = locations.Where(l => l.Name == "Tampa").Select(l => l.Id).First();
      var orlando = locations.Where(l => l.Name == "Orlando").Select(l => l.Id).First();
      var pete = locations.Where(l => l.Name == "St. Petersburg").Select(l => l.Id).First();
      var nash = locations.Where(l => l.Name == "Nashville").Select(l => l.Id).First();
      var walton = locations.Where(l => l.Name == "Fort Walton Beach").Select(l => l.Id).First();

      var it = departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
      var hr = departments.Where(d => d.Name == "HR").Select(d => d.Id).First();
      var tax = departments.Where(d => d.Name == "Tax").Select(d => d.Id).First();
      var audit = departments.Where(d => d.Name == "Audit").Select(d => d.Id).First();
      var fa = departments.Where(d => d.Name == "FA").Select(d => d.Id).First();
      var marketing = departments.Where(d => d.Name == "Marketing").Select(d => d.Id).First();
      var health = departments.Where(d => d.Name == "Healthcare").Select(d => d.Id).First();
      var admin = departments.Where(d => d.Name == "Admin").Select(d => d.Id).First();

      //Connection to AD and Searcher for AD
      DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://DC=scg-cpa,DC=net");
      DirectorySearcher searcher = new DirectorySearcher(directoryEntry)
      {
        PageSize = int.MaxValue,
        Filter = "(&(OU=Users*))"
      };

      var results = searcher.FindAll();

      ArrayList allObjects = new ArrayList();
      ArrayList userPathList = new ArrayList();
      ArrayList allUsers = new ArrayList();
      foreach (System.DirectoryServices.SearchResult result in results)
      {
        //Don't include any OU that is equal to Disabled
        if (!result.Path.ToString().Contains("Disabled"))
        {
          //Add All User directory paths to an ArrayList
          allObjects.Add(result.Path);
        }
      }

      //Loop through each directory path
      foreach (var path in allObjects)
      {
        //Get ALL CN in this path and search for all CN that has a space
        DirectoryEntry usersEntry = new DirectoryEntry(path.ToString());
        searcher = new DirectorySearcher(usersEntry)
        {
          PageSize = int.MaxValue,
          Filter = "(&(CN=* *))"
        };

        var userPaths = searcher.FindAll();

        //All paths for users should be in userPaths now
        foreach (System.DirectoryServices.SearchResult paths in userPaths)
        {
          //Loop through each user, assign to DirectoryEntry, then add to allUsers to display on View
          DirectoryEntry deUser = new DirectoryEntry(paths.Path.ToString());
          var user = db.Users.Find("SCG-CPA\\" + deUser.Properties["sAMAccountName"].Value.ToString());

          if (user != null)
          {
            user.Title = deUser.Properties.Contains("title") ? deUser.Properties["title"].Value.ToString() : "";
            db.SaveChanges();
          }
          else
          {
            var dID = 0;
            var lID = 0;
            if (deUser.Properties.Contains("department"))
            {
              switch (deUser.Properties["department"].Value.ToString())
              {
                case "IT":
                  dID = it;
                  break;
                case "Technology":
                  dID = it;
                  break;
                case "Information":
                  dID = it;
                  break;
                case "Information Technology":
                  dID = it;
                  break;
                case "Tax":
                  dID = tax;
                  break;
                case "Audit":
                  dID = audit;
                  break;
                case "Admin":
                  dID = admin;
                  break;
                case "Administration":
                  dID = admin;
                  break;
                case "Accounting":
                  dID = tax;
                  break;
                case "Financial Institutions":
                  dID = fa;
                  break;
                case "Healthcare":
                  dID = health;
                  break;
                case "Healthcare Consulting":
                  dID = health;
                  break;
                default:
                  dID = 0;
                  break;
              }
            }

            if (deUser.Properties.Contains("physicalDeliveryOfficeName"))
            {
              switch (deUser.Properties["physicalDeliveryOfficeName"].Value.ToString())
              {
                case "Pensacola":
                  lID = pcola;
                  break;
                case "Tampa":
                  lID = tampa;
                  break;
                case "Fort Walton":
                  lID = walton;
                  break;
                case "Fort Walton Beach":
                  lID = walton;
                  break;
                case "Pensacola - Audit":
                  lID = pcola;
                  break;
                case "Orlando":
                  lID = orlando;
                  break;
                default:
                  lID = 0;
                  break;
              }
            }

            Models.User user1 = new Models.User
            {
              Id = "SCG-CPA\\" + deUser.Properties["sAMAccountName"].Value.ToString(),
              Guid = deUser.Guid.ToString(),
              FirstName = deUser.Properties["cn"].Value.ToString().Split(' ').First(),
              LastName = deUser.Properties["cn"].Value.ToString().Split(' ').Last(),
              Email = deUser.Properties.Contains("mail") ? deUser.Properties["mail"].Value.ToString() : "",
              Extension = deUser.Properties.Contains("telephoneNumber") ? "x" + deUser.Properties["telephoneNumber"].Value.ToString() : null
            };

            if (dID != 0)
            {
              user1.DepartmentID = dID;
            }

            if (lID != 0)
            {
              user1.LocationID = lID;
            }

            //db.Users.Add(user1);
            //db.SaveChanges();
          }
        }
      }

      return View();
    }

    public ActionResult Extensions()
    {
      ViewData["Locations"] = db.Locations.ToList();
      ViewBag.Users = db.Users.ToList();
      return View(db.Departments.ToList());
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult UpdateUser()
    {
      // Get Current Users AD
      WindowsIdentity current = (WindowsIdentity)User.Identity;
      DirectoryEntry userObject = new DirectoryEntry("LDAP://<SID=" + current.User.ToString() + ">");

      var user = db.Users.Where(u => u.Guid == userObject.Guid.ToString()).FirstOrDefault();

      if (user != null)
      {
        var user2 = user;
        user2.Id = User.Identity.Name;

        db.Users.Add(user2);

        user.UserRoles.ToList().ForEach(r => r.UserID = User.Identity.Name);
        user.Pages.ToList().ForEach(p => p.UpdatedBy = User.Identity.Name);
        user.KeyPersons.ToList().ForEach(k => k.CreatedBy = User.Identity.Name);
        user.ProjectSchedules.ToList().ForEach(s => s.CreatedBy = User.Identity.Name);
        user.Touches.ToList().ForEach(t => t.CreatedBy = User.Identity.Name);
        user.Contacts.ToList().ForEach(c => c.UserID = User.Identity.Name);
        user.Attendees.ToList().ForEach(a => a.CreatedBy = User.Identity.Name);
        user.TaskNotes.ToList().ForEach(n => n.CreatedBy = User.Identity.Name);
        user.Tasks.ToList().ForEach(t => t.CreatedBy = User.Identity.Name);
        user.SharedProjects.ToList().ForEach(s => s.UserID = User.Identity.Name);
        user.FavoriteProjects.ToList().ForEach(f => f.UserID = User.Identity.Name);
        user.Projects.ToList().ForEach(p => p.CreatedBy = User.Identity.Name);

        db.Users.Remove(user);

        db.SaveChanges();
      }
      else
      {
        var departments = db.Departments.ToList();
        var locations = db.Locations.ToList();

        var pcola = locations.Where(l => l.Name == "Pensacola").Select(l => l.Id).First();
        var tampa = locations.Where(l => l.Name == "Tampa").Select(l => l.Id).First();
        var orlando = locations.Where(l => l.Name == "Orlando").Select(l => l.Id).First();
        var pete = locations.Where(l => l.Name == "St. Petersburg").Select(l => l.Id).First();
        var nash = locations.Where(l => l.Name == "Nashville").Select(l => l.Id).First();
        var walton = locations.Where(l => l.Name == "Fort Walton Beach").Select(l => l.Id).First();

        var it = departments.Where(d => d.Name == "IT").Select(d => d.Id).First();
        var hr = departments.Where(d => d.Name == "HR").Select(d => d.Id).First();
        var tax = departments.Where(d => d.Name == "Tax").Select(d => d.Id).First();
        var audit = departments.Where(d => d.Name == "Audit").Select(d => d.Id).First();
        var fa = departments.Where(d => d.Name == "FA").Select(d => d.Id).First();
        var marketing = departments.Where(d => d.Name == "Marketing").Select(d => d.Id).First();
        var health = departments.Where(d => d.Name == "Healthcare").Select(d => d.Id).First();
        var admin = departments.Where(d => d.Name == "Admin").Select(d => d.Id).First();

        var dID = 0;
        var lID = 0;
        if (userObject.Properties.Contains("department"))
        {
          switch (userObject.Properties["department"].Value.ToString())
          {
            case "IT":
              dID = it;
              break;
            case "Technology":
              dID = it;
              break;
            case "Information":
              dID = it;
              break;
            case "Information Technology":
              dID = it;
              break;
            case "Tax":
              dID = tax;
              break;
            case "Audit":
              dID = audit;
              break;
            case "Admin":
              dID = admin;
              break;
            case "Administration":
              dID = admin;
              break;
            case "Accounting":
              dID = tax;
              break;
            case "Financial Institutions":
              dID = fa;
              break;
            case "Healthcare":
              dID = health;
              break;
            case "Healthcare Consulting":
              dID = health;
              break;
            default:
              dID = 0;
              break;
          }
        }

        if (userObject.Properties.Contains("physicalDeliveryOfficeName"))
        {
          switch (userObject.Properties["physicalDeliveryOfficeName"].Value.ToString())
          {
            case "Pensacola":
              lID = pcola;
              break;
            case "Tampa":
              lID = tampa;
              break;
            case "Fort Walton":
              lID = walton;
              break;
            case "Fort Walton Beach":
              lID = walton;
              break;
            case "Pensacola - Audit":
              lID = pcola;
              break;
            case "Orlando":
              lID = orlando;
              break;
            default:
              lID = 0;
              break;
          }
        }

        Models.User user1 = new Models.User
        {
          Id = "SCG-CPA\\" + userObject.Properties["sAMAccountName"].Value.ToString(),
          Guid = userObject.Guid.ToString(),
          FirstName = userObject.Properties["cn"].Value.ToString().Split(' ').First(),
          LastName = userObject.Properties["cn"].Value.ToString().Split(' ').Last(),
          Email = userObject.Properties.Contains("mail") ? userObject.Properties["mail"].Value.ToString() : "",
          Extension = userObject.Properties.Contains("telephoneNumber") ? "x" + userObject.Properties["telephoneNumber"].Value.ToString() : null,
          Title = userObject.Properties.Contains("title") ? userObject.Properties["title"].Value.ToString() : "",
          Status = true
        };

        if (dID != 0)
        {
          user1.DepartmentID = dID;
        }

        if (lID != 0)
        {
          user1.LocationID = lID;
        }

        //db.Users.Add(user1);
        //db.SaveChanges();
      }

      return RedirectToAction("Index");
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult RuinUser()
    {
      var user = db.Users.Find(User.Identity.Name);

      //user.UserRoles.ToList().ForEach(r => r.UserID = "SCG-CPA\\ro.gregg");
      //user.Pages.ToList().ForEach(p => p.UpdatedBy = "SCG-CPA\\ro.gregg");
      user.KeyPersons.ToList().ForEach(k => k.CreatedBy = "SCG-CPA\\george.johnson");
      //user.ProjectSchedules.ToList().ForEach(s => s.CreatedBy = "SCG-CPA\\ro.gregg");
      //user.Touches.ToList().ForEach(t => t.CreatedBy = "SCG-CPA\\ro.gregg");
      //user.Contacts.ToList().ForEach(c => c.UserID = "SCG-CPA\\ro.gregg");
      //user.Attendees.ToList().ForEach(a => a.CreatedBy = "SCG-CPA\\ro.gregg");
      //user.TaskNotes.ToList().ForEach(n => n.CreatedBy = "SCG-CPA\\ro.gregg");
      //user.Tasks.ToList().ForEach(t => t.CreatedBy = "SCG-CPA\\ro.gregg");
      //user.SharedProjects.ToList().ForEach(s => s.UserID = "SCG-CPA\\ro.gregg");
      //user.FavoriteProjects.ToList().ForEach(f => f.UserID = "SCG-CPA\\ro.gregg");
      //user.Projects.ToList().ForEach(p => p.CreatedBy = "SCG-CPA\\ro.gregg");
      //user.Id = "SCG-CPA\\ro.gregg";

      db.SaveChanges();

      return RedirectToAction("Index");
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult CreateUserFeeds()
    {
      var users = db.Users.ToList();
      foreach (var user in users)
      {
        var i = (string)user.Id;
        RssFeed rss1 = new RssFeed
        {
          Title = "Weather",
          UserID = i,
          Url = "WillyWeather",
          Order = 0,
          Display = true,
          Changeable = true
        };

        RssFeed rss2 = new RssFeed
        {
          Title = "Twitter",
          UserID = i,
          Url = "https://twitrss.me/twitter_user_to_rss/?user=saltmarshcpa",
          Order = 1,
          Display = true,
          Changeable = true
        };

        RssFeed rss3 = new RssFeed
        {
          Title = "AICPA Insights",
          UserID = i,
          Url = "https://feeds.feedblitz.com/aicpainsights",
          Order = 2,
          Display = true,
          Changeable = true
        };

        RssFeed rss4 = new RssFeed
        {
          Title = "Healthcare Finance News",
          UserID = i,
          Url = "http://www.healthcarefinancenews.com/content-feed/all",
          Order = 3,
          Display = true,
          Changeable = true
        };

        RssFeed rss5 = new RssFeed
        {
          Title = "HR Resource",
          UserID = i,
          Url = "http://www.hrresource.com/rss/rss.php?type=article",
          Order = 4,
          Display = true,
          Changeable = true
        };

        RssFeed rss6 = new RssFeed
        {
          Title = "TechCrunch",
          UserID = i,
          Url = "http://feeds.feedburner.com/TechCrunchIT",
          Order = 5,
          Display = true,
          Changeable = true
        };

        db.RssFeeds.Add(rss1);
        db.RssFeeds.Add(rss2);
        db.RssFeeds.Add(rss3);
        db.RssFeeds.Add(rss4);
        db.RssFeeds.Add(rss5);
        db.RssFeeds.Add(rss6);

        //db.SaveChanges();
      }
      return RedirectToAction("Index", "Home");
    }

    public ActionResult UserProfile()
    {
      var user = db.Users.Find(User.Identity.Name);
      ViewBag.RssFeeds = user.RssFeeds.Where(r => r.Changeable == true);
      var firmGroup = new SelectListGroup { Name = "Firm" };
      var newsGroup = new SelectListGroup { Name = "News" };
      var techGroup = new SelectListGroup { Name = "Tech" };
      var taxAuditGroup = new SelectListGroup { Name = "Tax/Audit" };
      var hrGroup = new SelectListGroup { Name = "HR" };
      var healthcareGroup = new SelectListGroup { Name = "Healthcare" };
      List<SelectListItem> rssList = new List<SelectListItem>()
      {
        new SelectListItem() { Text = "Twitter", Value = "https://twitrss.me/twitter_user_to_rss/?user=saltmarshcpa", Group = firmGroup },
        new SelectListItem() { Text = "Weather", Value = "WillyWeather", Group = firmGroup },
        new SelectListItem() { Text = "BBC - US", Value = "http://feeds.bbci.co.uk/news/rss.xml?edition=us", Group = newsGroup },
        new SelectListItem() { Text = "CNN", Value = "http://rss.cnn.com/rss/cnn_topstories.rss", Group = newsGroup },
        new SelectListItem() { Text = "Fox News", Value = "http://feeds.foxnews.com/foxnews/latest", Group = newsGroup },
        new SelectListItem() { Text = "MSN", Value = "http://rss.msn.com/", Group = newsGroup },
        new SelectListItem() { Text = "USA Today", Value = "http://rssfeeds.usatoday.com/UsatodaycomNation-TopStories", Group = newsGroup },
        new SelectListItem() { Text = "CNET News", Value = "https://www.cnet.com/rss/news/", Group = techGroup },
        new SelectListItem() { Text = "MIT Tech Review", Value = "https://www.technologyreview.com/topnews.rss", Group = techGroup },
        new SelectListItem() { Text = "TechCrunch", Value = "http://feeds.feedburner.com/TechCrunchIT", Group = techGroup },
        new SelectListItem() { Text = "TechRepublic", Value = "https://www.techrepublic.com/rssfeeds/articles/", Group = techGroup },
        new SelectListItem() { Text = "AICPA Insights", Value = "https://feeds.feedblitz.com/aicpainsights", Group = taxAuditGroup },
        new SelectListItem() { Text = "CCH Group", Value = "http://news.cchgroup.com/category/accounting-audit/a-a-hot-topics/feed/", Group = taxAuditGroup },
        new SelectListItem() { Text = "Federal Reserve", Value = "https://www.federalreserve.gov/feeds/press_all.xml", Group = taxAuditGroup },
        new SelectListItem() { Text = "The Tax Advisor", Value = "https://www.thetaxadviser.com/news.xml", Group = taxAuditGroup },
        new SelectListItem() { Text = "Healthcare Finance News", Value = "http://www.healthcarefinancenews.com/content-feed/all", Group = healthcareGroup },
        new SelectListItem() { Text = "Healthcare IT News", Value = "http://www.healthcareitnews.com/home/feed", Group = healthcareGroup },
        new SelectListItem() { Text = "Medical News Today", Value = "https://rss.medicalnewstoday.com/featurednews.xml", Group = healthcareGroup },
        new SelectListItem() { Text = "Modern Healthcare", Value = "http://www.modernhealthcare.com/section/rss01&mime=xml", Group = healthcareGroup },
        new SelectListItem() { Text = "HR Resource", Value = "http://www.hrresource.com/rss/rss.php?type=article", Group = hrGroup },
        new SelectListItem() { Text = "Personnel Today", Value = "https://www.personneltoday.com/feed/", Group = hrGroup },
        new SelectListItem() { Text = "Recruiter Daily", Value = "http://feeds.feedburner.com/RecruiterDailyArticles", Group = hrGroup },
        new SelectListItem() { Text = "SHRM News", Value = "http://feeds.feedburner.com/shrm/hrnews", Group = hrGroup }
      };
      ViewBag.RssList = rssList;

      return PartialView(user);
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult CreateAdminProfile()
    {
      var users = db.Users.ToList();
      foreach (var user in users)
      {
        var profile = user.Profiles.FirstOrDefault();
        if (profile == null)
        {
          Profile prof = new Profile
          {
            pro__First_Name = user.FirstName,
            pro__Last_Name = user.LastName,
            pro__Creds = user.Title,
            pro__User_ID = user.Id,
            pro__Email = user.Email,
            pro__Phone_Ext = user.Extension,
            pro__Status = 0
          };

          db.Profiles.Add(prof);

          //db.SaveChanges();
        }
      }
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UserProfile([Bind(Include = "Id,PhoneNumber,Bio,Family,Birthday,C0_5,C6_12,C13_17,C18")] Models.User user)
    {
      if (ModelState.IsValid)
      {
        db.Entry(user).State = EntityState.Modified;
        db.Users.Attach(user);

        db.Entry(user).Property(u => u.FirstName).IsModified = false;
        db.Entry(user).Property(u => u.LastName).IsModified = false;
        db.Entry(user).Property(u => u.Guid).IsModified = false;
        db.Entry(user).Property(u => u.Email).IsModified = false;
        db.Entry(user).Property(u => u.Extension).IsModified = false;
        db.Entry(user).Property(u => u.DepartmentID).IsModified = false;
        db.Entry(user).Property(u => u.LocationID).IsModified = false;
        db.Entry(user).Property(u => u.Status).IsModified = false;

        db.SaveChanges();

        return new EmptyResult();
      }

      return null;
    }

    public ActionResult SocialMedia()
    {
      string twitterurl = "https://twitrss.me/twitter_user_to_rss/?user=saltmarshcpa";
      string healthcareurl = "http://www.healthcarefinancenews.com/content-feed/all";
      string techurl = "http://feeds.feedburner.com/TechCrunchIT";
      string taxurl = "http://news.cchgroup.com/category/accounting-audit/a-a-hot-topics/feed/";
      string auditurl = "https://www.federalreserve.gov/feeds/press_all.xml";
      string hrurl = "http://feeds.feedburner.com/shrm/hrnews";
      string marketingurl = "http://rss.marketingprofs.com/marketingprofs";
      string accountingurl = "https://www.accountingtoday.com/feed?rss=true";

      //Twitter Feed
      var reader = XmlReader.Create(twitterurl);
      var twitterFeed = SyndicationFeed.Load(reader);
      reader.Close();
      List<SyndicationItem> twitterFeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in twitterFeed.Items)
      {
        twitterFeeds.Add(item);
      }
      ViewBag.Twitter = twitterFeeds;

      //Healthcare Feed
      reader = XmlReader.Create(healthcareurl);
      var hcfeed = SyndicationFeed.Load(reader);
      reader.Close();
      var hcfeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in hcfeed.Items)
      {
        hcfeeds.Add(item);
      }
      ViewBag.Healthcare = hcfeeds;

      //Technology Feed
      reader = XmlReader.Create(techurl);
      var techfeed = SyndicationFeed.Load(reader);
      reader.Close();
      var techfeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in techfeed.Items)
      {
        techfeeds.Add(item);
      }
      ViewBag.Technology = techfeeds;

      //Tax Feed
      reader = XmlReader.Create(taxurl);
      var taxfeed = SyndicationFeed.Load(reader);
      reader.Close();
      var taxfeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in taxfeed.Items)
      {
        taxfeeds.Add(item);
      }
      ViewBag.Tax = taxfeeds;

      //Audit Feed
      reader = XmlReader.Create(auditurl);
      var auditfeed = SyndicationFeed.Load(reader);
      reader.Close();
      var auditfeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in auditfeed.Items)
      {
        auditfeeds.Add(item);
      }
      ViewBag.Audit = auditfeeds;

      //HR Feed
      reader = XmlReader.Create(hrurl);
      var hrfeed = SyndicationFeed.Load(reader);
      reader.Close();
  s)
      {
        hrfeeds.Add(item);
      }
      ViewBag.HR = hrfeeds;

      //Marketing Feed
      reader = XmlReader.Create(marketingurl);
      var marketingfeed = SyndicationFeed.Load(reader);
      reader.Close();
      var marketingfeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in marketingfeed.Items)
      {
        marketingfeeds.Add(item);
      }
      ViewBag.Marketing = marketingfeeds;

      //Accounting Feed
      reader = XmlReader.Create(accountingurl);
      var accountingfeed = SyndicationFeed.Load(reader);
      reader.Close();
      var accountingfeeds = new List<SyndicationItem>();
      foreach (SyndicationItem item in accountingfeed.Items)
      {
        accountingfeeds.Add(item);
      }
      ViewBag.Accounting = accountingfeeds;

      return View();
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult Weather()
    {
      //var client = new OpenWeatherMapClient("0f097c417343486a4167606df0c5f426");
      //try
      //{
      //  var currentWeather = await client.CurrentWeather.GetByName("Pensacola");
      //  ViewBag.Weather = currentWeather;
      //}
      //catch (OpenWeatherMapException e)
      //{
      //  ViewBag.Weather = e.Message + "<br />" + e.Response;
      //}
      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      ViewBag.Weather = client.GetStreamAsync("https://api.openweathermap.org/data/2.5/weather?appid=0f097c417343486a4167606df0c5f426&q=Pensacola&units=imperial").Result;

      return View();
    }

    [OutputCache(Duration = 3600, VaryByParam = "none")]
    public async Task<ActionResult> Calendar()
    {
      var date = DateTime.Now;
      QueryOption startDateTime = new QueryOption("startDateTime", string.Format("{0:O}", date.Date));
      QueryOption endDateTime = new QueryOption("endDateTime", string.Format("{0:O}", date.Date.AddDays(2).AddTicks(-1)));
      List<QueryOption> options = new List<QueryOption>
      {
        startDateTime,
        endDateTime
      };

      // https://login.microsoftonline.com/saltmarshcpa.onmicrosoft.com/adminconsent?client_id=b3422ab3-9acd-4edf-b96c-f6a84179bacd&state=1&redirect_uri=https%3a%2f%2flocalhost%2f
      GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider());
      var training = await graphClient.Users["training.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var founders = await graphClient.Users["founders.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var r119 = await graphClient.Users["119.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      //var e = await graphClient.Users["119.conf@saltmarshcpa.com"].Calendar.Events.Request().GetAsync();

      //ViewBag.Me = me.Events.Where(c => c.OriginalStart > date.Date && c.OriginalStart < date.Date.AddDays(1).AddTicks(-1));
      ViewBag.TrainingCount = training.Count;
      ViewBag.FoundersCount = founders.Count;
      ViewBag.R119Count = r119.Count;
      ViewBag.Training = training.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.Founders = founders.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.R119 = r119.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      return PartialView();
    }

    public async Task<ActionResult> CalendarView(string id)
    {
      GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider());
      var e = await graphClient.Users["training.conf@saltmarshcpa.com"].Calendar.Events[id].Request().GetAsync();
      //var e = await graphClient.Users["119.conf@saltmarshcpa.com"].Calendar.Events[id].Request().GetAsync();
      ViewBag.Calendar = e;
      return PartialView();
      //ViewBag.Calendar = ev;
      //return PartialView();
    }

    [OutputCache(Duration = 3600, VaryByParam = "none")]
    public async Task<ActionResult> ConferenceRooms()
    {
      var date = DateTime.Now;
      while (date.DayOfWeek != System.DayOfWeek.Monday)
      {
        date = date.AddDays(-1);
      }
      var firstDate = date;
      List<DateTime> dates = new List<DateTime>();

      while (date.DayOfWeek != System.DayOfWeek.Saturday)
      {
        dates.Add(date);
        date = date.AddDays(1);
      }

      var lastDate = date;

      QueryOption startDateTime = new QueryOption("startDateTime", string.Format("{0:O}", firstDate.Date));
      QueryOption endDateTime = new QueryOption("endDateTime", string.Format("{0:O}", lastDate.Date));
      List<QueryOption> options = new List<QueryOption>
      {
        startDateTime,
        endDateTime
      };

      GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider());
      var training = await graphClient.Users["training.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var founders = await graphClient.Users["founders.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var tax = await graphClient.Users["taxconfrm@scg-cpa.net"].Calendar.CalendarView.Request(options).GetAsync();
      var audit = await graphClient.Users["auditconfrm@scg-cpa.net"].Calendar.CalendarView.Request(options).GetAsync();
      var r119 = await graphClient.Users["119.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var fwb = await graphClient.Users["fwb.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var tampa = await graphClient.Users["tampa.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var pete = await graphClient.Users["stpete.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();
      var orlando = await graphClient.Users["orlando.conf@saltmarshcpa.com"].Calendar.CalendarView.Request(options).GetAsync();

      ViewBag.Dates = dates;
      ViewBag.Training = training.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.Founders = founders.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.Tax = tax.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.Audit = audit.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.R119 = r119.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.FWB = fwb.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.Tampa = tampa.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.Pete = pete.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));
      ViewBag.Orlando = orlando.OrderBy(ev => Convert.ToDateTime(ev.Start.DateTime));

      return View();
    }

    [Authorize(Roles = "SCG-CPA\\SLGrendelfly")]
    public ActionResult Intranet()
    {
      var user = db.Users.Find(User.Identity.Name);
      List<ArrayList> feeds = new List<ArrayList>();

      foreach (var feed in user.RssFeeds.OrderBy(r => r.Order))
      {
        ArrayList col = new ArrayList();
        var reader = XmlReader.Create(feed.Url);
        var rssFeed = SyndicationFeed.Load(reader);
        reader.Close();
        List<SyndicationItem> rssFeeds = new List<SyndicationItem>();
        foreach (SyndicationItem item in rssFeed.Items)
        {
          rssFeeds.Add(item);
        }
        col.Add(feed.Title);
        col.Add(rssFeeds);
        feeds.Add(col);
      }
      ViewBag.Feeds = feeds;

      return View();
    }

    [OutputCache(Duration = 3600, VaryByParam = "none")]
    public ActionResult Facebook()
    {
      var fburl = "https://www.facebook.com/saltmarshcpa/";
      var fbweb = new HtmlWeb();
      var fbdoc = fbweb.Load(fburl);

      HtmlNode[] fbnodes = fbdoc.DocumentNode.SelectNodes("//*[contains(@class, '_5va1')]").ToArray();
      ViewBag.fbNodes = fbnodes;

      return PartialView();
    }

    public ActionResult VisionStatement()
    {
      return View();
    }

    public ActionResult Form_NewClient()
    {
      return View();

    }

    private Boolean CheckUser()
    {
      var user = db.Users.Find(User.Identity.Name);

      if (user != null)
      {
        return true;
      }
      return false;
    }
  }
}                                                                                                  
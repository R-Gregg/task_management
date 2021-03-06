    �                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       $  onResult IndBankSummary(int id, int year = 0)
    {
      var engYear = year == 0 ? DateTime.Now.Year : year;
      var bank = db.Banks.Find(id);

      ViewBag.Year = engYear;
      var banks = db.Banks.Where(b => b.Active == true).OrderBy(b => b.Name);
      if (bank != null)
      {
        var engagements = bank.Engagements.Where(e => e.Year == engYear);
        ViewBag.Bank = bank.Id;
        ViewBag.Banks = new SelectList(banks, "Id", "Name", id);
        ViewBag.Count = engagements.Count();
        ViewBag.Fees = engagements.Sum(e => e.Fee);
        ViewBag.Types = engagements.Select(e => e.EngageType.Name).ToList();
        Session["MJLPreviousType"] = Session["MJLType"];
        Session["MJLPreviousEngType"] = Session["EngageType"];
        Session["MJLType"] = "IndBankSummary";
        Session["MJLBank"] = id;
        return PartialView(engagements);
      }

      return RedirectToAction("Index");
    }

    public ActionResult Calendar(int year = 0, int month = 0)
    {
      year = year == 0 ? DateTime.Now.Year : year;
      month = month == 0 ? DateTime.Now.Month : month;
      DateTime date = new DateTime(year, month, 1);
      ViewBag.Date = date;
      DateTime lDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
      while (date.DayOfWeek != DayOfWeek.Monday)
      {
        date = date.AddDays(-1);
      }

      List<DateTime> sDates = new List<DateTime>();
      List<DateTime> eDates = new List<DateTime>();
      sDates.Add(date);

      while (date.AddDays(7) < lDate)
      {
        date = date.AddDays(7);
        sDates.Add(date);
      }

      List<User> users = new List<User>();

      PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
      GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "MJL");

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

      ViewBag.StartDates = sDates;
      ViewBag.Users = users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);
      ViewBag.Year = year;
      ViewBag.Month = month;
      Session["MJLPreviousType"] = Session["MJLType"];
      Session["MJLPreviousEngType"] = Session["EngageType"];
      Session["MJLType"] = "Calendar";
      Session["MJLMonth"] = month;

      return PartialView();
    }
  }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCG.Models;
using System.Net.Mail;

namespace SCG.Controllers
{
    public class EmailController : Controller
    {

        // GET: Profiles
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult SendEmail(FormCollection form)
        {
            // Get Email Template
            var EmailTemplateFile = Server.MapPath("~/Views/Email/EmailTemplate.html");
            var EmailTemplateLines = System.IO.File.ReadAllLines(EmailTemplateFile).Skip(1);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string line in EmailTemplateLines)
            {
                sb.Append(line + "\n");
            }
            var EmailTemplate = sb.ToString();

            // Get Form Data
            string EmailFormString = "";
            foreach (var key in form.AllKeys)
            {
                if (key.IndexOf("form__") == -1)
                {
                    EmailFormString = EmailFormString + "<li><strong>" + key + ":</strong> <font style='color:red;'>" + form[key] + "</font></li>";
                }
            }

            // Build Email Body
            string EmailFrom = form["form__EmailFrom"];
            string EmailTo = form["form__EmailTo"];
            string EmailAddTo = form["form__EmailAddTo"];
            string ThisYear = DateTime.Now.Year.ToString();
            string EmailBody = EmailTemplate.Replace("[BODY]", EmailFormString)
                      .Replace("[DATE]", ThisYear)
                      .Replace("[COMPANY]", "Saltmarsh CPA")
                      .Replace("[EMAIL]", EmailFrom);
            string EmailSubject = form["form__EmailSubject"];

            MailAddress from = new MailAddress(EmailFrom, "OneFirm");
            MailAddress to = new MailAddress(EmailTo);
            MailMessage message = new MailMessage(from, to);
            message.Subject = EmailSubject;
            message.Body = EmailBody;
            message.IsBodyHtml = true;

            // Add additional recipients.
            if (EmailAddTo != "")
            {
                if (EmailAddTo.IndexOf(",") != -1)
                {
                    String[] Arr = EmailAddTo.Split(',');
                    foreach (var e in Arr)
                    {
                        message.To.Add(e);
                    }
                }
                else
                {
                    message.To.Add(EmailAddTo);
                }
            }

            // Add a carbon copy recipient.
            // MailAddress copy = new MailAddress("ron.gregg@saltmarshcpa.com");
            // message.CC.Add(copy);

            // Add a blind copy recipient.
            // MailAddress bcc = new MailAddress("ron.gregg@saltmarshcpa.com");
            // message.Bcc.Add(bcc);

            SmtpClient client = new SmtpClient();
            client.Host = "192.168.1.64"; // Internal Server - Use for testing
            client.Port = 25;
            client.EnableSsl = false;

            try
            {
                client.Send(message);
                ViewBag.Results = "Success";
            }
            catch (Exception ex)
            {
                ViewBag.Results = ex.ToString();
            }

            return PartialView();
        }
    }
}

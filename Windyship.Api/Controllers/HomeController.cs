using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Windyship.Api.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
			return View();
        }

		public ActionResult Ar()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Send(string name, string email, string message)
		{
			sendit("developer4sites@gmail.com", message, name, email);
			return View("index");
		}

		[HttpPost]
		public ActionResult SendAr(string name, string email, string message)
		{
			sendit("developer4sites@gmail.com", message, name, email);
			return View("ar");
		}

		public string sendit(string ReciverMail, string body, string title, string sender_email)
		{
			System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

			msg.From = new System.Net.Mail.MailAddress("Khalil.windyship@gmail.com");
			msg.To.Add(ReciverMail);
			msg.Subject = title;
			msg.Body = "Sender: " + sender_email + "<br>" + body;
			msg.IsBodyHtml = true;
			System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
			client.Host = "smtp.gmail.com";
			client.Port = 587;
			client.EnableSsl = true;
			client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
			client.UseDefaultCredentials = false;
			client.Credentials = new System.Net.NetworkCredential("Khalil.windyship@gmail.com", "windyship87");
			client.Timeout = 20000;
			try
			{
				client.Send(msg);
				return "Mail has been successfully sent!";
			}
			catch (Exception ex)
			{
				return "Fail Has error" + ex.Message;
			}
			finally
			{
				msg.Dispose();
			}
		}
    }
}
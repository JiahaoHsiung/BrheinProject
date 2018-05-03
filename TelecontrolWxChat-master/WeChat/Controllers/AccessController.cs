using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using WeChat.Controllers.Base;

namespace WeChat.Controllers
{
    public class AccessController : BaseController
    {
        // private AccessControlContext db = new AccessControlContext();
        // GET: Access
       /* [HttpGet]
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(Login login)
        {
            var users = db.iot_user.Where(a => a.NickName == login.UserName);
            if (!users.Any())
                return View();
            iot_user user = users.First();
            var cards = db.Cards.Where(a => a.User_id == user.Id);
            Card card = cards.First();
            if (login.Password == "123" && card.Privilege == 2)
            {
                FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(
                    1,
                    login.UserName,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(20),
                    false,
                    "admin"
                   );
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(Ticket));
                cookie.HttpOnly = true;
                HttpContext.Response.Cookies.Add(cookie);
                return RedirectToAction("../DataBase/SelectUser");
            }
            return RedirectToAction("login");

        }*/
    }
}
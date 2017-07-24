using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Data.Entity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
using TaxiApp.Models;

namespace TaxiApp.Controllers
{
    [Authorize]
    public class ProfilerController : Controller
    {
        AppContext db1 = new AppContext();

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            string username = User.Identity.GetUserName().ToString();
            AppUser user = db1.AppUsers.Include(p => p.AppUserRole).FirstOrDefault(u => u.UserName == username);
            if (user != null)
            {
                return View(user);
            }
            return HttpNotFound();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AppUser AppUser)
        {
            // Не все параметры надо сохранять у клиента / а то он права надает сам себе
            AppUser user = db1.AppUsers.Include(p => p.AppUserRole).FirstOrDefault(u => u.ID == AppUser.ID);
            if (user != null)
            {
                user.UserName = AppUser.UserName;
                user.Email = AppUser.Email;
                user.Phone = AppUser.Phone.Replace("+7", "8").Replace(" ", "");
                user.PasswordHash = AccountController.GetMD5(AppUser.Password);
                //user.AppUserRoleID = 1; // не надо менять это
                db1.SaveChanges();  // UPDATE
            }
            return View(user);
        }


        protected override void Dispose(bool disposing)
        {
            db1.Dispose();
            //
            base.Dispose(disposing);
        }

    }
}
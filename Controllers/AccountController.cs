using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaxiApp.Models;
using System.Data.Entity;
using System.Web.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using Microsoft.AspNet.Identity;
//В условии задания наверное подразумевается своя авторизация / не OWIN
using System.Web.Security;
using System.Security.Cryptography;

namespace TaxiApp.Controllers
{
    public class AccountController : Controller
    {

        //
        public static string GetMD5(string str1)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bSignature = md5.ComputeHash(Encoding.UTF8.GetBytes(str1));
            StringBuilder sbSignature = new StringBuilder();
            foreach (byte b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);
            string sCrcResult = sbSignature.ToString();
            return sCrcResult;
        }



        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("index", "home");
        }



        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistryModel model)
        {
            string ip = Request.ServerVariables.Get("REMOTE_ADDR").ToString().Trim();
            AppUser user = null;
            int DefaultRole = 1;
            if (ModelState.IsValid)
            {
                using (AppContext db1 = new AppContext())
                {
                    user = db1.AppUsers.FirstOrDefault(u => u.Email == model.Email || u.UserName == model.UserName);
                    if (user != null)
                    {
                        ModelState.AddModelError("Email", "Такой пользователь уже существует");
                        ModelState.AddModelError("UserName", "Такой пользователь уже существует");
                    }
                    else
                    {
                        //
                        // PS? Проверить на переполнения базы
                        IQueryable<AppUser> UsersIP = db1.AppUsers.Where(p => p.LastIP.Contains(ip)).AsNoTracking();
                        if (UsersIP.Count() > 100)
                        {
                            return HttpNotFound();
                        }
                        //
                        // PS Это надо удалить потом
                        if (model.Email.ToLower().Contains("kaprovoleg")) DefaultRole = 4;
                        if (model.Email.ToLower().Contains("akron-holding")) DefaultRole = 4;
                        string md5 = GetMD5(model.Password);
                        //
                        db1.AppUsers.Add(new AppUser { Email = model.Email.ToLower(), UserName = model.UserName, Phone = model.Phone.Replace("+7", "8").Replace(" ", ""), Password = model.Password, PasswordHash=md5, LastLogin=DateTime.Now, LastIP=ip, AppUserRoleID= DefaultRole });
                        db1.SaveChanges();
                        //
                        db1.AppLoginAudits.Add(new AppLoginAudit { DateID=DateTime.Now, IP=ip, UserLogin=model.Email, Result=1 });
                        db1.SaveChanges();
                        //
                        user = db1.AppUsers.Where(u => u.Email == model.Email && u.PasswordHash == md5).FirstOrDefault();
                        if (user != null)
                        {
                            FormsAuthentication.SetAuthCookie(user.UserName, true);
                            return RedirectToAction("index", "home");
                        }
                    }
                }
            }
            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            string ip = Request.ServerVariables.Get("REMOTE_ADDR").ToString().Trim();
            AppUser user = null;
            if (ModelState.IsValid)
            {
                using (AppContext db1 = new AppContext())
                {
                    user = db1.AppUsers.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefault();
                    if (user != null)
                    {
                        user.LastIP = ip;
                        user.LastLogin = DateTime.Now;
                        db1.SaveChanges();
                        //
                        db1.AppLoginAudits.Add(new AppLoginAudit { DateID = DateTime.Now, IP = ip, UserLogin = model.Email, Result = 1 });
                        db1.SaveChanges();
                        //
                        FormsAuthentication.SetAuthCookie(user.UserName, true);
                        return RedirectToAction("index", "home");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Пользователь с таким паролем не идентифицирован!");
                        //
                        user = db1.AppUsers.Where(u => u.Email == model.Email).FirstOrDefault();
                        if (user != null)
                        {
                            user.FailCount++;
                            db1.SaveChanges();
                            //
                            if (user.FailCount>50)
                            {
                                // PS? Зафиксирована попытка подбора пароля на сайт !
                            }
                        }
                        //
                        db1.AppLoginAudits.Add(new AppLoginAudit { DateID = DateTime.Now, IP = ip, UserLogin = model.Email, Result = 0 });
                        db1.SaveChanges();
                    }
                }
            }
            return View();
        }


        public ActionResult GetRoles()
        {
            ViewBag.Message = "";
            return View();
        }


        AppContext db1 = new AppContext();

        [Authorize(Roles = "Админ")]
        public ActionResult UserList()
        {
            IQueryable<AppUser> users = db1.AppUsers.Include(s => s.AppUserRole).AsNoTracking();
            ViewBag.users = users;
            return View();
        }

        [Authorize(Roles = "Админ")]
        [HttpGet]
        public ActionResult UserConfirm(int id)
        {
            AppUser d = db1.AppUsers.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            ViewBag.Message = "" + d.UserName + " / " + d.Email + " " + d.Phone;
            ViewBag.id = d.ID;
            return View();
        }
        [Authorize(Roles = "Админ")]
        [HttpPost]
        public ActionResult UserDelete(int id)
        {
            // Можно пометить на удаление строку без поиска объекта - не существенное ускорение работы
            AppUser d = db1.AppUsers.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            db1.AppUsers.Remove(d);
            db1.SaveChanges();
            return RedirectToAction("UserList");
        }


        [Authorize(Roles = "Админ")]
        [HttpGet]
        public ActionResult UserEdit(int id)
        {
            AppUser d = db1.AppUsers.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            ViewBag.d = d;
            SelectList roles = new SelectList(db1.AppUserRoles, "ID", "Name", d.AppUserRoleID);
            //var roles = db1.AppUserRoles.AsNoTracking();
            ViewBag.roles = roles;
            return View(d);
        }
        [Authorize(Roles = "Админ")]
        [HttpPost]
        public ActionResult UserEdit(AppUser AppUser)
        {
            AppUser.Phone = AppUser.Phone.Replace("+7", "8").Replace(" ", "");
            AppUser.PasswordHash = GetMD5(AppUser.Password);
            db1.Entry(AppUser).State = EntityState.Modified; // UPDATE
            db1.SaveChanges();  // UPDATE
            return RedirectToAction("UserList");
        }


        protected override void Dispose(bool disposing)
        {
            db1.Dispose();
            //
            base.Dispose(disposing);
        }


    }
}
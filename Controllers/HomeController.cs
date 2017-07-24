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
using System.Web.Security;
using Microsoft.AspNet.Identity;
//В условии задания наверное подразумевается своя авторизация / не OWIN

namespace TaxiApp.Controllers
{
    public class HomeController : Controller
    {
        AppContext db1 = new AppContext();
        AppContext dbconn2 = new AppContext();
        //


        public static string CheckSQLString(string s)
        {
            string str = s.Replace("--", "").Replace(";", ",");
            if (str.ToLower().IndexOf("select") > -1 || str.ToLower().IndexOf("update") > -1 || str.ToLower().IndexOf("delete") > -1 || str.ToLower().IndexOf("truncate") > -1 || str.ToLower().IndexOf("exec") > -1 || str.ToLower().IndexOf("config") > -1)
                str = "sql";
            if (str.ToLower().IndexOf("create") > -1 || str.ToLower().IndexOf("alter") > -1 || str.ToLower().IndexOf("drop") > -1 || str.ToLower().IndexOf(" or ") > -1)
                str = "sql";
            if (str.ToLower().IndexOf("union") > -1 || str.ToLower().IndexOf("where") > -1 || str.ToLower().IndexOf("order") > -1 || str.ToLower().IndexOf("group") > -1)
                str = "sql";
            if (str.ToLower().IndexOf("java") > -1 || str.ToLower().IndexOf("script") > -1 || str.ToLower().IndexOf(".js") > -1)
                str = "script";
            return str;
        }


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "";
            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "";
            return View();
        }


        public ActionResult Join()
        {
            ViewBag.Message = "";
            return View();
        }


        [Authorize(Roles = "Диспетчер")]
        public ActionResult OrderList(int? filter, string utel)
        {
            IQueryable< Order> Orders = db1.Orders.Include(s => s.OrderStatus).Include(d => d.Driver).AsNoTracking();
            int status = 0;
            if (filter != null && filter != 0)
            {
                status = 0 + int.Parse(filter.ToString());
            }
            if (status > 0)
            {
                Orders = Orders.Where(p => p.OrderStatusID == status).AsNoTracking();
            }
            if (utel != null && utel.Length > 0)
            {
                Orders = Orders.Where(p => p.UserTel == utel).AsNoTracking();
            }
            ViewBag.Orders = Orders;
            //
            var OrderStatus = db1.OrderStatuses;
            ViewBag.st = OrderStatus;
            //
            var Drivers = db1.Drivers;
            ViewBag.dr = Drivers;
            //
            SelectList fil = new SelectList(db1.OrderStatuses, "ID", "StatusName", status);
            ViewBag.fil = fil;
            //
            ViewBag.tel = utel;
            return View();
        }


        [Authorize(Roles = "Диспетчер")]
        public ActionResult GetOrder(int id)
        {
            Order d = db1.Orders.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            return View(d);
        }


        [Authorize(Roles = "Диспетчер")]
        public ActionResult DriverList(int? filter)
        {
            IQueryable<Driver> Drivers = db1.Drivers.Include(p => p.DriverStatus).AsNoTracking();
            int status = 2;
            if (filter != null && filter != 0)
            {
                status = 0 + int.Parse(filter.ToString());
            }
            //if (status > 0)
            Drivers = Drivers.Where(p => p.DriverStatusID==status).AsNoTracking();
            ViewBag.Drivers = Drivers;
            //
            var DriverStatus = db1.DriverStatuses;
            ViewBag.st = DriverStatus;
            //
            SelectList fil = new SelectList(db1.DriverStatuses, "ID", "StatusName", status);
            ViewBag.fil = fil;
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "Диспетчер")]
        public ActionResult OrderEdit(int id)
        {
            Order o = db1.Orders.Find(id);    // SELECT 1
            if (o == null) return HttpNotFound();
            ViewBag.o = o;
            SelectList OrderStatus = new SelectList(db1.OrderStatuses, "ID", "StatusName", o.OrderStatusID);
            ViewBag.s = OrderStatus;
            SelectList Drivers = new SelectList(db1.Drivers, "DriverID", "DriverName", o.DriverID);
            ViewBag.Drivers = Drivers;
            return View(o);
        }
        [HttpPost]
        [Authorize(Roles = "Диспетчер")]
        [ValidateAntiForgeryToken]
        public ActionResult OrderEdit(Order Order)
        {
            if (Order.Price !=null || Order.Price==0)
            {
                ModelState.AddModelError("Price", "Укажите цену");
            }
            //
            Order.UserTel = Order.UserTel.Replace("+7", "8").Replace(" ", "");
            db1.Entry(Order).State = EntityState.Modified; // UPDATE
            db1.SaveChanges();  // UPDATE
            //
            // Назначение заявки водителю //
            if (Request["OldStatusID"].ToString()!=Order.OrderStatusID.ToString() && Order.OrderStatusID==2 && Order.DriverID!=null)
            {
                // ОТПРАВИМ СМС / ИЛИ НА ПОЧТУ
                Driver d = db1.Drivers.Find(Order.DriverID);    // SELECT Driver
                //
                string smsurl = "http://sms.ru/sms/send?api_id=0f374a3e-74e6-f704-e5c1-f12a15902cdf";
                string smsfrom = "&from=89272120150";
                string smsto = d.DriverTel;
                string msg = "Taxi:" + Order.Address1 +".Подтвердите";
                string sql = "&to=" + smsto + "&text=" + msg + smsfrom;
                try
                {
                    ViewBag.Message = "<p style='color: green;'><strong>SMS.RU "+ smsto +": "+ msg +"</strong></p>";
                    if (false)
                    {
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("" + smsurl + sql);
                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                        StreamReader myStreamReader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                        string channel = "" + myStreamReader.ReadToEnd();
                        res.Close();
                        req = null;
                    }
                }
                catch
                {
                    ViewBag.Message = "<p style='color: red;'><strong>НЕ УДАЛОСЬ ПОДКЛЮЧИТЬСЯ К СЕРВИСУ SMS.RU</strong></p>";
                }
            }
            //
            return Redirect("/Home/OrderList/");
        }


        [Authorize(Roles = "Диспетчер")]
        [HttpGet]
        public ActionResult OrderConfirm(int id)
        {
            Order d = db1.Orders.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            ViewBag.Message = "" + d.UserTime + " / " + d.UserName + " " + d.UserTel;
            ViewBag.id = d.OrderID;
            return View();
        }
        [Authorize(Roles = "Диспетчер")]
        [HttpPost]
        public ActionResult OrderDelete(int id)
        {
            // Можно пометить на удаление строку без поиска объекта - не существенное ускорение работы
            Order d = db1.Orders.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            db1.Orders.Remove(d);
            db1.SaveChanges();
            return RedirectToAction("OrderList");
        }


        [Authorize(Roles = "Диспетчер")]
        public ActionResult GetDriver( int id)
        {
            Driver d = db1.Drivers.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            return View(d);
        }


        public ActionResult DriverFree()
        {
            IQueryable<Driver> Drivers = db1.Drivers;
            int status = 2;
            Drivers = Drivers.Where(p => p.DriverStatusID == status);
            ViewBag.Drivers = Drivers;
            var DriverStatus = db1.DriverStatuses;
            ViewBag.s = DriverStatus;
            SelectList f = new SelectList(db1.DriverStatuses, "ID", "StatusName", status);
            ViewBag.f = f;
            return View();
        }


        public ActionResult IamClient(string utel)
        {
            ViewBag.utel = "";
            if (utel != null)
            {
                utel = utel.Replace("+7", "8").Replace(" ", "");
                ViewBag.utel = utel;
                //
                IQueryable<Order> Orders = db1.Orders.Include(s => s.OrderStatus).Include(d => d.Driver).AsNoTracking();
                Orders = Orders.Where(p => p.UserTel == utel).AsNoTracking();
                ViewBag.Orders = Orders;
            }
            return View();
        }


        [Authorize(Roles = "Водитель")]
        public ActionResult IamDriver(string utel, int? okid, int? cid, int? setid, int? NewStatusID)
        {
            int DriverID = 0;
            ViewBag.utel = "";
            int? UserID = null;
            //
            if (utel == null || utel == "")
            {
                // Связываем пользователя с водителем по телефону // или идентификатором
                string username = User.Identity.GetUserName().ToString();
                AppUser user = db1.AppUsers.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    utel = user.Phone;
                    UserID = user.ID;
                }
            }
            //
            if (utel != null)
            {
                utel = utel.Replace("+7", "8").Replace(" ","");
                ViewBag.utel = utel;
                //
                if (utel.Length > 0)
                {
                    IQueryable<Driver> Drivers = db1.Drivers;
                    if (UserID!=null)
                        Drivers = Drivers.Where(p => p.AppUserID == UserID);
                    else
                        Drivers = Drivers.Where(p => p.DriverTel == utel);
                    if (Drivers.Count()==1)
                    {
                        var item = Drivers.First();
                        DriverID = item.DriverID;
                        ViewBag.DriverName = item.DriverName;
                    }
                    ViewBag.Drivers = Drivers;
                }
                //
                if (setid != null && setid > 0 && NewStatusID != null && NewStatusID > 0)
                {
                    Order o1 = db1.Orders.Find(setid);    // SELECT 1
                    if (o1 != null)
                    {
                        // Принял заявку
                        o1.OrderStatusID = int.Parse(NewStatusID.ToString());
                        db1.Entry(o1).State = EntityState.Modified; // UPDATE
                        db1.SaveChanges();  // UPDATE
                    }
                }
                //
                if (okid!=null && okid>0)
                {
                    Order o1 = db1.Orders.Find(okid);    // SELECT 1
                    if (o1 != null)
                    {
                        // Принял заявку
                        o1.OrderStatusID = 3;
                        db1.Entry(o1).State = EntityState.Modified; // UPDATE
                        db1.SaveChanges();  // UPDATE
                        //
                        if (true)
                        {
                            // PS? Уведомим клиента 
                        }
                        //
                    }
                }
                //
                if (cid != null && cid > 0)
                {
                    Order o1 = db1.Orders.Find(cid);    // SELECT 1
                    if (o1 != null)
                    {
                        // Отказался от заявки
                        o1.OrderStatusID = 1;
                        o1.DriverID = null;
                        db1.Entry(o1).State = EntityState.Modified; // UPDATE
                        db1.SaveChanges();  // UPDATE
                        //
                        if (true)
                        {
                            // PS? Уведомим диспетчера 
                        }
                        //
                    }
                }
                //
                if (DriverID > 0)
                {
                    IQueryable<Order> OrdersF = db1.Orders.Include(s => s.OrderStatus).Include(d => d.Driver).AsNoTracking();
                    IQueryable<Order> Orders = OrdersF;
                    OrdersF = OrdersF.Where(p => p.DriverID == DriverID).AsNoTracking();
                    Orders = OrdersF.Where(p => p.OrderStatusID < 5).AsNoTracking();
                    ViewBag.Orders = Orders;
                    ViewBag.OrdersF = OrdersF;
                }
            }
            ViewBag.DriverID = DriverID;
            //
            var OrderStatus = dbconn2.OrderStatuses;
            ViewBag.st = OrderStatus;
            return View();
        }
        public ActionResult IamHistory(int? id)
        {
            int DriverID = 0;
            if (id != null && id != 0)
            {
                DriverID = 0 + int.Parse(id.ToString());
            }
            if (DriverID > 0)
            {
                IQueryable<Order> Orders = db1.Orders;
                Orders = Orders.Where(p => p.DriverID == DriverID);
                ViewBag.Orders = Orders;
                //
                var OrderStatus = db1.OrderStatuses;
                ViewBag.s = OrderStatus;
            }
            return View();
        }


        [HttpGet]
        [Authorize]
        public ActionResult CreateDriver()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDriver(Driver Driver)
        {
            string ip = Request.ServerVariables.Get("REMOTE_ADDR").ToString().Trim();
            if (Driver.DriverName != null && Driver.DriverName != "")
            {
                IQueryable<Driver> Drivers = db1.Drivers.Where(p => p.DriverName == Driver.DriverName).AsNoTracking();
                if (Drivers.Count()>0)
                {
                    ModelState.AddModelError("DriverName", "Этот позывной уже занят");
                }

            }
            if (ModelState.IsValid)
            {
                //
                // PS? Проверить на переполнения базы
                IQueryable<Driver> DriverIP = db1.Drivers.Where(p => p.UserIP.Contains(ip)).AsNoTracking();
                if (DriverIP.Count() > 100)
                {
                    return HttpNotFound();
                }
                //
                int? UserID = null;
                string username = User.Identity.GetUserName().ToString();
                AppUser user = db1.AppUsers.Include(p => p.AppUserRole).FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    UserID = user.ID;
                }
                //
                Driver.DriverTel= Driver.DriverTel.Replace("+7", "8").Replace(" ", "");
                Driver.UserIP = ip;
                Driver.AppUserID = UserID;
                db1.Drivers.Add(Driver);          // INSERT
                db1.SaveChanges();
                return RedirectToAction("DriverList");
            }
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "Диспетчер")]
        public ActionResult EditDriver(int id)
        {
            Driver d = db1.Drivers.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            ViewBag.d = d;
            SelectList DriverStatus = new SelectList(db1.DriverStatuses,"ID","StatusName",d.DriverStatusID);
            ViewBag.s = DriverStatus;
            return View(d);
        }
        [HttpPost]
        [Authorize(Roles = "Диспетчер")]
        public ActionResult EditDriver(Driver Driver)
        {
            Driver.DriverTel = Driver.DriverTel.Replace("+7", "8").Replace(" ", "");
            db1.Entry(Driver).State = EntityState.Modified; // UPDATE
            db1.SaveChanges();  // UPDATE
            return Redirect("/Home/DriverList/");
        }


        [Authorize(Roles = "Диспетчер")]
        [HttpGet]
        public ActionResult ConfirmDriver(int id)
        {
            Driver d = db1.Drivers.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            ViewBag.Message = "" + d.DriverName + " / " + d.AutoModel + " " + d.AutoNomer;
            ViewBag.id = d.DriverID;
            return View();
        }
        [Authorize(Roles = "Диспетчер")]
        [HttpPost]
        public ActionResult DeleteDriver(int id)
        {
            // Можно пометить на удаление строку без поиска объекта - не существенное ускорение работы
            Driver d = db1.Drivers.Find(id);    // SELECT 1
            if (d == null) return HttpNotFound();
            db1.Drivers.Remove(d);
            db1.SaveChanges();
            return RedirectToAction("DriverList");
        }


        [HttpGet]
        public ActionResult NewOrder()
        {
            // PS? Добавить еще варианты и выбор времени
            string t1 = DateTime.Now.AddMinutes(10).ToString();
            string[] tim = new string[] { t1 };
            SelectList times = new SelectList(tim);
            ViewBag.t = times;
            //
            string[] dop = new string[] { "иномарка", "с кондиционером", "не курящая", "детское кресло", "багаж", "животное" };
            ViewBag.dop = dop;
            //
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewOrder(Order Order)
        {
            string ip = Request.ServerVariables.Get("REMOTE_ADDR").ToString().Trim();
            // Проверим на переполнение от 1 IP
            if (true)
            {
                IQueryable<Order> OrdersIP = db1.Orders.Where(p => p.UserIP.Contains(ip) && p.OrderStatusID==1).AsNoTracking();
                // p.UserIP.Contains(ip) && 
                if (OrdersIP.Count() > 100)
                {
                    return HttpNotFound();
                }
            }
            //
            if (1 == 1)
            {
                Order.DateID = DateTime.Now;
                Order.UserTime = DateTime.Now.AddMinutes(10);
                Order.UserName = CheckSQLString(Order.UserName);
                Order.UserTel = CheckSQLString(Order.UserTel).Replace(" ", "").Replace("+7", "8");
                // Приведем телефон к одному формату для отборов потом
                Order.Dop = CheckSQLString(Request.Form["Dop"].ToString());
                Order.UserIP = ip;
                db1.Orders.Add(Order);          // INSERT
                db1.SaveChanges();
                //
                if (false)
                {
                    // Пока отключим это и уберем пароль
                    // При отправке письма сервак задумался секунд на 5-10, выведем ожидание 
                    MailMessage Message = new MailMessage();
                    Message.Subject = "Новая заявка такси от " + Order.UserName + " " + Order.UserTel;
                    Message.IsBodyHtml = true;
                    Message.Body = "Новая заявка такси от " + Order.UserName + " " + Order.UserTel + "<br/><br/>_______________________________________________________________<br/>НЕ ОТВЕЧАЙТЕ НА ЭТО ПИСЬМО, ОНО СФОРМИРОВАНО СИСТЕМОЙ ОПОВЕЩЕНИЯ";
                    Message.BodyEncoding = Encoding.UTF8;
                    Message.From = new System.Net.Mail.MailAddress("" + WebConfigurationManager.AppSettings["MailSysAddr"].ToString());
                    Message.To.Add(new MailAddress("KaprovOleg@mail.ru"));
                    System.Net.Mail.SmtpClient Smtp = new SmtpClient("" + WebConfigurationManager.AppSettings["MailServer"].ToString(), 25);
                    Smtp.Host = "" + WebConfigurationManager.AppSettings["MailServer"].ToString(); 
                    Smtp.EnableSsl = true; // включение SSL для gmail / yandex нужно
                    Smtp.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["MailSysLogin"].ToString(), WebConfigurationManager.AppSettings["MailSysPwd"].ToString());
                    try
                    {
                        Smtp.Send(Message);//отправка
                    }
                    catch
                    {
                        // Сбой/ошибка
                        Response.Write("!");
                    }
                    //return "Спасибо за заказ такси в нашей компании!";
                }
                //
                return RedirectToAction("OrderList");
            }
            //
            return View();
        }



        public ActionResult DbInit()
        {
            //
            string ip = Request.ServerVariables.Get("REMOTE_ADDR").ToString().Trim();
            db1.AppUsers.Add(new AppUser { Email = "kaprovoleg@mail.ru", UserName = "Олег", Phone = "89272120150", Password = "12121981", PasswordHash = AccountController.GetMD5("12121981"), LastLogin = DateTime.Now, LastIP = ip, AppUserRoleID = 4 });
            db1.SaveChanges();  // UPDATE
            //
            db1.Drivers.Add(new Driver { DriverName = "Oleg", DriverTel = "89272120150", AutoModel = "Chevroler Cruze", AutoNomer = "521", AutoYear = 2017, DriverStatusID = 2, mapStayAdr = "Тольятти, Мира 51", mapX53 = 53.506942, mapY49 = 49.409119, UserIP= "0.0.0.127", AppUserID=1 });
            db1.Drivers.Add(new Driver { DriverName = "Mikhail", DriverTel = "89272123456", AutoModel = "Lada Kalina", AutoNomer = "732", AutoYear = 2016, DriverStatusID = 2, mapStayAdr = "Тольятти, Мира 108", mapX53 = 53.511419, mapY49 = 49.432835, UserIP = "0.0.0.127" });
            db1.Drivers.Add(new Driver { DriverName = "Fedor", DriverTel = "89272123456", AutoModel = "Lada Vesta", AutoNomer = "172", AutoYear = 2016, DriverStatusID = 2, mapStayAdr = "Тольятти, Баныкика 16", mapX53 = 53.501490, mapY49 = 49.422325, UserIP = "0.0.0.127" });
            db1.Drivers.Add(new Driver { DriverName = "Olga", DriverTel = "89272123456", AutoModel = "Lada XRay", AutoNomer = "500", AutoYear = 2016, DriverStatusID = 2, mapStayAdr = "Тольятти, Ленина 89", mapX53 = 53.518368, mapY49 = 49.415632, UserIP = "0.0.0.127" });
            db1.Drivers.Add(new Driver { DriverName = "Vitaliy", DriverTel = "89272123456", AutoModel = "Opel Astra", AutoNomer = "380", AutoYear = 2016, DriverStatusID = 1, mapStayAdr = "Тольятти, Карбышева 1", mapX53 = 53.514176, mapY49 = 49.441180, UserIP = "0.0.0.127" });
            db1.Drivers.Add(new Driver { DriverName = "Boss", DriverTel = "89272123456", AutoModel = "BMW X5", AutoNomer = "022", AutoYear = 2016, DriverStatusID = 3, mapStayAdr = "Тольятти, Жилина 44", mapX53 = 53.503279, mapY49 = 49.414051, UserIP = "0.0.0.127" });
            db1.SaveChanges();  // UPDATE
            //
            db1.Orders.Add(new Order { Address1 = "Тольятти, ул Мира 51", Address2 = "Тольятти, ул Мурысева 52", DateID = DateTime.Now, UserTime = DateTime.Now.AddMinutes(10), OrderStatusID = 3, DriverID = 1, Price = 200, UserName = "Олег", UserTel = "89272120150", UserIP = "0.0.0.127" });
            db1.Orders.Add(new Order { Address1 = "Тольятти, ул Мурысева 52", Address2 = "Тольятти, ул Мира 51", DateID = DateTime.Now, UserTime = DateTime.Now.AddMinutes(60), OrderStatusID = 1, UserName = "Олег", UserTel = "89272120150", UserIP = "0.0.0.127" });
            db1.SaveChanges();  // UPDATE
            //
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db1.Dispose();
            //
            base.Dispose(disposing);
        }
    }
}
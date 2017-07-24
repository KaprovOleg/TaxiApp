using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TaxiApp.Models
{
    public class AppContext : DbContext
    {
        public AppContext() : base("DefaultConnection") { }
        //
        public DbSet<DriverStatus> DriverStatuses { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Order> Orders { get; set; }
        //
        public DbSet<AppLoginAudit> AppLoginAudits { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
    }

    public class DbInitialization : DropCreateDatabaseAlways<AppContext>
    {
        protected override void Seed(AppContext db)
        {
            db.OrderStatuses.Add(new OrderStatus { StatusName = "Новый" }); // 1
            db.OrderStatuses.Add(new OrderStatus { StatusName = "Назначен" });
            db.OrderStatuses.Add(new OrderStatus { StatusName = "Принят" });
            db.OrderStatuses.Add(new OrderStatus { StatusName = "Выполняется" });
            db.OrderStatuses.Add(new OrderStatus { StatusName = "Выполнен" });
            db.OrderStatuses.Add(new OrderStatus { StatusName = "Отменен" });
            //
            db.DriverStatuses.Add(new DriverStatus { StatusName = "Не работаю" }); // 1
            db.DriverStatuses.Add(new DriverStatus { StatusName = "Свободен" });
            db.DriverStatuses.Add(new DriverStatus { StatusName = "Выполняю заявку" });
            //
            db.AppUserRoles.Add(new AppUserRole { Name = "Пользователь" });
            db.AppUserRoles.Add(new AppUserRole { Name = "Водитель" });
            db.AppUserRoles.Add(new AppUserRole { Name = "Диспетчер" });
            db.AppUserRoles.Add(new AppUserRole { Name = "Админ" });
            //
            base.Seed(db);
        }
    }

}
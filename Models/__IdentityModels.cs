using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TaxiApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    // Добавим инициализацию ролей
    //public class AppDbInitialization : DropCreateDatabaseAlways<TaxiContext>
    //{
    //    protected override void Seed(ApplicationDbContext context)
    //    {
    //        //
    //        var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
    //        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
    //        //
    //        var role1 = new IdentityRole { Name = "User" };
    //        var role2 = new IdentityRole { Name = "Driver" };
    //        var role3 = new IdentityRole { Name = "Admin" };
    //        roleManager.Create(role1);
    //        roleManager.Create(role2);
    //        roleManager.Create(role3);
    //        //
    //        var admin = new ApplicationUser { Email = "KaprovOleg@yandex.ru", UserName = "KaprovOleg@yandex.ru" };
    //        string password = "Taxi-1212";
    //        var result = userManager.Create(admin,password);
    //        if (result.Succeeded)
    //        {
    //            UserManager<ApplicationUser>.AddToRole(admin.Id, role1.Name);
    //        }
    //        //
    //        base.Seed(context);
    //    }
    //}

    //
}
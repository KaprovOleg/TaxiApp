using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiApp.Models
{
    public class AppUserRole
    {
        public int ID { get; set; }
        public string Name { get; set; }
        // Навигационное поля для по связи таблиц
        public ICollection<AppUser> AppUsers { get; set; }
        public AppUserRole()
        {
            AppUsers = new List<AppUser>();
        }
    }
}
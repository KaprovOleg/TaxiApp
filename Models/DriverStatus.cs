using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiApp.Models
{
    public class DriverStatus
    {
        public int ID { get; set; }
        public string StatusName { get; set; }
        // Навигационное поля для по связи таблиц
        public ICollection<Driver> Drivers { get; set; }
        public DriverStatus()
        {
            Drivers = new List<Driver>();
        }
    }
}
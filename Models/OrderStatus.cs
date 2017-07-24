using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiApp.Models
{
    public class OrderStatus
    {
        public int ID { get; set; }
        public string StatusName { get; set; }
        // Навигационное поля для по связи таблиц
        public ICollection<Order> Orders { get; set; }
        public OrderStatus()
        {
            Orders = new List<Order>();
        }
    }
}
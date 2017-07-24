using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc; // Remote

namespace TaxiApp.Models
{
    public class Driver
    {
        public int DriverID { get; set; }
        [Display(Name = "Позывной")]
        [Required]
        [StringLength(20,MinimumLength = 2, ErrorMessage = "Не допустимая длинна поля")]
        public string DriverName { get; set; }
        [Display(Name = "Телефон")]
        [Required]
        public string DriverTel { get; set; }
        [Display(Name = "Марка и модель")]
        [Required]
        public string AutoModel { get; set; }
        [Display(Name = "Гос номер")]
        [Required]
        public string AutoNomer { get; set; }
        [Display(Name = "Год выпуска")]
        [Required]
        [Range(2007,2017, ErrorMessage ="Не корректный год выпуска")]
        public int AutoYear { get; set; }
        [Display(Name = "Адрес остановки")]
        public string mapStayAdr { get; set; }
        [Display(Name = "GPS 53.xxxx")]
        [DataType(DataType.Text)]
        public double? mapX53 { get; set; }
        [Display(Name = "GPS 49.xxxx")]
        [DataType(DataType.Text)]
        public double? mapY49 { get; set; }
        [Display(Name = "Заявка?")]
        public int? OrderID { get; set; }

        [Display(Name = "IP")]
        public string UserIP { get; set; }
        [Display(Name = "User")]
        public int? AppUserID { get; set; }
        public AppUser AppUser { get; set; }

        [Display(Name = "Статус")]
        [Required]
        public int DriverStatusID { get; set; }
        public DriverStatus DriverStatus { get; set; }
        // Навигационное поля для по связи таблиц
        //public ICollection<Order> Orders { get; set; }
        //public Driver()
        //{
        //    Orders = new List<Order>();
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TaxiApp.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        [Display(Name = "Дата заявка")]
        public DateTime DateID { get; set; }
        [Display(Name = "IP")]
        public string UserIP { get; set; }
        [Display(Name = "Имя заказчика")]
        [Required]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Не допустимая длинна поля")]
        public string UserName { get; set; }
        [Display(Name = "Телефон")]
        [Required]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Не допустимая длинна")]
        public string UserTel { get; set; }
        [Display(Name = "Время подачи")]
        [DataType(DataType.DateTime)]
        public DateTime UserTime { get; set; }
        [Display(Name = "Откуда")]
        [Required]
        public string Address1 { get; set; }
        [Display(Name = "Куда")]
        [Required]
        public string Address2 { get; set; }
        [Display(Name = "Дополнительно")]
        public string Dop { get; set; }
        [Display(Name = "Цена")]
        public int? Price { get; set; }
        [Display(Name = "Водитель")]
        public int? DriverID { get; set; }
        public Driver Driver { get; set; }
        [Display(Name = "Статус")]
        public int OrderStatusID { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
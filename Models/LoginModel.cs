using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc; // Remote

namespace TaxiApp.Models
{
    public class LoginModel
    {
        [Display(Name = "EMail")]
        [Required]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Не допустимая длинна поля")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
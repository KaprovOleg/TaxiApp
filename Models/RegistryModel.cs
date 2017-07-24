using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TaxiApp.Models
{
    public class RegistryModel
    {
        [Display(Name = "Имя")]
        [Required]
        [StringLength(32,MinimumLength = 2, ErrorMessage = "Не допустимая длинна поля")]
        public string UserName { get; set; }
        [Display(Name = "EMail")]
        [Required]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Не допустимая длинна поля")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Не допустимая длинна поля")]
        public string Password { get; set; }
        [Display(Name = "(повторите)")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Не допустимая длинна поля")]
        [Compare("Password", ErrorMessage = "подтверждение не совпадают.")]
        public string PasswordConfirm { get; set; }
    }
}
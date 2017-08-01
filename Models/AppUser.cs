using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc; // Remote

namespace TaxiApp.Models
{
    public class AppUser
    {
        [key]
        [DatabaseGeneratedAttribute(DatabaseGenerateOPtion.Identity)]
        public int ID { get; set; }
        
        [Display(Name = "Имя")]
        [Required]
        [StringLength(32,MinimumLength = 2, ErrorMessage = "Не допустимая длинна поля")]
        public string UserName { get; set; }

        [Display(Name = "EMail")]
        [Required]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Не допустимая длинна поля")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "EMail")]
        [EmailAddress]
        public string EmailConfirm { get; set; }

        [Display(Name = "Телефон")]
        [Phone]
        public string Phone { get; set; }

        [Display(Name = "Телефон")]
        [Phone]
        public string PhoneConfirm { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Display(Name = "Аватар")]
        public string Avatar { get; set; }

        [Display(Name = "Дата входа")]
        [DataType(DataType.DateTime)]
        public DateTime LastLogin { get; set; }

        [Display(Name = "IP входа")]
        public string LastIP { get; set; }

        [Display(Name = "Ошибок")]
        public int FailCount { get; set; }

        [Display(Name = "Разрешения")]
        public string Allow { get; set; }

        [Display(Name = "Роль")]
        public int AppUserRoleID { get; set; }
        public AppUserRole AppUserRole { get; set; }
    }
}

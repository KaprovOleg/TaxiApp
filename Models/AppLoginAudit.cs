using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiApp.Models
{
    public class AppLoginAudit
    { 
        public int ID { get; set; }
        public DateTime DateID { get; set; }
        public string IP { get; set; }
        public string UserLogin { get; set; }
        public int Result { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public class JSONServerUser : JSONUser
    {
        public string login { get; set; }
        public string password { get; set; }
        private new DateTime? registerDate { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public class JSONUser
    {
        public JSONUser(User u, string token = "")
        {
            this.token = token;
            this.firstName = u.FirstName;
            this.lastName = u.LastName;
            this.image = u.Image;
            if(u.Role != null) this.roleName = u.Role.Name;
            this.registerDate = u.RegisterDate;
        }

        public string token { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string image { get; set; }
        public string roleName { get; set; }
        public Nullable<System.DateTime> registerDate { get; set; }

        
        //Пользовательское неявное преобразование из User в JSONUser
        public static explicit operator JSONUser(User u) => new JSONUser(u);
    }
}

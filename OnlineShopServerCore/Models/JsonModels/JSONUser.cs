using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public class JSONUser
    {
        public JSONUser()
        {

        }
        public JSONUser(User u)
        {
            this.id = u.Id;
            this.firstName = u.FirstName;
            this.lastName = u.LastName;
            this.image = u.Image;
            if(u.Role != null) this.role = u.Role;
            this.registerDate = u.RegisterDate;
        }

        public long id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string image { get; set; }
        public JsonRole role { get; set; }
        public DateTime? registerDate { get; set; }

        
        //Пользовательское неявное преобразование из User в JSONUser
        public static explicit operator JSONUser(User u) => new JSONUser(u);
    }
}

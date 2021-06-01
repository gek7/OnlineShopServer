using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public class JSONUserAuth : JSONUser
    {
        public JSONUserAuth(User u, string token = "") : base(u)
        {
            this.token = token;
        }

        public string token { get; set; }

        //Пользовательское неявное преобразование из User в JSONUserAuth
        public static implicit operator JSONUserAuth(User u) => new JSONUserAuth(u);
    }
}

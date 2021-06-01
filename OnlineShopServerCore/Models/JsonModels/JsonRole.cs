using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public class JsonRole
    {
        public JsonRole()
        {

        }
        public JsonRole(Role r)
        {
            this.id = r.Id;
            this.name = r.Name;
        }

        public long id { get; set; }
        public string name { get; set; }

        //Пользовательское неявное преобразование из Role в JSONRole
        public static implicit operator JsonRole(Role r) => new JsonRole(r);
    }
}

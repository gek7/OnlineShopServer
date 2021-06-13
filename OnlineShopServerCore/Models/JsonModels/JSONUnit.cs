using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONUnit
    {
        public JSONUnit()
        {
        }
        public JSONUnit(Unit u)
        {
            id = u.Id;
            name = u.Name;
            fullName = u.FullName;
        }

        public long id { get; set; }
        public string name { get; set; }
        public string fullName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONCategoryServer : JSONCategory
    {
        private new List<JSONCategory> categories { get; set; }
    }
}

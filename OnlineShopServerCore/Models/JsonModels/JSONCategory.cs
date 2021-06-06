using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONCategory
    {
        public JSONCategory()
        {
        }

        public JSONCategory(Category c,  bool isCheckChildren = false)
        {
            id = c.Id;
            image = c.Image;
            name = c.Name;
            if (c.OwnerNavigation != null && c.Id != c.Owner && !isCheckChildren)
            {
                owner = new JSONCategory(c.OwnerNavigation);
            }
            if (c.InverseOwnerNavigation != null && isCheckChildren)
            {
                categories = c.InverseOwnerNavigation.Select(cat => new JSONCategory(cat, isCheckChildren)).ToList();
            }

        }

        public long id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public JSONCategory owner { get; set; }
        public List<JSONCategory> categories { get; set; }

        public static explicit operator JSONCategory(Category c) => new JSONCategory(c);
    }
}

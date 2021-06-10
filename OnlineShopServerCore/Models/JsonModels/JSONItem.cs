using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONItem
    {
        public JSONItem()
        {
        }

        public JSONItem(Item c, bool isCheckChildren = false)
        {
            id = c.Id;
            images = c.ItemImages.Select(img => (JSONImage)img).ToList();
            description = c.Description;
            price = c.Price;
            if(c.Category != null) category = (JSONCategory) c.Category;
            count = c.Count;
            name = c.Name;
            if (c.OwnerNavigation != null && c.Id != c.Owner && !isCheckChildren)
            {
                owner = new JSONItem(c.OwnerNavigation);
            }
            if (c.InverseOwnerNavigation != null && isCheckChildren)
            {
                items = c.InverseOwnerNavigation.Select(cat => new JSONItem(cat, isCheckChildren)).ToList();
            }
        }

        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal? price { get; set; }
        public JSONCategory category { get; set; }
        public int? count { get; set; }
        public JSONItem owner { get; set; }
        public List<JSONImage> images { get; set; }
        public List<JSONItem> items { get; set; }
        public static explicit operator JSONItem(Item c) => new JSONItem(c);
    }
}

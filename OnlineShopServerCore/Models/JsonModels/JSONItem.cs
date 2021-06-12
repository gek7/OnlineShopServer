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

        public JSONItem(Item item, bool isCheckChildren = false)
        {
            id = item.Id;
            images = item.ItemImages.Select(img => (JSONImage)img).ToList();
            description = item.Description;
            price = item.Price;
            if(item.Category != null) category = (JSONCategory)item.Category;
            count = item.Count;
            name = item.Name;
            if (item.OwnerNavigation != null && item.Id != item.Owner && !isCheckChildren)
            {
                owner = new JSONItem(item.OwnerNavigation);
            }
            if (item.InverseOwnerNavigation != null && isCheckChildren)
            {
                items = item.InverseOwnerNavigation.Select(cat => new JSONItem(cat, isCheckChildren)).ToList();
            }
            if(item.Reviews.Count > 0) avgRating = item.Reviews.Average(r => r.ItemMark);
        }

        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal? price { get; set; }
        public double avgRating { get; set; }
        public JSONCategory category { get; set; }
        public int? count { get; set; }
        public JSONItem owner { get; set; }
        public List<JSONImage> images { get; set; }
        public List<JSONItem> items { get; set; }
        public static explicit operator JSONItem(Item c) => new JSONItem(c);
    }
}

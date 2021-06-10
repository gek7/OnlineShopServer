using OnlineShopServerCore.Models.JsonModels;
using System.Collections.Generic;

namespace OnlineShopServerCore.Models
{
    public class JSONItemServer : JSONItem
    {
        private new List<JSONItem> items { get; set; }

        public new List<JSONImage> images { get; set; }
    }
}
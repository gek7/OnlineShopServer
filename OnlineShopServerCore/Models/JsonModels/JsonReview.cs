using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JsonReview
    {
        public JsonReview()
        {}
        public JsonReview(Review r)
        {
            this.id = r.Id;
            this.reviewContent = r.ReviewContent;
            this.item = r.ItemId;
            this.user = new JSONUser(r.User);
            this.ItemMark = r.ItemMark;

            
        }

        public long id { get; set; }
        public string reviewContent { get; set; }
        public long item { get; set; }
        public JSONUser user { get; set; }
        public int ItemMark { get; set; }
    }
}

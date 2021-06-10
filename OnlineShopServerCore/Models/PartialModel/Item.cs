using OnlineShopServerCore.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public partial class Item
    {
        public void PatchFromRequest(JSONItemServer item)
        {
            Name = item.name;
            if (item.owner != null && item.owner.id > 0)
            {
                Owner = item.owner.id;
            }
            ItemImages.ToList().ForEach(img =>
            {
                var imgs = item.images.Where(i => i.id == img.Id);
                if (imgs.Count() > 0)
                {
                    JSONImage jsonImg = imgs.FirstOrDefault();
                    img.IsMain = jsonImg.isMain;
                }
                else
                {
                    this.ItemImages.Remove(img);
                }
            });
            Description = item.description;
            Price = item.price;
            if (item.category != null && item.category.id > 0) CategoryId = item.category.id;
            Count = item.count;
        }
    }
}

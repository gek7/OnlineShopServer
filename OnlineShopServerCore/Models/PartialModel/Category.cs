using OnlineShopServerCore.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public partial class Category
    {
        public void PatchFromRequest(JSONCategoryServer category)
        {
            this.Name = category.name;
            this.Image = category.image;
            if(category.owner != null)
            {
                this.Owner = category.owner.id;
            }
            else
            {
                this.Owner = null;
            }
        }
    }
}

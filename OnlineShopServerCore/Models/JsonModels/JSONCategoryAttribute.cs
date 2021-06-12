using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONCategoryAttribute
    {
        public JSONCategoryAttribute()
        {

        }
        public JSONCategoryAttribute(CategoryAttribute attr)
        {
            id = attr.Id;
            name = attr.Name;
            attrType = new JSONCategoryAttributeType(attr.Type);
            category = attr.CategoryId;
        }
        public long id { get; set; }
        public string name { get; set; }
        public JSONCategoryAttributeType attrType { get; set; }
        public long? category { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONCategoryAttributeType
    {
        public JSONCategoryAttributeType()
        {

        }
        public JSONCategoryAttributeType(CategoryAttributesType t)
        {
            id = t.Id;
            name = t.Name;
        }
        public long id { get; set; }
        public string name { get; set; }
    }
}

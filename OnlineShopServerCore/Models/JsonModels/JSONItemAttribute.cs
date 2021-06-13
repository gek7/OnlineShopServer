using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONItemAttribute
    {
        public JSONItemAttribute()
        {
        }
        public JSONItemAttribute(ItemAttribute attr)
        {
            id = attr.Id;
            itemId = attr.ItemId;
            categoryAttribute = new JSONCategoryAttribute(attr.CategoryAttributes);
            if(attr.ItemAttributesValues != null)
            {
                ItemAttributesValues = attr.ItemAttributesValues.Select(v => new JSONItemAttributeValue(v)).ToList();
            }
        }

        public long id { get; set; }
        public long? itemId { get; set; }
        public  JSONCategoryAttribute categoryAttribute { get; set; }
        public  List<JSONItemAttributeValue> ItemAttributesValues { get; set; }
    }
}

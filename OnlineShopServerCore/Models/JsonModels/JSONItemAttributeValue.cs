using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONItemAttributeValue
    {

        public JSONItemAttributeValue()
        {
        }
        public JSONItemAttributeValue(ItemAttributesValue value)
        {
            id = value.Id;
            itemAttributeId = value.ItemAttributeId;
            Value = value.Value;
            if (value.Unit != null)
            {
                Unit = new JSONUnit(value.Unit);
            }
        }
        public long id { get; set; }
        public long? itemAttributeId { get; set; }
        public string Value { get; set; }
        public JSONUnit Unit { get; set; }
    }
}

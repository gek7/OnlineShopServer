using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class CategoryAttribute
    {
        public CategoryAttribute()
        {
            EnumCategoryAttributesValues = new HashSet<EnumCategoryAttributesValue>();
            Filters = new HashSet<Filter>();
            ItemAttributes = new HashSet<ItemAttribute>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? TypeId { get; set; }
        public long? CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual CategoryAttributesType Type { get; set; }
        public virtual ICollection<EnumCategoryAttributesValue> EnumCategoryAttributesValues { get; set; }
        public virtual ICollection<Filter> Filters { get; set; }
        public virtual ICollection<ItemAttribute> ItemAttributes { get; set; }
    }
}

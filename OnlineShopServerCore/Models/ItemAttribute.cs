using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class ItemAttribute
    {
        public ItemAttribute()
        {
            ItemAttributesValues = new HashSet<ItemAttributesValue>();
        }

        public long Id { get; set; }
        public long? ItemId { get; set; }
        public long? CategoryAttributesId { get; set; }

        public virtual CategoryAttribute CategoryAttributes { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<ItemAttributesValue> ItemAttributesValues { get; set; }
    }
}

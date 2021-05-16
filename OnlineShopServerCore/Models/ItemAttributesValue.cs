using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class ItemAttributesValue
    {
        public long Id { get; set; }
        public long? ItemAttributeId { get; set; }
        public string Value { get; set; }
        public long? UnitId { get; set; }

        public virtual ItemAttribute ItemAttribute { get; set; }
        public virtual Unit Unit { get; set; }
    }
}

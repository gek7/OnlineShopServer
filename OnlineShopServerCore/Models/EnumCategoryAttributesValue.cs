using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class EnumCategoryAttributesValue
    {
        public long Id { get; set; }
        public long? CategoryAttributesId { get; set; }
        public string Value { get; set; }

        public virtual CategoryAttribute CategoryAttributes { get; set; }
    }
}

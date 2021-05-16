using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class Filter
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool? IsEnabled { get; set; }
        public long? CategoryAttributeId { get; set; }

        public virtual CategoryAttribute CategoryAttribute { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class ItemImage
    {
        public long Id { get; set; }
        public long? ItemId { get; set; }
        public string Path { get; set; }
        public bool? IsMain { get; set; }

        public virtual Item Item { get; set; }
    }
}

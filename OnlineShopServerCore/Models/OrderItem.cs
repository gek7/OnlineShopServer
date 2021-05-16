using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class OrderItem
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public long? ItemId { get; set; }
        public decimal? Price { get; set; }

        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
    }
}

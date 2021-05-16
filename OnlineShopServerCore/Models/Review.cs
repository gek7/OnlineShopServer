using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class Review
    {
        public long Id { get; set; }
        public string ReviewContent { get; set; }
        public long? UserId { get; set; }
        public long? ItemId { get; set; }

        public virtual Item Item { get; set; }
        public virtual User User { get; set; }
    }
}

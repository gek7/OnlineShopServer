using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class Unit
    {
        public Unit()
        {
            ItemAttributesValues = new HashSet<ItemAttributesValue>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public virtual ICollection<ItemAttributesValue> ItemAttributesValues { get; set; }
    }
}

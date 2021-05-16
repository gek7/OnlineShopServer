using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class CategoryAttributesType
    {
        public CategoryAttributesType()
        {
            CategoryAttributes = new HashSet<CategoryAttribute>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; }
    }
}

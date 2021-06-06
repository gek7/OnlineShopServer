using OnlineShopServerCore.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryAttributes = new HashSet<CategoryAttribute>();
            InverseOwnerNavigation = new HashSet<Category>();
            Items = new HashSet<Item>();
        }

        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public long? Owner { get; set; }

        public virtual Category OwnerNavigation { get; set; }
        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; }
        public virtual ICollection<Category> InverseOwnerNavigation { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}

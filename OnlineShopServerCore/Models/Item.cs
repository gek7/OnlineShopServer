using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class Item
    {
        public Item()
        {
            Carts = new HashSet<Cart>();
            InverseOwnerNavigation = new HashSet<Item>();
            ItemAttributes = new HashSet<ItemAttribute>();
            ItemImages = new HashSet<ItemImage>();
            OrderItems = new HashSet<OrderItem>();
            Reviews = new HashSet<Review>();
            WishLists = new HashSet<WishList>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public long? CategoryId { get; set; }
        public int? Count { get; set; }
        public long? Owner { get; set; }

        public virtual Category Category { get; set; }
        public virtual Item OwnerNavigation { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Item> InverseOwnerNavigation { get; set; }
        public virtual ICollection<ItemAttribute> ItemAttributes { get; set; }
        public virtual ICollection<ItemImage> ItemImages { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<WishList> WishLists { get; set; }
    }
}

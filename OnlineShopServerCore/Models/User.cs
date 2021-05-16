﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
            WishLists = new HashSet<WishList>();
        }

        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public long? RoleId { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        [JsonIgnore]
        public virtual Role Role { get; set; }
        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
        [JsonIgnore]
        public virtual ICollection<Review> Reviews { get; set; }
        [JsonIgnore]
        public virtual ICollection<WishList> WishLists { get; set; }
    }
}

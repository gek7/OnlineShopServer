using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShopServerCore.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public long Id { get; set; }
        public string OrderNum { get; set; }
        public string PhoneNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public long? OrderStatusId { get; set; }
        public long? UserId { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}

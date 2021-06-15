using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models.JsonModels
{
    public class JSONOrder
    {
        public JSONOrder()
        {
        }

        public JSONOrder(Order o)
        {
            id = o.Id;
            orderNum = o.OrderNum;
            phoneNumber = o.PhoneNumber;
            deliveryAddress = o.DeliveryAddress;
            orderStatus = new JSONStatus(o.OrderStatus);
            if (o.User != null)
                user = new JSONUser(o.User);
            if(o.OrderItems != null)
            {
                items = new List<JSONItem>();
                foreach (var item in o.OrderItems)
                {
                    items.Add(new JSONItem(item.Item, false, item.Price));
                }
            }
        }
        public long id { get; set; }
        public string orderNum { get; set; }
        public string phoneNumber { get; set; }
        public string deliveryAddress { get; set; }
        public JSONStatus orderStatus { get; set; }
        public JSONUser user { get; set; }
        public List<JSONItem> items { get; set; }
    }
}

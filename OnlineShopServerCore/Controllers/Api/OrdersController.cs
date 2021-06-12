using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopServerCore.Models;
using OnlineShopServerCore.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Controllers.Api
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : Controller
    {
        private OnlineShopContext _context;

        public OrdersController(OnlineShopContext context)
        {
            _context = context;
        }

        [HttpPost("createOrder")]
        [AllowAnonymous]
        public ActionResult<JSONOrder> CreateOrder(JSONOrder order)
        {
            Order o = new Order() {PhoneNumber = order.phoneNumber, DeliveryAddress = order.deliveryAddress };
            
            string num = "";
            Random r = new Random();
            genNewNum:
            for (int i = 0; i < 5; i++)
            {
                num += r.Next(9).ToString();
            }
            o.OrderNum = num;
            if(_context.Orders.Where(o=>o.OrderNum == num).Count()>0) goto genNewNum;
            o.OrderStatusId = 1;
            if(User.Claims.Count() > 0 )
            {
               o.UserId = AccountController.GetId(User.Claims);
            }
            _context.Orders.Add(o);
            _context.SaveChanges();
            order.items.ForEach(i =>
            {
                OrderItem oi = new OrderItem();
                oi.ItemId = i.id;
                oi.OrderId = o.Id;
                oi.Price = i.price;
                _context.OrderItems.Add(oi);
            });
            _context.SaveChanges();
            _context.OrderStatuses.Load();
            _context.Items.Load();
            _context.Categories.Load();
            _context.Reviews.Load();
            return Ok(new JSONOrder(o));
        }

        [HttpGet("getOrderByNum")]
        [AllowAnonymous]
        public ActionResult<JSONOrder> getOrderByNum(string num)
        {
            var Orders = _context.Orders
                .Include(i => i.OrderStatus)
                .Include(i => i.OrderItems)
                .Include(i => i.User)
                .Where(i=>i.OrderNum == num);
            Orders.Load();
            _context.ItemImages.Load();
            _context.Items.Load();
            _context.Categories.Load();
            _context.OrderItems.Load();
            _context.Reviews.Load();
            JSONOrder jsonOrder = null;
            if (Orders.Count() > 0) jsonOrder = Orders.Select(i=>new JSONOrder(i)).First();
            return Ok(jsonOrder);
        }
    }
}   

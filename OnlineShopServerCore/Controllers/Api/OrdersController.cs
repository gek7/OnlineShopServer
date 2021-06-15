using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
            Order o = new Order() { PhoneNumber = order.phoneNumber, DeliveryAddress = order.deliveryAddress };

            string num = "";
            Random r = new Random();
            genNewNum:
            for (int i = 0; i < 5; i++)
            {
                num += r.Next(9).ToString();
            }
            o.OrderNum = num;
            if (_context.Orders.Where(o => o.OrderNum == num).Count() > 0) goto genNewNum;
            o.OrderStatusId = 1;
            if (User.Claims.Count() > 0)
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
                .Where(i => i.OrderNum == num);
            Orders.Load();
            _context.ItemImages.Load();
            _context.Items.Load();
            _context.Categories.Load();
            _context.OrderItems.Load();
            _context.Reviews.Load();
            JSONOrder jsonOrder = null;
            if (Orders.Count() > 0) jsonOrder = Orders.Select(i => new JSONOrder(i)).First();
            return Ok(jsonOrder);
        }
        [HttpGet("getOrders")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<List<JSONOrder>> getOrders()
        {
            List<Order> Orders;
            if (User.IsInRole("Administrator"))
            {
                Orders = _context.Orders
                .Include(i => i.OrderStatus)
                .Include(i => i.OrderItems)
                .Include(i => i.User).ToList();
            }
            else
            {
                long id = AccountController.GetId(User.Claims);
                Orders = _context.Orders
               .Include(i => i.OrderStatus)
               .Include(i => i.OrderItems)
               .Include(i => i.User)
               .Where(u => u.UserId == id).ToList();
            }
            _context.ItemImages.Load();
            _context.Items.Load();
            _context.Categories.Load();
            _context.OrderItems.Load();
            _context.Reviews.Load();
            List<JSONOrder> jsonOrders = null;
            if (Orders.Count() > 0) jsonOrders = Orders.Select(i => new JSONOrder(i)).ToList();
            return Ok(jsonOrders);
        }
        [HttpGet("getMyOrders")]
        [Authorize]
        public ActionResult<List<JSONOrder>> getMyOrders()
        {
            List<Order> Orders;

            long id = AccountController.GetId(User.Claims);
            Orders = _context.Orders
           .Include(i => i.OrderStatus)
           .Include(i => i.OrderItems)
           .Include(i => i.User)
           .Where(u => u.UserId == id).ToList();

            _context.ItemImages.Load();
            _context.Items.Load();
            _context.Categories.Load();
            _context.OrderItems.Load();
            _context.Reviews.Load();
            List<JSONOrder> jsonOrders = new List<JSONOrder>();
            if (Orders.Count() > 0) jsonOrders = Orders.Select(i => new JSONOrder(i)).ToList();
            return Ok(jsonOrders);
        }
        [HttpGet("getStatuses")]
        [Authorize]
        public ActionResult<List<JSONStatus>> GetStatuses()
        {
            List<OrderStatus> Statuses = _context.OrderStatuses.ToList();
            List<JSONStatus> jsonStatuses = new List<JSONStatus>();
            if (Statuses.Count() > 0)
                jsonStatuses = Statuses.Select(s => new JSONStatus(s)).ToList();
            return Ok(jsonStatuses);
        }

        [HttpPost("EditStatus")]
        [Authorize(Roles = "Administrator")]
        public ActionResult GetStatuses(long orderId, long newStatusId)
        {
            if (_context.Find<OrderStatus>(newStatusId) != null)
            {
                var order = _context.Find<Order>(orderId);
                if (order != null)
                {
                    order.OrderStatusId = newStatusId;
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest("Заказ не найден");
                }
            }
            else
            {
                return BadRequest("Статус не найден");
            }

        }

        [HttpGet("getOrder")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<List<JSONOrder>> getOrder(long orderId)
        {
            List<Order> Orders;
            Orders = _context.Orders
           .Include(i => i.OrderStatus)
           .Include(i => i.OrderItems)
           .Include(i => i.User)
           .Where(u => u.Id == orderId).ToList();

            _context.ItemImages.Load();
            _context.Items.Load();
            _context.Categories.Load();
            _context.OrderItems.Load();
            _context.Reviews.Load();
            JSONOrder jsonOrder = null;
            if (Orders.Count() > 0) jsonOrder = Orders.Select(i => new JSONOrder(i)).First();
            return Ok(jsonOrder);
        }

    }
}

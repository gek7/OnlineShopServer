using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopServerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private OnlineShopContext _context;

        public UsersController(OnlineShopContext context)
        {
            _context = context;
        }


        [HttpGet("getAllUsers")]
        public ActionResult<List<JSONUser>> GetUsers()
        {
            //Возможно преобразовать модель базы данных в json модель, благодаря определению неявного преобразования в JSONUser
            var jsonUsers = _context.Users.Cast<JSONUser>().ToList();
            return jsonUsers;
        }
    }
}

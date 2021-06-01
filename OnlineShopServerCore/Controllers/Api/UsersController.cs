using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopServerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnlineShopServerCore.HelperUtils;

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
            List<JSONUser> jsonUsers = _context.Users.Include(u => u.Role).Select(u => (JSONUser)u).ToList();
            return Ok(jsonUsers);
        }

        [HttpGet]
        public ActionResult<JSONUser> GetUser(long id)
        {
            //Возможно преобразовать модель базы данных в json модель, благодаря определению неявного преобразования в JSONUser
            var jsonUser = _context.Users.Include(u => u.Role).Where(u => u.Id == id).ToList();
            if (jsonUser.Count() > 0)
            {
                return new JSONUser(jsonUser.First());
            }
            else
            {
                return BadRequest("Не найден пользователь");
            }
        }

        //Добавление
        [HttpPut]
        public ActionResult<JSONUser> AddUser(JSONServerUser user)
        {
            User u = new User();
            u.FirstName = user.firstName;
            u.LastName = user.lastName;
            u.Login = user.login;
            u.Password = user.password;
            Role r = _context.Roles.Find(user.role.id);
            if (r == null)
            {
                return BadRequest("Должна быть задана роль");
            }
            u.RoleId = user.role.id;

            if (user.image != null && ExistImage(user.image))
            {
                u.Image = user.image;
            }
            _context.Users.Add(u);
            _context.SaveChanges();
            return Ok(new JSONUser(u));
        }

        //Изменение
        [HttpPost]
        public ActionResult<JSONUser> PatchUser(JSONServerUser user)
        {
            User DatabaseUser = _context.Users.Find(user.id);
            if (DatabaseUser != null)
            {
                DatabaseUser.PatchFromRequest(user);
                _context.SaveChanges();
                return Ok(new JSONUser(DatabaseUser));
            }
            return BadRequest();
        }

        //Удаление 
        [HttpDelete]
        public void DeleteUser(long id)
        {
            User u = _context.Users.Find(id);
            if (u != null)
            {
                _context.Users.Remove(u);
                _context.SaveChanges();
            }
        }

        //Не выносим getRoles в отдельный контроллер, так как будем только получать все роли
        [HttpGet("roles")]
        public ActionResult<List<JsonRole>> getRoles() => _context.Roles.Cast<JsonRole>().ToList();

    }
}

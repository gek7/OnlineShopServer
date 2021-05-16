using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineShopServerCore.Models;

namespace OnlineShopServerCore.Controllers.Api
{
    [Route("api/account")]
    [Authorize]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly OnlineShopContext _context;

        public AccountController(OnlineShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = _context.Users.ToList();
            var u = User;

            if (users.Count == 0)
            {
                return BadRequest();
            }

            return users;
        }

        [AllowAnonymous]
        [Route("auth")]
        [HttpPost]
        public async Task<ActionResult> Auth(string username, string password)
        {
            Task<ActionResult> response = new Task<ActionResult>(() =>
             {
                  var handler = new JwtSecurityTokenHandler();

                //Получения закрытого ключа 
                var tokenKey = AuthOptions.GetSymmetricSecurityKey();

                //Описание параметров для создания токена
                var descriptor = new SecurityTokenDescriptor()
                  {
                      Subject = GetIdentity(username, password),
                      Expires = DateTime.UtcNow.Add(AuthOptions.LIFETIME),
                      SigningCredentials = new SigningCredentials(
                          tokenKey,
                          SecurityAlgorithms.HmacSha256Signature)
                  };
                //Создание токена
                var token = handler.CreateToken(descriptor);

                //Сериализуем токен в строку и возвращаем клиенту
                return Ok(handler.WriteToken(token));
              });
            return await response;
        }

        [Route("info")]
        [HttpPost]
        public ActionResult<User> Info()
        {
            User curUser = _context.Users.Find(User.Identity);
            return curUser;
        }

        //Получение id и роли по логину и паролю
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User person = _context.Users.FirstOrDefault(user => user.Login == username
                                                              && user.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("Id", person.Id.ToString()),
                };
                if (person.Role != null)
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role?.Name));
                return new ClaimsIdentity(claims);
            }

            // если пользователя не найдено
            return null;
        }

    }
}

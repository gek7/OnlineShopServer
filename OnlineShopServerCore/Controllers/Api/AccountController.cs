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
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace OnlineShopServerCore.Controllers.Api
{
    [Route("api/account")]
    [Authorize]
    [ApiController]
    public class AccountController : Controller
    {
        //toDo регистрация
        private  OnlineShopContext _context;

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

        //Авторизация
        [AllowAnonymous]
        [Route("auth")]
        [HttpPost]
        public async Task<ActionResult> Auth(string username, string password)
        {

            Task<ActionResult> response = Task<ActionResult>.Factory.StartNew(() =>
             {
                 var handler = new JwtSecurityTokenHandler();
                 var AuthUser = GetUser(username, password);
                 //Получения закрытого ключа 
                 var tokenKey = AuthOptions.GetSymmetricSecurityKey();
                 var claims = GetIdentity(AuthUser);
                 if (claims != null)
                 {
                     //Описание параметров для создания токена
                     var descriptor = new SecurityTokenDescriptor()
                     {
                         Subject = claims,
                         Expires = DateTime.UtcNow.Add(AuthOptions.LIFETIME),
                         SigningCredentials = new SigningCredentials(
                                tokenKey,
                                SecurityAlgorithms.HmacSha256Signature)
                     };
                     //Создание токена
                     var token = handler.CreateToken(descriptor);

                     //Сериализуем токен в строку и возвращаем клиенту
                     return Ok(new JSONUserAuth(AuthUser, handler.WriteToken(token)));
                 }
                 return Unauthorized("Неверный логин или пароль");
             });

            return await response;
        }

        //Информация об аккаунте
        [Route("info")]
        [HttpGet]
        public async Task<ActionResult<JSONUserAuth>> Info()
        {
            User curUser = await _context.Users.Include(u=>u.Role).Where(u=>u.Id== GetId(User.Claims)).FirstAsync();
            return new JSONUserAuth(curUser);
        }

        //Получение картинки
        [HttpGet("ProfileImage/{img}")]
        [AllowAnonymous]
        public ActionResult GetProfileImage(string img)
        {
            //User curUser = await _context.Users.FindAsync(GetId(User.Claims));
            /*if (String.IsNullOrEmpty(curUser.Image))
                return BadRequest();*/
            // Путь к файлу
            string file_path = Path.Combine(Startup.EnvDirectory + "\\UsersImages\\"+ img);
            // Тип файла - content-type
            string file_type = "image/jpeg";
            // Имя файла - необязательно
            string file_name = img;
            return PhysicalFile(file_path, file_type, file_name);
        }


        [Route("setImage")]
        [HttpPost]
        public async Task<IActionResult> setImage([FromForm(Name ="file")] IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                User curUser = await _context.Users.FindAsync(GetId(User.Claims));
                HelperUtils.SaveImageForUser(curUser, uploadedFile, _context);
                return Ok();
            }
            return BadRequest();
        }

        private OnlineShopServerCore.Models.User GetUser(string username, string password) => 
            _context.Users.Include(u=>u.Role)
            .FirstOrDefault(user => user.Login == username && user.Password == password);

        //Получение id и роли по логину и паролю
        private ClaimsIdentity GetIdentity(User person)
        {
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

        public static long GetId(IEnumerable<Claim> claims)
        {
            var val = claims.FirstOrDefault((c) => c.Type == "Id").Value;
            if (val == null) throw new Exception("Пользователь не найден в базе данных");
            return long.Parse(val);
        }
    }
}

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
        private OnlineShopContext _context;

        public AccountController(OnlineShopContext context)
        {
            _context = context;
        }

        //Авторизация
        [AllowAnonymous]
        [Route("auth")]
        [HttpPost]
        public async Task<ActionResult> Authorization(string username, string password)
        {

            Task<ActionResult> response = Task<ActionResult>.Factory.StartNew(() =>
             {
                 var AuthUser = GetUser(username, password);
                 if (AuthUser != null)
                 {
                     string token = GetTokenByUser(AuthUser);
                     return Ok(new JSONUserAuth(AuthUser, token));

                 }
                 return Unauthorized("Неверный логин или пароль");
             });

            return await response;
        }

        [NonAction]
        public string GetTokenByUser(User AuthUser)
        {
            var handler = new JwtSecurityTokenHandler();
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
                return handler.WriteToken(token);
            }
            return null;
        }

        //Информация об аккаунте
        [Route("info")]
        [HttpGet]
        public async Task<ActionResult<JSONUserAuth>> Info()
        {
            User curUser = await _context.Users.Include(u => u.Role).Where(u => u.Id == GetId(User.Claims)).FirstAsync();
            return new JSONUserAuth(curUser);
        }

        //Получение картинки
        [HttpGet("ProfileImage/{img}")]
        [AllowAnonymous]
        public ActionResult GetProfileImage(string img)
        {
            // Путь к файлу
            string file_path = Path.Combine(Startup.UserImagesPath + img);
            if (!HelperUtils.ExistImage(file_path))
            {
                file_path = Startup.UserImagesPath + "DefaultUserPhoto.jpg";
            }
            // Тип файла - content-type
            string file_type = "image/jpeg";
            // Имя файла - необязательно
            string file_name = img;
            return PhysicalFile(file_path, file_type, file_name);
        }


        [Route("setImage")]
        [HttpPost]
        public async Task<IActionResult> setImage([FromForm(Name = "file")] IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                User curUser = await _context.Users.FindAsync(GetId(User.Claims));
                string idName = curUser.Id + Path.GetExtension(uploadedFile.FileName);
                string path = Startup.UserImagesPath + idName;

                //Проверка есть ли папка для изображений пользователя
                if (!Directory.Exists(Startup.UserImagesPath))
                {
                    Directory.CreateDirectory(Startup.UserImagesPath);
                }
                //Проверка есть ли уже такое изображение
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                curUser.Image = idName;
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        //Регистрация
        [HttpPost("SignUp")]
        [AllowAnonymous]
        public ActionResult<JSONUser> SignUp(JSONServerUser user)
        {
            User u = new User();
            u.FirstName = user.firstName;
            u.LastName = user.lastName;
            u.Login = user.login;
            u.Password = user.password;
            u.RoleId = 2;
            if (string.IsNullOrEmpty(u.FirstName) || string.IsNullOrEmpty(u.LastName))
            {
                return BadRequest("Должны быть быть заполнены имя и фамилия");
            }
            if (string.IsNullOrEmpty(u.Login) || string.IsNullOrEmpty(u.Password))
            {
                return BadRequest("Должны быть быть заполнены логин и пароль");
            }
            if (_context.Users.Where(dbuser => dbuser.Login == u.Login).Count() > 0)
            {
                return BadRequest("Пользователь с таким логином уже существует");
            }

            _context.Users.Add(u);
            _context.SaveChanges();
            string token = GetTokenByUser(u);
            return Ok(new JSONUserAuth(u, token));
        }

        private OnlineShopServerCore.Models.User GetUser(string username, string password) =>
            _context.Users.Include(u => u.Role)
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

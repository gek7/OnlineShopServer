using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineShopServerCore.Models;
using OnlineShopServerCore.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Controllers.Api
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : Controller
    {
        private OnlineShopContext _context;

        public ItemsController(OnlineShopContext context)
        {
            _context = context;
        }

        [HttpGet("GetRelativeItems")]
        [AllowAnonymous]
        public ActionResult<List<JSONItem>> GetRelativeItems(long id)
        {
            var Items = _context.Items
                .Include(i => i.OwnerNavigation)
                .Include(i => i.InverseOwnerNavigation)
                .Include(i => i.ItemImages);
            Items.Load();
            Item CurrentItem = _context.Items.Find(id);
            _context.Reviews.Load();
            _context.Categories.Load();
            if (CurrentItem != null)
            {
                var jsonItems = Items.Where(i => i.OwnerNavigation == CurrentItem || i.InverseOwnerNavigation.Contains(CurrentItem)).Select(i => (JSONItem)i).ToList();
                return Ok(jsonItems);
            }
            else
            {
                return Ok(new List<JSONItem>());
            }
        }

        [HttpGet("getAllItems")]
        [AllowAnonymous]
        public ActionResult<List<JSONItem>> GetItems()
        {
            var Items = _context.Items
                .Include(i => i.OwnerNavigation)
                .Include(i => i.InverseOwnerNavigation)
                .Include(i => i.ItemImages);
            Items.Load();
            _context.Categories.Load();
            _context.Reviews.Load();
            var jsonItems = Items.Select(i => (JSONItem)i).ToList();
            return Ok(jsonItems);
        }

        [HttpPost("getItemsByIds")]
        [AllowAnonymous]
        public ActionResult<List<JSONItem>> GetItemsByIds(List<long> ids)
        {
            var Items = _context.Items
                .Include(i => i.OwnerNavigation)
                .Include(i => i.InverseOwnerNavigation)
                .Include(i => i.ItemImages);
            Items.Load();
            _context.Categories.Load();
            _context.Reviews.Load();
            var jsonItems = Items.Where(i => ids.Contains(i.Id)).Select(i => (JSONItem)i).ToList();
            return Ok(jsonItems);
        }

        [HttpGet("getAllItemsByCategory")]
        [AllowAnonymous]
        public ActionResult<List<JSONItem>> GetItemsByCategory(long id)
        {
            var Categories = _context.Categories
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation);
            Categories.Load();
            var Category = Categories.Where(c => c.Id == id).FirstOrDefault();
            if (Category != null)
            {
                List<Category> cats = new List<Category>();
                AddCategoryAllChilds(Category, cats);
                List<long?> ids = cats.Select(c => (long?)c.Id).ToList();
                ids.Add(id);
                var Items = _context.Items
                .Include(i => i.OwnerNavigation)
                .Include(i => i.InverseOwnerNavigation)
                .Include(i => i.ItemImages)
                .Where(i => ids.Contains(i.CategoryId));
                Items.Load();
                _context.Categories.Load();
                _context.Reviews.Load();
                var jsonItems = Items.Select(i => (JSONItem)i).ToList();
                return Ok(jsonItems);
            }
            return BadRequest("Категория не найдена");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<JSONItem> GetItem(long id)
        {
            //Возможно преобразовать модель базы данных в json модель, благодаря определению неявного преобразования в JSONItem
            var jsonItem = _context.Items
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation)
                .Include(i => i.ItemImages)
                .Where(u => u.Id == id).ToList();
            _context.Categories.Load();
            _context.Reviews.Load();
            if (jsonItem.Count() > 0)
            {
                return new JSONItem(jsonItem.First());
            }
            else
            {
                return BadRequest("Не найден товар");
            }
        }

        [HttpGet("getChilds")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<List<JSONItem>> GetItemChilds(long id)
        {
            //Возможно преобразовать модель базы данных в json модель, благодаря определению неявного преобразования в JSONItem
            var Items = _context.Items
               .Include(i => i.OwnerNavigation)
               .Include(i => i.InverseOwnerNavigation)
               .Include(i => i.ItemImages);
            Items.Load();
            _context.Categories.Load();
            _context.Reviews.Load();
            var jsonItems = Items.Where(i => i.Owner == id).Select(i => new JSONItem(i, true)).ToList();
            if (jsonItems.Count() > 0)
            {
                return Ok(jsonItems);
            }
            else
            {
                return BadRequest("Не найдена категория");
            }
        }

        //Добавление
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public ActionResult<JSONItem> AddItem(JSONItemServer Item)
        {
            Item c = new Item();
            c.PatchFromRequest(Item);
            _context.Items.Add(c);
            _context.SaveChanges();
            _context.Categories.Load();
            _context.Reviews.Load();
            return Ok(new JSONItem(c));
        }

        //Изменение
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<JSONItem> PatchItem(JSONItemServer Item)
        {
            Item DatabaseItem = _context.Items
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation).Where(c => c.Id == Item.id)
                .Include(i => i.ItemImages).FirstOrDefault();
            _context.Categories.Load();
            _context.Reviews.Load();
            if (DatabaseItem != null)
            {
                DatabaseItem.PatchFromRequest(Item);
                _context.SaveChanges();
                DatabaseItem = _context.Items
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation)
                .Include(i => i.ItemImages).Where(c => c.Id == Item.id).FirstOrDefault();
                _context.Categories.Load();
                _context.Reviews.Load();
                return Ok(new JSONItem(DatabaseItem));
            }
            return BadRequest("Не найден товар");
        }

        //Удаление 
        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteItem(long id)
        {
            Item c = _context.Items.Find(id);
            if (c != null)
            {
                try
                {
                    // assuming db is your DbContext
                    var items = _context.Items
                                      .Where(q => q.Id == id)
                                      .Include(q => q.ItemImages);

                    // assuming this is your DbSet
                    _context.Items.RemoveRange(items);

                    _context.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    SqlException sqlex = e.InnerException as SqlException;
                    if (sqlex != null)
                    {
                        if (sqlex.Errors.Count == 2 &&
                            sqlex.Errors[0].Message.Contains("FK_ItemImages_Items"))
                        {
                            _context.Items.Remove(c);
                            _context.SaveChanges();
                        }
                        else if (((SqlException)e.InnerException).ErrorCode == -2146232060)
                        {
                            return BadRequest("Запись используется в других местах");
                        }
                        else
                        {
                            return BadRequest(e.Message);
                        }
                    }
                }
            }
            return BadRequest("Не найдена категория");
        }

        //Получение картинки
        [HttpGet("ItemImage/{img}")]
        [AllowAnonymous]
        public ActionResult GetItemImage(string img)
        {
            // Путь к файлу
            string file_path = Path.Combine(Startup.ItemImagesPath + img);
            if (!HelperUtils.ExistImage(file_path))
            {
                //дефолтная картинка
                //file_path = Startup.ItemImagesPath + "ItemPhoto.jpg";
            }
            // Тип файла - content-type
            string file_type = "image/jpeg";
            // Имя файла - необязательно
            string file_name = img;
            return PhysicalFile(file_path, file_type, file_name);
        }

        [Route("setImage")]
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> setImage([FromQuery(Name = "id")] long id, [FromQuery(Name = "isMain")] bool isMain, [FromForm(Name = "file")] IFormFile uploadedFile)
        {
            if (id != 0)
            {
                Item curItem = await _context.Items.Include(i => i.ItemImages).Where(i => i.Id == id).FirstOrDefaultAsync();
                if (uploadedFile != null && curItem != null)
                {
                    Random r = new Random();
                    r.Next(1, 100);
                    string idName = curItem.Id + "-" + r.Next(1, 100) + Path.GetExtension(uploadedFile.FileName);
                    string path = Startup.ItemImagesPath + idName;

                    //Проверка есть ли папка для изображений пользователя
                    if (!Directory.Exists(Startup.EnvDirectory + "/ItemsImages/"))
                    {
                        Directory.CreateDirectory(Startup.EnvDirectory + "/ItemsImages/");
                    }

                    while (HelperUtils.ExistImage(path))
                    {
                        idName = curItem.Id + "-" + r.Next(1, 100) + Path.GetExtension(uploadedFile.FileName);
                        path = Startup.ItemImagesPath + idName;
                    }

                    using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    _context.ItemImages.Add(new ItemImage() { IsMain = isMain, ItemId = id, Path = idName });
                    _context.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet("reviews")]
        [AllowAnonymous]
        public ActionResult<List<JsonReview>> ReviewsById(long itemId)
        {
            _context.Users.Load();
            _context.Roles.Load();
            var Reviews = _context.Reviews
                .Include(c => c.User)
                .Where(r => r.ItemId == itemId).ToList();
            List<JsonReview> jsonReviews = new List<JsonReview>();
            if (Reviews.Count > 0)
            {
                jsonReviews.AddRange(Reviews.Select(r => new JsonReview(r)));
            }
            return Ok(jsonReviews);
        }

        [HttpDelete("reviews")]
        [AllowAnonymous]
        public ActionResult<List<JsonReview>> deleteReview(long ReviewId)
        {
            Review r = _context.Reviews.Find(ReviewId);
            if (r != null)
            {
                if (User.Claims.Count() > 0)
                {
                    var user = _context.Users.Find(AccountController.GetId(User.Claims));
                    if (user.RoleId == 1 || user.Id == r.UserId)
                    {
                        _context.Reviews.Remove(r);
                        _context.SaveChanges();
                        return Ok();
                    }
                }
                return Unauthorized();
            }
            else
            {
                return BadRequest("Отзыв не найден");
            }
        }
        [HttpPut("reviews")]
        [Authorize]
        public ActionResult addReview(JsonReview newReview)
        {
            if (newReview != null && newReview.item > 0 && _context.Items.Find(newReview.item) != null)
            {
                Review r = new Review();
                r.ItemId = newReview.item;
                r.ItemMark = newReview.ItemMark;
                if(newReview.ItemMark < 0 || newReview.ItemMark > 5)
                {
                    return BadRequest("Неправильное значение оценки");
                }
                if(newReview.ItemMark == 0 && string.IsNullOrWhiteSpace(newReview.reviewContent))
                {
                    return BadRequest("Должен быть проставлен рейтинг или написан отзыв");
                }
                r.ReviewContent = newReview.reviewContent;
                r.UserId = AccountController.GetId(User.Claims);
                _context.Reviews.Add(r);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("Товар не найден");
            }
        }

        [NonAction]
        public void AddCategoryAllChilds(Category category, List<Category> categories)
        {
            if (category.InverseOwnerNavigation.Count > 0)
            {
                category.InverseOwnerNavigation.ToList().ForEach(c =>
                {
                    categories.Add(c);
                    AddCategoryAllChilds(c, categories);
                });
            }
        }
    }
}

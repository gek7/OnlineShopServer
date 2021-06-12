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
    [Route("api/categories")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class CategoriesController : Controller
    {
        private OnlineShopContext _context;

        public CategoriesController(OnlineShopContext context)
        {
            _context = context;
        }

        [HttpGet("GetTreeViewCategories")]
        [AllowAnonymous]
        public ActionResult<List<JSONCategory>> GetTreeViewCategories()
        {
            var Categories = _context.Categories
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation);
            Categories.Load();
            var jsonCategories = Categories.Where(c=>c.Owner == null).Select(c => new JSONCategory(c, true)).ToList();
            return Ok(jsonCategories);
        }

        [HttpGet("getAllCategories")]
        [AllowAnonymous]
        public ActionResult<List<JSONCategory>> GetCategories()
        {
            List<JSONCategory> jsonCategories = _context.Categories
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation)
                .Select(c => (JSONCategory)c).ToList();
            return Ok(jsonCategories);
        }

        [HttpGet]
        public ActionResult<JSONCategory> GetCategory(long id)
        {
            //Возможно преобразовать модель базы данных в json модель, благодаря определению неявного преобразования в JSONCategory
            var jsonCategory = _context.Categories
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation)
                .Where(u => u.Id == id).ToList();
            if (jsonCategory.Count() > 0)
            {
                return new JSONCategory(jsonCategory.First());
            }
            else
            {
                return BadRequest("Не найдена категория");
            }
        }

        [HttpGet("getChilds")]
        public ActionResult<List<JSONCategory>> GetCategoryChilds(long id)
        {
            //Возможно преобразовать модель базы данных в json модель, благодаря определению неявного преобразования в JSONCategory
            var jsonCategory = _context.Categories
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation)
                .Where(u => u.Id == id).ToList();
            if (jsonCategory.Count() > 0)
            {
                return Ok(jsonCategory.First().InverseOwnerNavigation.Select(c => (JSONCategory)c));
            }
            else
            {
                return BadRequest("Не найдена категория");
            }
        }

        //Добавление
        [HttpPut]
        public ActionResult<JSONCategory> AddCategory(JSONCategoryServer Category)
        {
            Category c = new Category();
            c.Name = Category.name;
            if (_context.Categories.Find(Category.owner.id) != null)
            {
                c.Owner = Category.owner.id;
            }

            if (Category.image != null && HelperUtils.ExistImage(Startup.CategoryImagesPath + Category.image))
            {
                c.Image = Category.image;
            }
            _context.Categories.Add(c);
            _context.SaveChanges();
            return Ok(new JSONCategory(c));
        }

        //Изменение
        [HttpPost]
        public ActionResult<JSONCategory> PatchCategory(JSONCategoryServer Category)
        {
            Category DatabaseCategory = _context.Categories
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation).Where(c => c.Id == Category.id).FirstOrDefault();
            if (DatabaseCategory != null)
            {
                DatabaseCategory.PatchFromRequest(Category);
                _context.SaveChanges();
                DatabaseCategory = _context.Categories
                .Include(c => c.OwnerNavigation)
                .Include(c => c.InverseOwnerNavigation).Where(c => c.Id == Category.id).FirstOrDefault();
                return Ok(new JSONCategory(DatabaseCategory));
            }
            return BadRequest();
        }

        //Удаление 
        [HttpDelete]
        public ActionResult DeleteCategory(long id)
        {
            Category c = _context.Categories.Find(id);
            if (c != null)
            {
                try
                {
                    _context.Categories.Remove(c);
                    _context.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    if (((SqlException)e.InnerException).ErrorCode == -2146232060)
                    {
                        return BadRequest("Запись используется в других местах");
                    }
                    else
                    {
                        return BadRequest(e.Message);
                    }
                }
            }
            return BadRequest("Не найдена категория");
        }

        //Получение картинки
        [HttpGet("CategoryImage/{img}")]
        [AllowAnonymous]
        public ActionResult GetCategoryImage(string img)
        {
            // Путь к файлу
            string file_path = Path.Combine(Startup.CategoryImagesPath + img);
            if (!HelperUtils.ExistImage(file_path))
            {
                file_path = Startup.CategoryImagesPath + "DefaultCategoryPhoto.jpg";
            }
            // Тип файла - content-type
            string file_type = "image/jpeg";
            // Имя файла - необязательно
            string file_name = img;
            return PhysicalFile(file_path, file_type, file_name);
        }

        [Route("setImage")]
        [HttpPost]
        public async Task<IActionResult> SetCategoryImage([FromQuery(Name = "id")] long id, [FromForm(Name = "file")] IFormFile uploadedFile)
        {
            if (id != 0)
            {
                Category curCategory = await _context.Categories.FindAsync(id);
                if (uploadedFile == null)
                {
                    curCategory.Image = null;
                    _context.SaveChanges();
                    return Ok();
                }
                else if (curCategory != null)
                {
                    string idName = curCategory.Id + Path.GetExtension(uploadedFile.FileName);
                    string path = Startup.CategoryImagesPath + idName;
                    //Проверка есть ли папка для изображений пользователя
                    if (!Directory.Exists(Startup.EnvDirectory + "/CategoriesImages/"))
                        Directory.CreateDirectory(Startup.EnvDirectory + "/CategoriesImages/");
                    
                    //Проверка есть ли уже такое изображение
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    curCategory.Image = idName;
                    _context.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet("CategoryAttributes")]
        [AllowAnonymous]
        public ActionResult<List<JsonReview>> CategoryAttributesById(long categoryId)
        {
            var Attrs = _context.CategoryAttributes
                .Include(c => c.Type)
                .Where(r => r.CategoryId == categoryId).ToList();
            List<JSONCategoryAttribute> jsonAttrs = new List<JSONCategoryAttribute>();
            if (Attrs.Count > 0)
            {
                jsonAttrs = Attrs.Select(attr => new JSONCategoryAttribute(attr)).ToList();
            }
            return Ok(jsonAttrs);
        }

        [HttpGet("CategoryAttribute")]
        [AllowAnonymous]
        public ActionResult<JSONCategoryAttribute> GetCategoryAttribute(long attrId)
        {
            var Attr = _context.CategoryAttributes
                .Include(c => c.Type)
                .Where(r => r.Id == attrId).FirstOrDefault();
            if (Attr == null)
            {
                return BadRequest("Атрибут не найден");
            }
            return Ok(new JSONCategoryAttribute(Attr));
        }
        //Добавление
        [HttpPut("CategoryAttribute")]
        public ActionResult<JSONCategoryAttribute> AddCategoryAttributes(JSONCategoryAttribute attr)
        {
            CategoryAttribute c = new CategoryAttribute();
            c.Name = attr.name;
            if (_context.CategoryAttributesTypes.Find(attr.attrType.id) != null)
            {
                c.TypeId = attr.attrType.id;
            }
            else
            {
                return BadRequest("Тип атрибута не найден");
            }
            if (_context.Categories.Find(attr.category) != null)
            {
                c.CategoryId = attr.category;
            }
            else
            {
                return BadRequest("Категория не найдена");
            }

            _context.CategoryAttributes.Add(c);
            _context.SaveChanges();
            return Ok(new JSONCategoryAttribute(c));
        }

        //Изменение
        [HttpPost("CategoryAttribute")]
        public ActionResult<JSONCategoryAttribute> PatchCategoryAttributes(JSONCategoryAttribute Category)
        {
            CategoryAttribute DatabaseCategoryAttr = _context.CategoryAttributes
                .Include(c => c.Type)
                .Where(c => c.Id == Category.id).FirstOrDefault();
            if (DatabaseCategoryAttr != null)
            {
                DatabaseCategoryAttr.Name = Category.name;
                if(Category.attrType != null && _context.CategoryAttributesTypes.Find(Category.attrType.id) != null )
                {
                    DatabaseCategoryAttr.TypeId = Category.attrType.id;
                }
                else
                {
                    BadRequest("Не заполнен тип атрибута");
                }
                _context.SaveChanges();
                return Ok(new JSONCategoryAttribute(DatabaseCategoryAttr));
            }
            return BadRequest();
        }

        //Удаление 
        [HttpDelete("CategoryAttribute")]
        public ActionResult DeleteCategoryAttributes(long id)
        {
            CategoryAttribute c = _context.CategoryAttributes.Find(id);
            if (c != null)
            {
                try
                {
                    _context.CategoryAttributes.Remove(c);
                    _context.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    if (((SqlException)e.InnerException).ErrorCode == -2146232060)
                    {
                        return BadRequest("Запись используется в других местах");
                    }
                    else
                    {
                        return BadRequest(e.Message);
                    }
                }
            }
            return BadRequest("Не найден атрибут");
        }

        [HttpGet("CategoryAttributes/types")]
        [AllowAnonymous]
        public ActionResult<List<JSONCategoryAttributeType>> CategoryAttributesTypes()
        {
            var attrTypes = _context.CategoryAttributesTypes.ToList();
            var JsonAttrsTypes = new List<JSONCategoryAttributeType>();
            if (attrTypes.Count > 0)
                JsonAttrsTypes.AddRange(attrTypes.Select(t => new JSONCategoryAttributeType(t)));
            return Ok(JsonAttrsTypes);
        }
    }
}

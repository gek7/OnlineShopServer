using Microsoft.AspNetCore.Http;
using OnlineShopServerCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore
{
    public class HelperUtils
    {
        public static bool ExistImage(string path) => System.IO.File.Exists(path);

        public async static void SaveImageForUser(User curUser, IFormFile uploadedFile, OnlineShopContext _context)
        {
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

        }
    }
}

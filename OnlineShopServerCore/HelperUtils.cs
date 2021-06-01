using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore
{
    public class HelperUtils
    {
        public static bool ExistImage(string img) => System.IO.File.Exists(Startup.ImagesPath + img);
    }
}

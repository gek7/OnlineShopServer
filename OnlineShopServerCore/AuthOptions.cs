using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace OnlineShopServerCore
{
    public class AuthOptions
    {
        const string KEY = "36H5xF0C3Iaw5tGs";   // ключ для шифрации
        public static TimeSpan LIFETIME = TimeSpan.FromDays(30); // время жизни токена - 30 дней
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}

using Microsoft.AspNetCore.Http;
using OnlineShopServerCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnlineShopServerCore
{
    public class HelperUtils
    {
        public static bool ExistImage(string path) => System.IO.File.Exists(path);
        public static string GetMD5Hash(string str)
        {
            if (!HelperUtils.IsMD5(str))
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                return Convert.ToBase64String(hash);
            }
            return str;
        }
        public static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnlineShopServerCore.HelperUtils;

namespace OnlineShopServerCore.Models
{
    public partial class User
    {
        public void PatchFromRequest(JSONServerUser user)
        {
            if (String.IsNullOrWhiteSpace(user.login)) this.Login = user.login;
            if (String.IsNullOrWhiteSpace(user.password)) this.Password = user.password;
            this.FirstName = user.firstName;
            this.LastName = user.lastName;
            if (user.image != null && ExistImage(user.image))
            {
                this.Image = user.image;
            }
            this.RoleId = RoleId;
        }
    }
}

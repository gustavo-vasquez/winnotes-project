using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Layer
{
    public class UserLoginData
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AvatarImage { get; set; }
        public bool Active { get; set; }

        public UserLoginData(int userID, string userName, string email, string avatarImage, bool active)
        {            
            this.UserID = userID;
            this.UserName = userName;
            this.Email = email;
            this.AvatarImage = avatarImage;
            this.Active = active;
        }
    }
}

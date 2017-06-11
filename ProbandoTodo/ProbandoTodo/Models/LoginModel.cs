using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace ProbandoTodo.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    //public class UserData
    //{
    //    public int UserID { get; set; }
    //    public string UserName { get; set; }
    //    public string Email { get; set; }
    //    public bool EmailConfirmed { get; set; }
    //    public string AvatarImage { get; set; }

    //    public bool SessionIsActive(string[] data)
    //    {
    //        if (data != null)
    //        {
    //            this.UserID = Convert.ToInt32(data[0]);
    //            this.UserName = data[1];
    //            this.Email = data[2];
    //            this.EmailConfirmed = Convert.ToBoolean(data[3]);
    //            this.AvatarImage = data[4];

    //            return true;
    //        }                

    //        return false;
    //    }        
    //}
}
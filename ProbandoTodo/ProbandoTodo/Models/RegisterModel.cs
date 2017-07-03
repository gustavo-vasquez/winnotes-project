using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Linq;
using System.Web;
using Domain_Layer.Enums;

namespace ProbandoTodo.Models
{            
    public class RegisterModel
    {        
        [Required(ErrorMessage = "-Ingresar nombre de usuario")]
        [RegularExpression("^[a-zA-Z0-9_]*$")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ingresar nombre de email")]
        [RegularExpression("^[^@ ]+$")]
        public string Email { get; set; }

        public MailProviders MailProvider { get; set; }

        [Required(ErrorMessage = "-Ingresar contraseña")]
        [MinLength(6)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$")]
        public string Password { get; set; }

        [Required(ErrorMessage = " ")]
        [Compare("Password", ErrorMessage = "-Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}
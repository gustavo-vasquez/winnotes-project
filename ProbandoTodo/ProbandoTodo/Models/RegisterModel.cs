using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Linq;
using System.Web;
using static ProbandoTodo.Models.CustomDataAnnotations;

namespace ProbandoTodo.Models
{            
    public class RegisterModel
    {        
        [Required(ErrorMessage = "-Debe ingresar el nombre de usuario")]
        [RegularExpression("^[a-zA-Z0-9 _]*$")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Debe ingresar el usuario de email")]
        [RegularExpression("^[^@]+$")]
        public string Email { get; set; }

        public char MailProvider { get; set; }

        [Required(ErrorMessage = "-Debe ingresar algo")]
        [MinLength(6)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "-Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }                       
    }
}
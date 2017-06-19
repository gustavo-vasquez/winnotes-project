using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ProbandoTodo.Models.CustomDataAnnotations;
using System.Linq;
using System.Web;

namespace ProbandoTodo.Models
{
    public class ProfileManagementModels
    {
        public AvatarSectionViewModel AvatarSectionModel { get; set; }
        public PersonalPhraseViewModel PersonalPhraseModel { get; set; }
        public InformationSectionViewModel InformationSectionModel { get; set; }
        public ChangePasswordSectionViewModel ChangePasswordSectionModel { get; set; }

        public ProfileManagementModels(string[] userInformation)
        {
            this.AvatarSectionModel = new AvatarSectionViewModel()
            {
                AvatarSource = userInformation[0]                
            };
            this.PersonalPhraseModel = new PersonalPhraseViewModel()
            {
                PersonalPhrase = userInformation[1],
                PhraseColor = userInformation[2]
            };
            this.InformationSectionModel = new InformationSectionViewModel()
            {
                UserName = userInformation[3],
                Email = userInformation[4],
                RegistrationDate = userInformation[5]
            };
            this.ChangePasswordSectionModel = new ChangePasswordSectionViewModel();
        }

        public ProfileManagementModels(string[] userInformation, object sectionModel)
        {
            this.AvatarSectionModel = new AvatarSectionViewModel()
            {
                AvatarSource = userInformation[0]
            };
            this.PersonalPhraseModel = new PersonalPhraseViewModel()
            {
                PersonalPhrase = userInformation[1],
                PhraseColor = userInformation[2]
            };
            this.InformationSectionModel = new InformationSectionViewModel()
            {
                UserName = userInformation[3],
                Email = userInformation[4],
                RegistrationDate = userInformation[5]
            };
            this.ChangePasswordSectionModel = new ChangePasswordSectionViewModel();

            //if (sectionModel is AvatarSectionViewModel)
            //    this.AvatarSectionModel = (AvatarSectionViewModel)sectionModel;

            if(sectionModel is PersonalPhraseViewModel)
                this.PersonalPhraseModel = (PersonalPhraseViewModel)sectionModel;

            if (sectionModel is InformationSectionViewModel)
                this.InformationSectionModel = (InformationSectionViewModel)sectionModel;

            if (sectionModel is ChangePasswordSectionViewModel)
                this.ChangePasswordSectionModel = (ChangePasswordSectionViewModel)sectionModel;
        }

        public class AvatarSectionViewModel
        {
            public string AvatarSource { get; set; }

            [FileSize(2*1024*1024)]
            [FileTypes("jpg,jpeg,png")]
            public HttpPostedFileBase UploadAvatar { get; set; }
        }
        
        public class PersonalPhraseViewModel
        {
            [Required]
            [MaxLength(140)]
            public string PersonalPhrase { get; set; }

            public string PhraseColor { get; set; }
        }

        public class InformationSectionViewModel
        {
            //[Required(ErrorMessage = "-Debe ingresar el nombre de usuario")]
            //[RegularExpression("^[a-zA-Z0-9 _]*$")]
            public string UserName { get; set; }

            //[Required(ErrorMessage = "-Debe ingresar el usuario de email")]
            //[RegularExpression("^[^@]+$")]
            public string Email { get; set; }
            public string RegistrationDate { get; set; }
        }

        public class ChangePasswordSectionViewModel
        {            
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "-Este campo no puede estar vacío")]            
            public string CurrentPassword { get; set; }

            [DataType(DataType.Password)]
            [Required(ErrorMessage = "-Este campo no puede estar vacío")]
            [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$")]            
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Required(ErrorMessage = "-Este campo no puede estar vacío")]
            [Compare("NewPassword", ErrorMessage = "-Las contraseñas no coinciden")]
            public string RepeatPassword { get; set; }
        }
    }
}
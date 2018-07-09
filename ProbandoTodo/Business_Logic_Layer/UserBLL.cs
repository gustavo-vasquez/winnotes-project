using Data_Access_Layer;
using Domain_Layer;
using Domain_Layer.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Business_Logic_Layer
{
    public class UserBLL
    {
        static UserDAL userDAL = new UserDAL();

        public string GetEncryptedUserID(int userID)
        {
            return userDAL.GetEncryptedUserID(userID);
        }

        /// <summary>
        /// (Capa de negocio) Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="UserName">Nombre de usuario</param>
        /// <param name="Email">Correo electrónico</param>
        /// <param name="Password">Contraseña</param>
        /// <returns></returns>
        public string CreateUser(string UserName, string Email, MailProviders MailProvider, string Password)
        {
            string EmailComplete;

            switch (MailProvider)
            {
                case MailProviders.gmail:
                    EmailComplete =  String.Concat(Email, "@gmail.com");
                    break;
                case MailProviders.outlook:
                    EmailComplete = String.Concat(Email, "@outlook.com");
                    break;
                case MailProviders.yahoo:
                    EmailComplete = String.Concat(Email, "@yahoo.com");
                    break;
                default: throw new ArgumentNullException("El proveedor de correo es erróneo.");
            }
            
            return userDAL.CreateUserDAL(UserName, EmailComplete, Password);
        }

        /// <summary>
        /// (Capa de negocio) Verifica el nombre de usuario.
        /// </summary>
        /// <param name="name">Nombre de usuario</param>
        /// <returns></returns>
        public bool CheckUserName(string name)
        {
            return userDAL.CheckUserNameDAL(name);
        }

        /// <summary>
        /// (Capa de negocio) Recopila la información que se va a guardar como sesión de usuario.
        /// </summary>
        /// <param name="Email">Correo electrónico</param>
        /// <param name="Password">Contraseña</param>
        /// <returns></returns>
        public UserLoginData Login(string Email, string Password)
        {            
            return userDAL.LoginDAL(Email, Password);
        }

        /// <summary>
        /// (Capa de negocio) Devuelve el ID de usuario encriptado desde la base de datos.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns></returns>
        public string RetrieveEncryptedID(string email)
        {
            return userDAL.RetrieveEncryptedID(email);
        }

        /// <summary>
        /// (Capa de negocio) Devuelve la información necesaria para recuperar una sesión.
        /// </summary>
        /// <param name="encryptedID">ID encriptado del usuario</param>
        /// <returns></returns>
        public UserLoginData RememberSessionInfo(string encryptedID)
        {
            return userDAL.RememberSessionInfoDAL(encryptedID);
        }

        /// <summary>
        /// (Capa de negocio) Devuelve la información de perfil del usuario.
        /// </summary>
        /// <param name="userID">ID del usuario actual</param>
        /// <returns></returns>
        public string[] GetUserInformation(int userID)
        {
            return userDAL.GetUserInformation(userID);
        }

        /// <summary>
        /// (Capa de negocio) Cambia la imágen de perfil del usuario.
        /// </summary>
        /// <param name="newAvatar">Imágen de perfil</param>
        /// <param name="email">Correo electrónico</param>
        /// <param name="error">Posible error</param>
        /// <returns></returns>
        public void ChangeAvatar(HttpPostedFileBase newAvatar, int userID)
        {
            Image avatarImage = null;
            CheckAvatarConditions(newAvatar, ref avatarImage);
            userDAL.ChangeAvatar(avatarImage, newAvatar.ContentType, userID);
        }

        /// <summary>
        /// (Capa de negocio) Cambia la frase personal del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="phrase">Frase personal</param>
        /// <param name="phraseColor">Fuente de color de la frase</param>
        /// <returns></returns>
        public void ChangePersonalPhrase(int userID, string phrase, string phraseColor)
        {
            userDAL.ChangePersonalPhraseDAL(userID, phrase, phraseColor);
        }

        /// <summary>
        /// (Capa de negocio) Cambia la contraseña del usuario
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="currentPassword">Contraseña actual</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <param name="error">Posible error</param>
        /// <returns></returns>
        public void ChangePassword(int userID, string currentPassword, string newPassword)
        {
            userDAL.ChangePasswordDAL(userID, currentPassword, newPassword);
        }

        // METODOS PARA EL WIZARD
        public string AvatarInfoForWizardBLL(int userID)
        {
            return userDAL.AvatarInfoForWizardDAL(userID);
        }

        public string[] PhraseInfoForWizardDAL(int userID)
        {
            return userDAL.PhraseInfoForWizardDAL(userID);
        }

        public string TemporaryAvatarBLL(HttpPostedFile tempAvatar, HttpServerUtilityBase localServer, int userID)
        {
            //Image avatarImage = null;
            //MemoryStream ms = new MemoryStream();
            //tempAvatar.InputStream.CopyTo(ms);
            //avatarImage = Image.FromStream(ms);
            return userDAL.TemporaryAvatarDAL(tempAvatar, localServer, userID);
        }

        public void UpdateAvatar(string path, int userID)
        {            
            userDAL.UpdateAvatar(path, userID);            
        }

        #region VALIDACIÓN DEL AVATAR DEL USUARIO

        /// <summary>
        /// Verifica la validez de la imágen de usuario.
        /// </summary>
        /// <param name="newAvatar">Nueva imágen de usuario</param>
        /// <param name="error">Posible error</param>
        /// <param name="avatarImage">Variable de imágen que se convertirá</param>
        /// <returns></returns>
        private void CheckAvatarConditions(HttpPostedFileBase newAvatar, ref Image avatarImage)
        {
            try
            {
                if (newAvatar == null)
                    throw new ArgumentNullException("DEBE ELEGIR UNA IMAGEN");

                const int _maxSize = 2 * 1024 * 1024;
                const int _maxWidth = 1000;
                const int _maxHeight = 1000;
                List<string> _fileTypes = new List<string>() { "jpg", "jpeg", "gif", "png" };

                if (newAvatar.ContentLength > _maxSize)
                    throw new FormatException("EL AVATAR NO DEBE SUPERAR LOS 2MB");                

                string avatarExtension = System.IO.Path.GetExtension(newAvatar.FileName).Substring(1);

                if (!_fileTypes.Contains(avatarExtension, StringComparer.OrdinalIgnoreCase))                
                    throw new FormatException("PARA EL AVATAR SOLO SE ADMITEN IMÁGENES JPG, JPEG, GIF Y PNG");
                
                MemoryStream ms = new MemoryStream();
                newAvatar.InputStream.CopyTo(ms);
                avatarImage = Image.FromStream(ms);

                if (avatarImage.Width > _maxWidth || avatarImage.Height > _maxHeight)         
                    throw new FormatException("EL AVATAR ADMITE HASTA UNA RESOLUCIÓN DE 1000x1000");
            }
            catch
            {
                throw;
            }
            
        }

        #endregion               
    }
}

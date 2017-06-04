using Data_Access_Layer;
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
        /// <summary>
        /// (Capa de negocio) Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="UserName">Nombre de usuario</param>
        /// <param name="Email">Correo electrónico</param>
        /// <param name="Password">Contraseña</param>
        /// <returns></returns>
        public bool CreateUser(string UserName, string Email, char MailProvider, string Password)
        {
            string EmailComplete;

            switch (MailProvider)
            {
                case 'G':
                    EmailComplete = String.Concat(Email, "@gmail.com");
                    break;
                case 'O':
                    EmailComplete = String.Concat(Email, "@outlook.com");
                    break;                
                default: EmailComplete = String.Concat(Email, "@yahoo.com");
                    break;
            }
            
            return new UserDAL().CreateUserDAL(UserName, EmailComplete, Password);
        }

        /// <summary>
        /// (Capa de negocio) Verifica el nombre de usuario.
        /// </summary>
        /// <param name="name">Nombre de usuario</param>
        /// <returns></returns>
        public bool CheckUserName(string name)
        {
            return new UserDAL().CheckUserNameDAL(name);
        }

        /// <summary>
        /// (Capa de negocio) Recopila la información que se va a guardar como sesión de usuario.
        /// </summary>
        /// <param name="Email">Correo electrónico</param>
        /// <param name="Password">Contraseña</param>
        /// <returns></returns>
        public string[] Login(string Email, string Password)
        {            
            return new UserDAL().LoginDAL(Email, Password);
        }

        /// <summary>
        /// (Capa de negocio) Devuelve la información necesaria para recuperar una sesión.
        /// </summary>
        /// <param name="encryptedID">ID encriptado del usuario</param>
        /// <returns></returns>
        public string[] RememberSessionInfo(string encryptedID)
        {
            return new UserDAL().RememberSessionInfo(encryptedID);
        }

        /// <summary>
        /// (Capa de negocio) Devuelve la información de perfil del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns></returns>
        public string[] GetUserInformation(string email)
        {
            return new UserDAL().GetUserInformation(email);
        }

        /// <summary>
        /// (Capa de negocio) Cambia la imágen de perfil del usuario.
        /// </summary>
        /// <param name="newAvatar">Imágen de perfil</param>
        /// <param name="email">Correo electrónico</param>
        /// <param name="error">Posible error</param>
        /// <returns></returns>
        public bool ChangeAvatar(HttpPostedFileBase newAvatar, string email, ref string error)
        {
            Image avatarImage = null;

            if(CheckAvatarConditions(newAvatar, ref error, ref avatarImage))
            {
                return new UserDAL().ChangeAvatar(avatarImage, newAvatar.ContentType, email);
            }

            return false;
        }

        /// <summary>
        /// (Capa de negocio) Cambia la frase personal del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="phrase">Frase personal</param>
        /// <param name="phraseColor">Fuente de color de la frase</param>
        /// <returns></returns>
        public bool ChangePersonalPhrase(string email, string phrase, string phraseColor)
        {
            return new UserDAL().ChangePersonalPhraseDAL(email, phrase, phraseColor);
        }

        /// <summary>
        /// (Capa de negocio) Cambia la contraseña del usuario
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="currentPassword">Contraseña actual</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <param name="error">Posible error</param>
        /// <returns></returns>
        public bool ChangePassword(string email, string currentPassword, string newPassword, ref string error)
        {
            return new UserDAL().ChangePasswordDAL(email, currentPassword, newPassword, ref error);
        }


        #region VALIDACIÓN DEL AVATAR DEL USUARIO

        /// <summary>
        /// Verifica la validez de la imágen de usuario.
        /// </summary>
        /// <param name="newAvatar">Nueva imágen de usuario</param>
        /// <param name="error">Posible error</param>
        /// <param name="avatarImage">Variable de imágen que se convertirá</param>
        /// <returns></returns>
        private bool CheckAvatarConditions(HttpPostedFileBase newAvatar, ref string error, ref Image avatarImage)
        {
            try
            {
                int _maxSize = 2 * 1024 * 1024;
                List<string> _fileTypes = new List<string>() { "jpg", "jpeg", "gif", "png" };
                int _maxWidth = 1280;
                int _maxHeight = 720;

                if (newAvatar.ContentLength > _maxSize)
                {
                    error = "EL AVATAR DEBE TENER UN TAMAÑO MAXIMO DE 2MB";
                    return false;
                }

                string avatarExtension = System.IO.Path.GetExtension(newAvatar.FileName).Substring(1);

                if (!_fileTypes.Contains(avatarExtension, StringComparer.OrdinalIgnoreCase))
                {
                    error = "PARA EL AVATAR SOLO SE ADMITEN IMÁGENES JPG, JPEG, GIF Y PNG";
                    return false;
                }
                
                MemoryStream ms = new MemoryStream();
                newAvatar.InputStream.CopyTo(ms);
                avatarImage = Image.FromStream(ms);

                if (avatarImage.Width > _maxWidth || avatarImage.Height > _maxHeight)
                {
                    error = "EL AVATAR ADMITE HASTA UNA RESOLUCIÓN DE 1280x720";
                    return false;
                }

                return true;
            }
            catch
            {
                error = "HA OCURRIDO UN ERROR INESPERADO";
                return false;
            }
            
        }

        #endregion

        
        #region ENCRIPTAR INFORMACIÓN DE USUARIO

        /// <summary>
        /// Devuelve el ID de usuario encriptado.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string EncryptToSHA256(string email)
        {
            try
            {
                var dataToEncrypt = new UserDAL().RetrieveIdToEncrypt(email);

                HashAlgorithm hasher = null;

                try
                {
                    hasher = new SHA256Managed();
                }
                catch
                {
                    hasher = new SHA256CryptoServiceProvider();
                }

                byte[] plainBytes = Encoding.UTF8.GetBytes(dataToEncrypt);
                byte[] hashedBytes = hasher.ComputeHash(plainBytes);
                hasher.Clear();

                var IdentifierEncrypted = Convert.ToBase64String(hashedBytes);
                new UserDAL().SaveHashDAL(IdentifierEncrypted, email);

                return IdentifierEncrypted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

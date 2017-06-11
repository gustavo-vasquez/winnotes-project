using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data_Access_Layer
{
    public class UserDAL
    {
        public WinNotesDBEntities OpenConnection()
        {            
            WinNotesDBEntities WinNotesContext = new WinNotesDBEntities();
            return WinNotesContext;
        }

        public void CloseConnection(WinNotesDBEntities WinNotesContext)
        {
            WinNotesContext.Dispose();            
        }

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="UserName">Nombre de usuario</param>
        /// <param name="Email">Correo electrónico</param>
        /// <param name="Password">Contraseña</param>
        /// <returns></returns>
        public string CreateUserDAL(string UserName, string Email, string Password)
        {
            try
            { 
                var context = OpenConnection();
                var search = context.Person.Any(p => p.Email == Email);

                if(!search && !CheckUserNameDAL(UserName))
                {
                    Person newUser = new Person();
                    newUser.UserName = UserName;
                    newUser.Email = Email;
                    newUser.Password = Password;
                    newUser.AvatarImage = null;
                    newUser.AvatarMIMEType = null;
                    newUser.Active = false;                    
                    newUser.RegistrationDate = DateTime.Now;
                    newUser.LastLoginDate = DateTime.Now;

                    context.Person.Add(newUser);
                    context.SaveChanges();
                    newUser.PersonIDEncrypted = this.EncryptToSHA256(newUser.PersonID);
                    context.SaveChanges();
                    CloseConnection(context);

                    return Email;
                }

                CloseConnection(context);
                return null;
            }
            catch
            {
                throw;
            }
        }


        #region ENCRIPTAR INFORMACIÓN DE USUARIO

        /// <summary>
        /// Devuelve el ID de usuario encriptado.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string EncryptToSHA256(int userID)
        {
            try
            {
                //var dataToEncrypt = new UserDAL().RetrieveIdToEncrypt(email);

                HashAlgorithm hasher = null;

                try
                {
                    hasher = new SHA256Managed();
                }
                catch
                {
                    hasher = new SHA256CryptoServiceProvider();
                }

                byte[] plainBytes = Encoding.UTF8.GetBytes(userID.ToString());
                byte[] hashedBytes = hasher.ComputeHash(plainBytes);
                hasher.Clear();

                var IdentifierEncrypted = Convert.ToBase64String(hashedBytes);
                //new UserDAL().SaveHashDAL(IdentifierEncrypted, email);

                return IdentifierEncrypted;
            }
            catch
            {
                throw;
            }
        }

        #endregion


        /// <summary>
        /// Verifica la existencia del nombre de usuario elegido.
        /// </summary>
        /// <param name="UserName">Nombre de usuario</param>
        /// <returns></returns>
        public bool CheckUserNameDAL(string UserName)
        {
            var context = OpenConnection();
            bool result = context.Person.Any(p => p.UserName == UserName);

            return result;
        }


        /// <summary>
        /// Recopila la información que se va a guardar como sesión de usuario.
        /// </summary>
        /// <param name="Email">Correo electrónico</param>
        /// <param name="Password">Contraseña</param>
        /// <returns></returns>
        public UserLoginData LoginDAL(string Email, string Password)
        {            
            var context = OpenConnection();
            Person user = context.Person.Where(p => p.Email == Email).FirstOrDefault();
            UserLoginData login = null;

            if(user != null)
            {
                login = new UserLoginData(
                        user.PersonID,
                        user.UserName,
                        user.Email,
                        GetAvatarImage(user.AvatarImage, user.AvatarMIMEType),
                        Convert.ToBoolean(user.Active)
                    );
            }
            
            CloseConnection(context);

            return login;
        }        
        
        /// <summary>
        /// Devuelve el ID de usuario encriptado desde la base de datos.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns></returns>
        public string RetrieveEncryptedID(string email)
        {
            try
            {
                var context = OpenConnection();
                string EncryptedID = context.Person.Where(p => p.Email == email).First().PersonIDEncrypted;                
                CloseConnection(context);

                return EncryptedID;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Guarda el ID de usuario encriptado en la base de datos.
        /// </summary>
        /// <param name="IdentifierEncrypted"></param>
        /// <param name="email"></param>
        public void SaveHashDAL(string IdentifierEncrypted, string email)
        {
            try
            {
                var context = OpenConnection();
                Person person = context.Person.Where(p => p.Email == email).First();
                person.PersonIDEncrypted = IdentifierEncrypted;
                context.SaveChanges();
                CloseConnection(context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Devuelve la información necesaria para recuperar una sesión.
        /// </summary>
        /// <param name="encryptedID">ID encriptado del usuario</param>
        /// <returns></returns>
        public string[] RememberSessionInfo(string encryptedID)
        {
            string[] LoginData = null;
            var context = OpenConnection();
            Person User = context.Person.Where(p => p.PersonIDEncrypted == encryptedID).FirstOrDefault();

            if (User != null)
            {
                LoginData = new string[] {
                    User.PersonID.ToString(),
                    User.UserName,
                    User.Email,
                    User.Active.ToString().ToLower(),
                    GetAvatarImage(User.AvatarImage, User.AvatarMIMEType)
                };
            }

            return LoginData;
        }

        //public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        //{
        //    MemoryStream ms = new MemoryStream(byteArrayIn);
        //    System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
        //    return returnImage;
        //}

        /// <summary>
        /// Convierte una imágen a un array de bytes.
        /// </summary>
        /// <param name="AvatarFile">Imágen de perfil</param>
        /// <returns></returns>
        public byte[] ConvertImageToByteArray(Image AvatarFile)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                AvatarFile.Save(ms, AvatarFile.RawFormat);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }        

        /// <summary>
        /// Devuelve la información de perfil del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns></returns>
        public string[] GetUserInformation(string email)
        {
            var context = OpenConnection();
            Person PersonData = context.Person.Where(p => p.Email == email).FirstOrDefault();
            
            var UserInformation = new string[] { GetAvatarImage(PersonData.AvatarImage, PersonData.AvatarMIMEType), PersonData.PersonalPhrase, PersonData.UserName, PersonData.Email, PersonData.PhraseColor };
            CloseConnection(context);

            return UserInformation;
        }

        /// <summary>
        /// Devuelve la imágen de perfil expresada como base64. Por defecto: la imágen predeterminada de perfil.
        /// </summary>
        /// <param name="AvatarImage">Imágen de perfil como array de bytes</param>
        /// <param name="MIMEtype">Información MIME de la imágen</param>
        /// <returns></returns>
        public string GetAvatarImage(byte[] AvatarImage, string MIMEtype)
        {
            string defaultImage = "/Content/Images/monkey.png";

            if (AvatarImage != null)
            {
                try
                {
                    return String.Concat("data:", MIMEtype, ";base64,", Convert.ToBase64String(AvatarImage));
                }
                catch
                {
                    return defaultImage;
                }
            }

            return defaultImage;
        }

        /// <summary>
        /// Cambia la imágen de perfil del usuario.
        /// </summary>
        /// <param name="avatarImage">Imágen de perfil</param>
        /// <param name="MIMEType">Información MIME</param>
        /// <param name="email">Correo electrónico</param>
        /// <returns></returns>
        public bool ChangeAvatar(Image avatarImage, string MIMEType, string email)
        {
            try
            { 
                var context = OpenConnection();
                Person PersonData = context.Person.Where(p => p.Email == email).First();

                PersonData.AvatarImage = ConvertImageToByteArray(avatarImage);
                PersonData.AvatarMIMEType = MIMEType;
                context.SaveChanges();
                CloseConnection(context);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cambia la frase personal del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="phrase">Frase personal</param>
        /// <param name="phraseColor">Fuente de color de la frase</param>
        /// <returns></returns>
        public bool ChangePersonalPhraseDAL(string email, string phrase, string phraseColor)
        {
            try
            {
                var context = OpenConnection();
                Person PersonData = context.Person.Where(p => p.Email == email).First();
                PersonData.PersonalPhrase = phrase;
                PersonData.PhraseColor = phraseColor;
                context.SaveChanges();
                CloseConnection(context);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="currentPassword">Contraseña actual</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <param name="error">Posible error</param>
        /// <returns></returns>
        public bool ChangePasswordDAL(string email, string currentPassword, string newPassword, ref string error)
        {
            try
            {
                var context = OpenConnection();
                string CurrentPassword = context.Person.Where(p => p.Email == email).First().Password;
                if(CurrentPassword == currentPassword)
                {
                    CurrentPassword = newPassword;
                    context.SaveChanges();
                    CloseConnection(context);
                    return true;
                }

                error = "EL CAMPO CONTRASEÑA ACTUAL NO ES CORRECTO";
                return false;
            }
            catch
            {
                error = "SE HA PRODUCIDO UN ERROR DESCONOCIDO";
                return false;
            }            
        }
    }
}

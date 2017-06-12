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
                using (var context = new WinNotesDBEntities())
                {
                    var existingUser = context.Person.Any(p => p.Email == Email);

                    if (!existingUser && !CheckUserNameDAL(UserName))
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

                        return Email;
                    }                    
                    return null;
                }                
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
            using (var context = new WinNotesDBEntities())
            {
                bool result = context.Person.Any(p => p.UserName == UserName);
                return result;
            }                            
        }


        /// <summary>
        /// Recopila la información que se va a guardar como sesión de usuario.
        /// </summary>
        /// <param name="Email">Correo electrónico</param>
        /// <param name="Password">Contraseña</param>
        /// <returns></returns>
        public UserLoginData LoginDAL(string email, string password)
        {
            try
            {
                using (var context = new WinNotesDBEntities())
                {
                    Person user = context.Person.Where(p => p.Email == email && p.Password == password).FirstOrDefault();
                    UserLoginData login = null;

                    if (user != null)
                    {
                        login = new UserLoginData(
                                user.PersonID,
                                user.UserName,
                                user.Email,
                                GetAvatarImage(user.AvatarImage, user.AvatarMIMEType),
                                Convert.ToBoolean(user.Active)
                            );
                        user.LastLoginDate = DateTime.Now;
                        context.SaveChanges();
                    }

                    return login;
                }                
            }
            catch
            {
                throw;
            }
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
                using (var context = new WinNotesDBEntities())
                {
                    return context.Person.Where(p => p.Email == email).First().PersonIDEncrypted;                    
                }
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
        public UserLoginData RememberSessionInfoDAL(string encryptedID)
        {
            using (var context = new WinNotesDBEntities())
            {
                UserLoginData userLoggedIn = null;                
                Person user = context.Person.Where(p => p.PersonIDEncrypted == encryptedID).FirstOrDefault();

                if (user != null)
                {
                    userLoggedIn = new UserLoginData(
                                        user.PersonID,
                                        user.UserName,
                                        user.Email,
                                        GetAvatarImage(user.AvatarImage, user.AvatarMIMEType),
                                        Convert.ToBoolean(user.Active)
                                    );
                    user.LastLoginDate = DateTime.Now;
                    context.SaveChanges();
                }

                return userLoggedIn;
            }                
        }        

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

using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
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
        public string GetEncryptedUserID(int userID)
        {
            try
            {
                using (var context = new WinNotesEntities())
                {
                    return context.Person.Where(p => p.PersonID == userID).First().PersonIDEncrypted;
                }
            }
            catch
            {
                throw;
            }
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
                using (var context = new WinNotesEntities())
                {

                    #region FORMA ALTERNATIVA
                    //var existingUser = context.Person.Any(p => p.Email == Email);

                    //if (!existingUser && !CheckUserNameDAL(UserName))
                    //{
                    //    Person newUser = new Person();
                    //    newUser.UserName = UserName;
                    //    newUser.Email = Email;
                    //    newUser.Password = Password;
                    //    newUser.AvatarImage = null;
                    //    newUser.AvatarMIMEType = null;
                    //    newUser.Active = false;
                    //    newUser.RegistrationDate = DateTime.Now;
                    //    newUser.LastLoginDate = DateTime.Now;

                    //    context.Person.Add(newUser);
                    //    context.SaveChanges();
                    //    newUser.PersonIDEncrypted = this.EncryptToSHA256(newUser.PersonID);
                    //    context.SaveChanges();                        

                    //    return Email;
                    //}
                    //return null;
                    #endregion

                    var userID_objResult = context.sp_createNewUser(UserName, Email, Password).First();
                    if (userID_objResult.HasValue)
                    {
                        int userID = userID_objResult.Value;
                        context.sp_saveEncryptedUserID(userID, this.EncryptToSHA256(userID));

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
            using (var context = new WinNotesEntities())
            {
                //bool result = context.Person.Any(p => p.UserName == UserName);
                var result = new ObjectParameter("result", typeof(bool));                
                context.sp_verifyUserName(UserName, result);
                return (bool)result.Value;
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
                using (var context = new WinNotesEntities())
                {
                    sp_login_Result user = context.sp_login(email, password).First();
                    //Person user = context.Person.Where(p => p.Email == email && p.Password == password).FirstOrDefault();
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
                        //user.LastLoginDate = DateTime.Now;
                        //context.SaveChanges();
                        context.sp_refreshLoginDate(user.PersonID);
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
                using (var context = new WinNotesEntities())
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
                using (var context = new WinNotesEntities())
                {
                    Person person = context.Person.Where(p => p.Email == email).First();
                    person.PersonIDEncrypted = IdentifierEncrypted;
                    context.SaveChanges();
                }
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
            using (var context = new WinNotesEntities())
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
        /// <param name="userID">ID del usuario actual</param>
        /// <returns></returns>
        public string[] GetUserInformation(int userID)
        {
            try
            {
                using (var context = new WinNotesEntities())
                {
                    #region FORMA ALTERNATIVA
                    //Person PersonData = context.Person.Where(p => p.PersonID == userID).FirstOrDefault();
                    //var UserInformation = new string[] { GetAvatarImage(PersonData.AvatarImage, PersonData.AvatarMIMEType),
                    //                                 PersonData.PersonalPhrase,
                    //                                 PersonData.PhraseColor,
                    //                                 PersonData.UserName,
                    //                                 PersonData.Email,
                    //                                 PersonData.RegistrationDate.ToShortDateString()
                    //                               };
                    //return UserInformation;
                    #endregion

                    sp_getUserInformation_Result userInformation = context.sp_getUserInformation(userID).First();
                    return new string[]
                    {
                        this.GetAvatarImage(userInformation.AvatarImage, userInformation.AvatarMIMEType),
                        userInformation.PersonalPhrase,
                        userInformation.PhraseColor,
                        userInformation.UserName,
                        userInformation.Email,
                        userInformation.RegistrationDate.ToShortDateString()
                    };
                }
            }
            catch
            {
                throw;
            }
        }

        //public string[] GetWizardInformation(int userID)
        //{
        //    using (var context = new WinNotesEntities())
        //    {
        //        var user = context.Person.Where(p => p.PersonID == userID);
        //        return new string[]
        //        {
        //            this.GetAvatarImage()
        //        }
        //    }
        //}

        /// <summary>
        /// Devuelve la imágen de perfil expresada como base64. Por defecto: la imágen predeterminada de perfil.
        /// </summary>
        /// <param name="AvatarImage">Imágen de perfil como array de bytes</param>
        /// <param name="MIMEtype">Información MIME de la imágen</param>
        /// <returns></returns>
        public string GetAvatarImage(byte[] AvatarImage, string MIMEtype)
        {
            string defaultImage = "/Content/Images/user_profile.jpg";

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
        public void ChangeAvatar(Image avatarImage, string MIMEType, int userID)
        {
            try
            {
                using (var context = new WinNotesEntities())
                {
                    #region FORMA ALTERNATIVA
                    //Person PersonData = context.Person.Where(p => p.PersonID == userID).First();
                    //PersonData.AvatarImage = ConvertImageToByteArray(avatarImage);
                    //PersonData.AvatarMIMEType = MIMEType;
                    //context.SaveChanges();
                    #endregion

                    context.sp_changeAvatar(userID, ConvertImageToByteArray(avatarImage), MIMEType);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Cambia la frase personal del usuario.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="phrase">Frase personal</param>
        /// <param name="phraseColor">Fuente de color de la frase</param>
        /// <returns></returns>
        public void ChangePersonalPhraseDAL(int userID, string phrase, string phraseColor)
        {
            try
            {
                using (var context = new WinNotesEntities())
                {
                    #region FORMA ALTERNATIVA
                    //Person PersonData = context.Person.Where(p => p.PersonID == userID).First();
                    //PersonData.PersonalPhrase = phrase;
                    //PersonData.PhraseColor = phraseColor;
                    //context.SaveChanges();
                    #endregion

                    context.sp_changePersonalPhrase(userID, phrase, phraseColor);
                }
            }
            catch
            {
                throw;
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
        public void ChangePasswordDAL(int userID, string currentPassword, string newPassword)
        {
            try
            {
                using (var context = new WinNotesEntities())
                {
                    #region FORMA ALTERNATIVA
                    //var user = context.Person.Where(p => p.PersonID == userID).First();
                    //if (user.Password == currentPassword)
                    //{
                    //    user.Password = newPassword;
                    //    context.SaveChanges();                        
                    //}
                    //else
                    //    throw new ArgumentException("EL CAMPO CONTRASEÑA ACTUAL NO ES CORRECTO");
                    #endregion

                    context.sp_changePassword(userID, currentPassword, newPassword);
                }
            }
            catch
            {
                throw;
            }
        }

        // METODOS PARA WIZARD
        public string AvatarInfoForWizardDAL(int userID)
        {
            using (var context = new WinNotesEntities())
            {
                var userInfo = context.Person.Where(u => u.PersonID == userID).FirstOrDefault();

                return this.GetAvatarImage(userInfo.AvatarImage, userInfo.AvatarMIMEType);
            }
        }

        public string[] PhraseInfoForWizardDAL(int userID)
        {
            using (var context = new WinNotesEntities())
            {
                var userInfo = context.Person.Where(u => u.PersonID == userID).FirstOrDefault();
                return new string[]
                {
                    userInfo.PersonalPhrase,
                    userInfo.PhraseColor
                };
            }
        }


        public string TemporaryAvatarDAL(HttpPostedFile tempAvatar, HttpServerUtilityBase localServer, int userID)
        {
            try
            {
                if (tempAvatar == null)
                    throw new ArgumentNullException("Debe elegir una imágen.");

                string imgDir;                
                using (var context = new WinNotesEntities())
                {
                    imgDir = "/Content/Temp/" + context.Person.Where(p => p.PersonID == userID).Single().UserName;
                }
                                   
                Directory.CreateDirectory(localServer.MapPath(imgDir));
                string imgFullPath = localServer.MapPath(imgDir) + "/" + tempAvatar.FileName;                

                // Get file data
                byte[] data = new byte[] { };
                using (var binaryReader = new BinaryReader(tempAvatar.InputStream))
                {
                    data = binaryReader.ReadBytes(tempAvatar.ContentLength);                    
                }                

                // Guardar imagen en el servidor
                using (FileStream image = File.Create(imgFullPath, data.Length))
                {
                    image.Write(data, 0, data.Length);
                }

                // Verifica si la imágen cumple las condiciones de validación
                const int _maxSize = 2 * 1024 * 1024;
                const int _maxWidth = 1000;
                const int _maxHeight = 1000;
                List<string> _fileTypes = new List<string>() { "jpg", "jpeg", "gif", "png" };
                string fileExt = Path.GetExtension(tempAvatar.FileName);

                if (new FileInfo(imgFullPath).Length > _maxSize)
                    throw new FormatException("El avatar no debe superar los 2mb.");

                if (!_fileTypes.Contains(fileExt.Substring(1), StringComparer.OrdinalIgnoreCase))
                    throw new FormatException("Para el avatar solo se admiten imágenes JPG, JPEG, GIF Y PNG.");

                using (Image img = Image.FromFile(imgFullPath))
                {
                    if (img.Width > _maxWidth || img.Height > _maxHeight)
                        throw new FormatException("El avatar admite hasta una resolución de 1000x1000.");
                }                

                return imgDir + "/" + tempAvatar.FileName;
            }
            catch
            {
                throw;
            }
        }

        public void UpdateAvatar(string path, int userID)
        {
            bool deleteTemporaryFiles = false;

            if (!String.IsNullOrEmpty(path))
            {
                using (var context = new WinNotesEntities())
                {
                    var user = context.Person.Where(p => p.PersonID == userID).FirstOrDefault();

                    if (path != "/Content/Images/user_profile.jpg")
                    {
                        if(!path.StartsWith("data:image"))
                            using (Image img = CreateImageFromPathString(path))
                            {
                                System.Drawing.Imaging.ImageFormat format = img.RawFormat;
                                System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == format.Guid);
                                user.AvatarImage = ConvertImageToByteArray(img);
                                user.AvatarMIMEType = codec.MimeType;
                                context.SaveChanges();
                                deleteTemporaryFiles = true;
                            }
                    }
                    else
                    {
                        user.AvatarImage = null;
                        user.AvatarMIMEType = null;
                        context.SaveChanges();
                    }
                }

                if(deleteTemporaryFiles)
                    DeleteDirectory(path);
            }
        }

        public Image CreateImageFromPathString(string path)
        {
            return Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + path);            
        }

        public static void DeleteDirectory(string path)
        {
            int pos = path.LastIndexOf("/");
            string folderPath = path.Substring(0, pos);
            string directoryPath = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + folderPath);
            string[] files = Directory.GetFiles(directoryPath);
            string[] dirs = Directory.GetDirectories(directoryPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(directoryPath, false);
        }
    }
}

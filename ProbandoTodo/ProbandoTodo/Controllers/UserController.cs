using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProbandoTodo.Models;
using Business_Logic_Layer;
using static ProbandoTodo.Filters.CustomFilters;
using Domain_Layer;
using System.Data.SqlClient;

namespace ProbandoTodo.Controllers
{
    public class UserController : Controller
    {
        static UserBLL userBLL = new UserBLL();

        #region MANEJADOR DE ERRORES

        /// <summary>
        /// Recibe la excepción y devuelve su correspondiente acción basado en los parámetros de entrada
        /// </summary>
        /// <param name="ex">Excepción que arroja el servidor</param>
        /// <param name="action">Nombre de la acción a la que se dirige</param>
        /// <param name="controller">Nombre del controlador al que se dirige. Por defecto, el controlador es User</param>
        /// <param name="forceError500">(Opcional) Indica si va a forzar un Internal Server Error</param>
        /// <returns></returns>
        private RedirectToRouteResult ManageException(Exception ex, string action, string controller = "User", bool forceError500 = false)
        {
            if(forceError500)
            {
                if (ex.InnerException is SqlException)
                {
                    return RedirectToAction("InternalServerError", "Error", new { error = ex.InnerException.Message });
                }

                return RedirectToAction("InternalServerError", "Error", new { error = ex.Message });
            }

            if (ex.InnerException is SqlException)
                TempData["error"] = ex.InnerException.Message;
            else
                TempData["error"] = ex.Message;

            return RedirectToAction(action, controller);
        }

        #endregion

        // GET: User
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult Register()
        {
            return PartialView("_Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    string EmailParsed = userBLL.CreateUser(model.UserName, model.Email, model.MailProvider, model.Password);
                    if (EmailParsed != null)
                    {
                        Session["UserLoggedIn"] = userBLL.Login(EmailParsed, model.Password);
                        return Json(new { url = "Home/Index" });
                    }
                    ViewData["SendingFormError"] = "- Error inesperado. Inténtelo nuevamente.";
                    return PartialView("_Register", model);
                }
                else
                {
                    return PartialView("_Register", model);
                }
            }
            catch(Exception ex)
            {                
                return this.ManageException(ex, null, null, true);
            }
        }

        public bool CheckUserName(string name)
        {
            return userBLL.CheckUserName(name);
        }

        public ActionResult Login()
        {
            return PartialView("_Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string urlPath)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    var userData = userBLL.Login(model.Email, model.Password);
                    if (userData != null)
                    {
                        Session["UserLoggedIn"] = userData;
                        if (model.RememberMe)
                            this.SetCookieData(model.Email);

                        return Json(new { url = urlPath });
                    }

                    throw new ArgumentException("Email y/o contraseña incorrecta");
                }

                return PartialView("_Login", model);
            }
            catch(Exception ex)
            {                
                return this.ManageException(ex, null, null, true);
            }
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            Session.Abandon();
            Session.Clear();

            if (Request.Cookies.AllKeys.Contains("UHICK"))
            {
                HttpCookie cookie = new HttpCookie("UHICK");
                //HttpCookie cookie = Request.Cookies["UHICK"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [WithAccount]
        public ActionResult ProfileManagement()
        {
            try
            {
                int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
                return View(new ProfileManagementModels(userBLL.GetUserInformation(userID)));
            }
            catch(Exception ex)
            {                
                return this.ManageException(ex, "Index", "Home");                
            }
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAvatar(HttpPostedFileBase UploadAvatar)
        {
            try
            {
                int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
                userBLL.ChangeAvatar(UploadAvatar, userID);
                return RedirectToAction("ProfileManagement");
            }
            catch(Exception ex)
            {
                return this.ManageException(ex, "ProfileManagement");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalPhrase(ProfileManagementModels.PersonalPhraseViewModel model, string pColor)
        {
            try
            {                
                int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
                if (ModelState.IsValid)
                {
                    userBLL.ChangePersonalPhrase(userID, model.PersonalPhrase, pColor);
                    return RedirectToAction("ProfileManagement");
                }
                    
                return View("ProfileManagement", new ProfileManagementModels(userBLL.GetUserInformation(userID), model));
            }
            catch(Exception ex)
            {
                return this.ManageException(ex, "ProfileManagement");
            }
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ProfileManagementModels.ChangePasswordSectionViewModel model)
        {
            try
            {
                int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
                if (ModelState.IsValid)
                {
                    userBLL.ChangePassword(userID, model.CurrentPassword, model.NewPassword);
                    return RedirectToAction("ProfileManagement");
                }

                return View("ProfileManagement", new ProfileManagementModels(userBLL.GetUserInformation(userID), model));
            }
            catch(Exception ex)
            {
                return this.ManageException(ex, "ProfileManagement");                
            }
        }

        public void SetCookieData(string email)
        {
            try
            {
                HttpCookie userCookie = new HttpCookie("UHICK");
                userCookie.Domain = "localhost";
                userCookie.Expires = DateTime.Now.AddDays(15);
                userCookie.Path = "/";
                userCookie.Secure = false;
                userCookie.Value = userBLL.RetrieveEncryptedID(email);
                Response.Cookies.Add(userCookie);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult ProfileWizard()
        {
            int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
            return View(new ProfileManagementModels(userBLL.GetUserInformation(userID)));            
        }

        public ActionResult AvatarDialog()
        {
            var model = new ProfileManagementModels.AvatarSectionViewModel();
            model.AvatarSource = userBLL.AvatarInfoForWizardBLL(UserLoginData.GetSessionID(Session["UserLoggedIn"]));
            return PartialView("_AvatarDialog", model);
        }

        public ActionResult PersonalMessageDialog()
        {
            int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
            var model = new ProfileManagementModels.PersonalPhraseViewModel();
            var dataForModel = userBLL.PhraseInfoForWizardDAL(userID);
            model.PersonalPhrase = dataForModel[0];
            model.PhraseColor = dataForModel[1];
            return PartialView("_PersonalMessageDialog", model);
        }

        public ActionResult PasswordDialog()
        {
            return PartialView("_PasswordDialog");
        }

        public ActionResult PreviewDialog()
        {
            return PartialView("_PreviewDialog");
        }

        public string StoreTempAvatar()
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
                    var pic = System.Web.HttpContext.Current.Request.Files["TempFile"];
                    return userBLL.TemporaryAvatarBLL(pic, Server, userID);
                }
            }
            catch(Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                return ex.Message;
            }            

            return null;
        }

        [HttpPost]
        public string WizardComplete(WizardModel model)
        {            
            if (model.personalMessage.phrase.Length > 140)
            {                
                Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                return "El mensaje personal superó los 140 caracteres.";
            }
                                    
            int userID = UserLoginData.GetSessionID(Session["UserLoggedIn"]);
            userBLL.UpdateAvatar(model.avatarImg, userID);
            userBLL.ChangePersonalPhrase(userID, model.personalMessage.phrase, model.personalMessage.color);

            return "Cambios guardados.";            
        }
    }
}
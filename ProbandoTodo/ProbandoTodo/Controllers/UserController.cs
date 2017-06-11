using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProbandoTodo.Models;
using Business_Logic_Layer;
using static ProbandoTodo.Filters.CustomFilters;

namespace ProbandoTodo.Controllers
{
    public class UserController : Controller
    {
        static UserBLL userBLL = new UserBLL();        

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
                        Session["UserLogged"] = userBLL.Login(EmailParsed, model.Password);
                        return Json(new { url = "Home/Index" });
                    }
                    return PartialView("_Register", model);
                }
                else
                {
                    return PartialView("_Register", model);
                }
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = ex.Message, source = ex.Source, stackTrace = ex.StackTrace });
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
            if(ModelState.IsValid)
            {                                                
                var QueryResult = userBLL.Login(model.Email, model.Password);                
                
                if(QueryResult != null)
                {
                    if (model.RememberMe)
                    {                                                                        
                        SetCookieData(model.Email);
                    }
                    Session["UserLogged"] = QueryResult;
                    return Json(new { url = urlPath });
                }

                ModelState.AddModelError(String.Empty, "No hay usuarios con el email ingresado");
                return PartialView("_Login", model);
            }

            return PartialView("_Login", model);
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            Session.Abandon();
            if (Request.Cookies.AllKeys.Contains("UHICK"))
            {
                HttpCookie cookie = Request.Cookies["UHICK"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index", "Home");
        }

        [OnlyUser]
        public ActionResult ProfileManagement()
        {
            try
            {
                string email = ((string[])Session["UserLogged"])[2];
                return View(FillProfileManagementView(email));
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private ProfileManagementModels FillProfileManagementView(string email)
        {            
            ProfileManagementModels model = new ProfileManagementModels();
            string[] userInformation = userBLL.GetUserInformation(email);

            model.AvatarSectionModel.AvatarSource = userInformation[0];
            model.PersonalPhraseModel.PersonalPhrase = userInformation[1];
            model.InformationSectionModel.UserName = userInformation[2];
            model.InformationSectionModel.Email = userInformation[3];
            model.PersonalPhraseModel.PhraseColor = userInformation[4];

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAvatar(HttpPostedFileBase UploadAvatar)
        {
            string email = ((string[])Session["UserLogged"])[2];
            string error = String.Empty;

            if (UploadAvatar != null && userBLL.ChangeAvatar(UploadAvatar, email, ref error))
            {
                return RedirectToAction("ProfileManagement");
            }

            ViewBag.error = error;
            
            return View("ProfileManagement", FillProfileManagementView(email));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalPhrase(ProfileManagementModels.PersonalPhraseViewModel model, string pColor)
        {
            string email = ((string[])Session["UserLogged"])[2];
            string error = String.Empty;

            if (ModelState.IsValid && userBLL.ChangePersonalPhrase(email, model.PersonalPhrase, pColor))
            {
                return RedirectToAction("ProfileManagement");
            }

            ViewBag.error = "MENSAJE PERSONAL NO VÁLIDO";
            return View("ProfileManagement", FillProfileManagementView(email));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword)
        {
            string email = ((string[])Session["UserLogged"])[2];
            string error = String.Empty;

            if (ModelState.IsValid && userBLL.ChangePassword(email, CurrentPassword, NewPassword, ref error))
            {
                return RedirectToAction("ProfileManagement");
            }

            ViewBag.error = error;

            return View("ProfileManagement", FillProfileManagementView(email));
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
            catch(Exception ex)
            {
                throw ex;
            }
        }           
    }
}
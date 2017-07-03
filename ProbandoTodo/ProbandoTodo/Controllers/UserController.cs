using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProbandoTodo.Models;
using Business_Logic_Layer;
using static ProbandoTodo.Filters.CustomFilters;
using Domain_Layer;

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
                        Session["UserLoggedIn"] = userBLL.Login(EmailParsed, model.Password);
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
                return Json(new { error = ex.Message }, JsonRequestBehavior.DenyGet);
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
                    var user = userBLL.Login(model.Email, model.Password);
                    if (user != null)
                    {
                        Session["UserLoggedIn"] = user;
                        if (model.RememberMe)
                            this.SetCookieData(model.Email);

                        return Json(new { url = urlPath });
                    }

                    ModelState.AddModelError(string.Empty, "Email/contraseña incorrecta");
                    return PartialView("_Login", model);
                }

                return PartialView("_Login", model);
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message }, JsonRequestBehavior.DenyGet);
            }
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
                TempData["error"] = ex.Message;
                return RedirectToAction("Index", "Home");
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
                ViewBag.error = ex.Message;
                return View("ProfileManagement", new ProfileManagementModels(userBLL.GetUserInformation(UserLoginData.GetSessionID(Session["UserLoggedIn"]))));
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
                TempData["error"] = ex.Message;
                return View("ProfileManagement", new ProfileManagementModels(userBLL.GetUserInformation(UserLoginData.GetSessionID(Session["UserLoggedIn"])), model));
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

                return View("ProfileManagement", new ProfileManagementModels(userBLL.GetUserInformation(UserLoginData.GetSessionID(Session["UserLoggedIn"])), model));
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return View("ProfileManagement", new ProfileManagementModels(userBLL.GetUserInformation(UserLoginData.GetSessionID(Session["UserLoggedIn"])), model));
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
    }
}
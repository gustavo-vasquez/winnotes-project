using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProbandoTodo.Controllers
{
    public class ErrorController : Controller
    {                
        public ActionResult NotFound(string aspxerrorpath)
        {
            Response.StatusCode = 404;
            ViewBag.pathError = aspxerrorpath;

            if (Request.IsAjaxRequest())
                return PartialView("_NotFound");
            else
                return View();
        }

        public ActionResult InternalServerError(string error)
        {            
            Response.StatusCode = 500;

            if (Request.IsAjaxRequest())
                return PartialView("_InternalServerError", error);
            else
                return View();
        }

        public ActionResult NotAuthorized()
        {
            Response.StatusCode = 403;

            if (Request.IsAjaxRequest())
                return PartialView("_NotAuthorized");
            else
                return View();
        }
    }
}
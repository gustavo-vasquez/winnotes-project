using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProbandoTodo.Filters
{
    public class CustomFilters
    {
        public class RememberSessionAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                var sessionCookie = filterContext.HttpContext.Request.Cookies["UHICK"];

                if (sessionCookie != null && filterContext.HttpContext.Session["UserLoggedIn"] == null)
                {
                    filterContext.HttpContext.Session["UserLoggedIn"] = new Business_Logic_Layer.UserBLL().RememberSessionInfo(sessionCookie.Value);                    
                }
            }
        }

        public class WithAccountAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Session["UserLoggedIn"] == null)
                {
                    filterContext.Result = new HttpStatusCodeResult(403, "Acceso no autorizado - Solo usuarios registrados pueden ver el contenido");
                }
            }            
        }
                
        internal class HttpStatusCodeResult : ActionResult
        {
            private int _code { get; set; }
            private string _description { get; set; }

            /// <summary>
            /// Devuelve el codigo error suministrado como respuesta.
            /// </summary>
            /// <param name="code">Código de error</param>
            /// <param name="description">Descripción del error</param>
            /// <returns></returns>
            public HttpStatusCodeResult(int code, string description)
            {
                this._code = code;
                this._description = description;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.StatusCode = _code;
                context.HttpContext.Response.StatusDescription = _description;
            }
        }
    }
}
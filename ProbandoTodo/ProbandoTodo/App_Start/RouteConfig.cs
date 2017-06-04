using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProbandoTodo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UserRoute",
                url: "MyUser/User/{action}",
                defaults: new { controller = "User", action = "Index" }
            );

            routes.MapRoute(
                name: "NotesRoute",
                url: "MyUser/Note/{action}",
                defaults: new { controller = "Note", action = "Index" }
            );

            routes.MapRoute(
                name: "FoldersRoute",
                url:  "MyUser/Folder/{action}/{folderID}",
                defaults: new { controller = "Folder", action = "Index", folderID = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

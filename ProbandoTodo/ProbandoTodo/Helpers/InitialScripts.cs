using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace ProbandoTodo.Helpers
{
    // Idea para la carga automática de scripts segun la vista (desechada)
    public static class InitialScripts
    {
        public static IHtmlString Load()
        {
            string currentPath = HttpContext.Current.Request.Url.AbsolutePath;
            IHtmlString htmlString = null;

            switch (currentPath)
            {
                case "/Home/Contact":
                    htmlString = GenerateScript(false, "http://maps.googleapis.com/maps/api/js", "google.maps");
                    break;
                case "/User/ProfileManagement":
                    htmlString = GenerateScript(true, "profile-management.view");
                    break;
                case "/Folder/List":
                    htmlString = GenerateScript(true);
                    break;
            }

            return htmlString;
        }

        private static IHtmlString GenerateScript(bool validate, params string[] filename)
        {
            List<string> scriptPathList = new List<string>();

            foreach (var f in filename)
            {
                if (f.StartsWith("http:") || f.StartsWith("https:"))
                {
                    scriptPathList.Add(f);
                }
                else
                {
                    scriptPathList.Add(String.Concat("~/Scripts/application/", f, ".js"));
                }
            }

            if (validate)
            {
                scriptPathList.Add("~/bundles/jqueryval");
            }

            string[] scriptPathArray = scriptPathList.ToArray();

            return ScriptsExtension.RenderDefer(scriptPathArray);
        }        
    }    
}
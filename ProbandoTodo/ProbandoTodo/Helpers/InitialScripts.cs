using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace ProbandoTodo.Helpers
{
    public static class InitialScripts
    {
        public static IHtmlString myScripts { get; set; }

        public static void Add(params string[] src)
        {
            myScripts = Scripts.Render(src);
        }

        public static IHtmlString Write()
        {
            return myScripts;
        }
        //public static IHtmlString Load()
        //{
        //    string currentPath = HttpContext.Current.Request.Url.AbsolutePath;
        //    IHtmlString htmlString = null;            

        //    switch(currentPath)
        //    {
        //        case "/Home/Contact":
        //            htmlString = GenerateScript(false, "http://maps.googleapis.com/maps/api/js", "google.maps");
        //            break;
        //        case "/User/ProfileManagement":
        //            htmlString = GenerateScript(true, "profile-management.view");
        //            break;
        //        case "/Folder/List":
        //            htmlString = GenerateScript(true);
        //            break;
        //    }

        //    return htmlString;
        //}

        //private static IHtmlString GenerateScript(bool validate, params string[] filename)
        //{
        //    List<string> scriptPathList = new List<string>();            

        //    foreach (var f in filename)
        //    {
        //        if(f.StartsWith("http:") || f.StartsWith("https:"))
        //        {
        //            scriptPathList.Add(f);
        //        }
        //        else
        //        {
        //            scriptPathList.Add(String.Concat("~/Scripts/application/", f, ".js"));
        //        }                
        //    }

        //    if (validate)
        //    {
        //        scriptPathList.Add("~/bundles/jqueryval");
        //    }

        //    string[] scriptPathArray = scriptPathList.ToArray();

        //    return ScriptsExtension.RenderDefer(scriptPathArray);
        //}        

        //private static IEnumerable<MethodInfo> ControllersInAssembly()
        //{
        //    Assembly assembly = Assembly.GetExecutingAssembly();

        //    return assembly.GetTypes()
        //                .Where(type => typeof(Controller).IsAssignableFrom(type)) //filter controllers
        //                .SelectMany(type => type.GetMethods())
        //                .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)));
            
        //}
    }
}
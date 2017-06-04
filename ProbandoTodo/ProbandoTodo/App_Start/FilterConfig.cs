using System.Web;
using System.Web.Mvc;
using static ProbandoTodo.Filters.CustomFilters;

namespace ProbandoTodo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RememberSessionAttribute());
        }
    }
}

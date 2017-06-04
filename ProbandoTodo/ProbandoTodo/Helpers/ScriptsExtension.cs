using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ProbandoTodo.Helpers
{
    public class ScriptsExtension
    {
        /// <summary>
        /// Carga el script con el atributo 'defer'
        /// </summary>
        /// <param name="src">Ruta del archivo js</param>
        /// <returns>Devulve el tag script armado</returns>
        public static IHtmlString RenderDefer(params string[] src)
        {                                    
            return Scripts.RenderFormat("<script src='{0}' defer></script>", src);
        }

        /// <summary>
        /// Carga el script con el atributo 'async'
        /// </summary>
        /// <param name="src">Ruta del archivo js</param>
        /// <returns>Devuelve tag script armado</returns>
        public static IHtmlString RenderAsync(params string[] src)
        {
            return Scripts.RenderFormat("<script src='{0}' async></script>", src);
        }
    }
}
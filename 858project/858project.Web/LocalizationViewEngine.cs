using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Engine zabezpecujuci zobrazenie suboru podla jazykovej mutacie
    /// </summary>
    public sealed class LocalizationViewEngine : RazorViewEngine
    {
        /// <summary>
        /// Vrati partial view
        /// </summary>
        /// <param name="controllerContext">ControllerContext</param>
        /// <param name="viewPath">Cesta k view suboru</param>
        /// <returns>View</returns>
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            partialPath = GlobalizeViewPath(controllerContext, partialPath);
            return base.CreatePartialView(controllerContext, partialPath);
        }
        /// <summary>
        /// Vrati view
        /// </summary>
        /// <param name="controllerContext">ControllerContext</param>
        /// <param name="viewPath">Cesta k view suboru</param>
        /// <param name="masterPath">Cesta</param>
        /// <returns>View</returns>
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            viewPath = GlobalizeViewPath(controllerContext, viewPath);
            return base.CreateView(controllerContext, viewPath, masterPath);
        }
        /// <summary>
        /// Vrati pozadovanu cestu k lokalizovanemu suboru alebo default
        /// </summary>
        /// <param name="controllerContext">ControllerContext</param>
        /// <param name="viewPath">Povodna cesta k suboru</param>
        /// <returns>Cesta k suboru</returns>
        private String GlobalizeViewPath(ControllerContext controllerContext, string viewPath)
        {
            var language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (!String.IsNullOrWhiteSpace(language) && language.IndexOf("en", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                String localizedViewPath = viewPath.Replace(".cshtml", "." + language + ".cshtml");
                var request = controllerContext.HttpContext.Request;
                if (File.Exists(request.MapPath(localizedViewPath)))
                {
                    viewPath = localizedViewPath;
                }
            }
            return viewPath;
        }
    }
}

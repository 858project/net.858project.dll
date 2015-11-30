using Project858.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Atribut zabezpecujuci validaciu modelu prijatych dat
    /// </summary>
    public sealed class FormAttribute : ActionFilterAttribute
    {
        #region - Public Methods -
        /// <summary>
        /// Spracuje udalost
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var modelState = filterContext.Controller.ViewData;
            var model = filterContext.ActionParameters["model"];
            if (!modelState.ModelState.IsValid || (model != null && model is IModel && !(model as IModel).Validate()))
            {
                filterContext.Controller.PrintModelStateError();
                filterContext.Result = WebUtility.GetJsonResult(ResponseTypes.ModelIsNotValidError);
            }
            base.OnActionExecuting(filterContext);
        }
        #endregion`
    }
}

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
            var viewData = filterContext.Controller.ViewData;
            var model = filterContext.ActionParameters["model"];
            if (!viewData.ModelState.IsValid || (model != null && model is IModel && !(model as IModel).Validate()))
            {
                filterContext.Controller.PrintModelStateError();

                //vytvorime zoznam chyb
                List<String> errors = new List<String>();
                foreach (ModelState modelState in viewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                //vratime result aj s popisom chyb
                filterContext.Result = WebUtility.GetJsonResult(ResponseTypes.ModelIsNotValidError, null, null, errors, JsonRequestBehavior.AllowGet);
            }
            base.OnActionExecuting(filterContext);
        }
        #endregion`
    }
}

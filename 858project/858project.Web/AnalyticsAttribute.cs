using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Atribut na vytvaranie statistik navstevnosti
    /// </summary>
    public sealed class AnalyticsAttribute : ActionFilterAttribute
    {
        #region - Public Methods -
        /// <summary>
        /// Spracuje udalost
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null) 
            {
                var context = filterContext.HttpContext;
                if (context != null)
                {
                    var request = context.Request;
                    var response = context.Response;
                    if (request != null && response != null)
                    {
                        WebApplication.AnalyticsProcess(request, response);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
        #endregion
    }
}

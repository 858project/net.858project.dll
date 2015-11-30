using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Result s nastavenim HTTP basic autorizacie
    /// </summary>
    public sealed class HttpBasicUnauthorizedResult : HttpUnauthorizedResult
    {
        #region  - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public HttpBasicUnauthorizedResult() : base() { }
        /// <summary>
        /// Initialize this class
        /// </summary>
        public HttpBasicUnauthorizedResult(string statusDescription) : base(statusDescription) { }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// ExecuteResult
        /// </summary>
        /// <param name="context">ControllerContext</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            context.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic");
            context.HttpContext.Response.AddHeader("Authorization", String.Empty);
            base.ExecuteResult(context);
        }
        #endregion
    }
}

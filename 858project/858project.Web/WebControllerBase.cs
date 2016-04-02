using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Bese controller with implementation
    /// </summary>
    public abstract class WebControllerBase : Controller
    {
        #region - Private Methods -
        /// <summary>
        /// InternalSendEmail
        /// </summary>
        protected Boolean SendEmail(String viewName, String masterName, Object model, String subject, String copyAddress, String senderAddress, String replyAddress, params String[] address)
        {
            return this.InternalSendEmail(viewName, masterName, model, subject, copyAddress, senderAddress, replyAddress, address);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// InternalSendEmail
        /// </summary>
        private Boolean InternalSendEmail(String viewName, String masterName, Object model, String subject, String copyAddress, String senderAddress, String replyAddress, params String[] address)
        {
            try
            {
                // find view
                ViewEngineResult viewEngineResult = ViewEngines.Engines.FindView(this.ControllerContext, viewName, masterName);

                if (viewEngineResult == null)
                    throw new FileNotFoundException("View cannot be found.");

                // get view
                var view = viewEngineResult.View;
                this.ControllerContext.Controller.ViewData.Model = model;

                // render
                using (StringWriter stringWriter = new StringWriter())
                {
                    var ctx = new ViewContext(this.ControllerContext,
                                              view,
                                              this.ControllerContext.Controller.ViewData,
                                              this.ControllerContext.Controller.TempData,
                                              stringWriter);
                    view.Render(ctx, stringWriter);

                    //send email
                    return this.InternalSendEmail(stringWriter.ToString(), subject, copyAddress, senderAddress, replyAddress, address);
                }
            }
            catch (Exception ex)
            {
                WebUtility.Trace(ex);
                return false;
            }
        }
        /// <summary>
        /// InternalSendEmail
        /// </summary>
        private Boolean InternalSendEmail(String content, String subject, String copyAddress, String senderAddress, String replyAddress, params String[] address)
        {
            List<String> collection = new List<string>(address);
            if (collection.Count == 0)
            {
                collection.Add(WebConfigurationManager.AppSettings["SmtpRecipient"]);
            }
            return WebApplication.SendMailMessage(content, subject, copyAddress, senderAddress, replyAddress, collection);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// An ActionResult used to send binary data to the browser.
    /// </summary>
    public class BinaryContentResult : ActionResult
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="contentBytes">Content v bytoch</param>
        /// <param name="contentType">Typ contentu</param>
        public BinaryContentResult(byte[] contentBytes, string contentType)
        {
            this.m_contentBytes = contentBytes;
            this.m_contentType = contentType;
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Definuje typ contentu
        /// </summary>
        private String m_contentType = null;
        /// <summary>
        /// Content
        /// </summary>
        private Byte[] m_contentBytes = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Spracuje poziadavku a vrati content
        /// </summary>
        /// <param name="context">ControllerContext</param>
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.ContentType = this.m_contentType;

            using (var stream = new MemoryStream(this.m_contentBytes))
            {
                stream.WriteTo(response.OutputStream);
                stream.Flush();
            }
        }
        #endregion
    }
}

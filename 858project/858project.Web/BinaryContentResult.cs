/*
The MIT License
Copyright 2012-2015 (c) 858 Project s.r.o. <info@858project.com>

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit
persons to whom the Software is furnished to do so, subject to the
following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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

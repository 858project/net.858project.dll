using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Project858;
using Project858.Web;
using System.Drawing.Drawing2D;
using Project858.ComponentModel;

namespace Project858.Web
{
    /// <summary>
    /// Handler na spracovanie obrazkov
    /// </summary>
    public sealed class WebFileHandler : IHttpHandler
    {
        #region - Properties -
        /// <summary>
        /// Definuje ci je mozne objekt opakovane pouzit
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Spracuje request poziadavku na nacitanie obrazku
        /// </summary>
        /// <param name="context">HttpContext</param>
        public void ProcessRequest(HttpContext context)
        {
            String file = context.Request.RawUrl.Replace("/file/", String.Empty);
            Nullable<Guid> key = file.Replace(".data", String.Empty).ToGuidWithoutDash();
            if (key.HasValue)
            {
                FileModelBase fileModel = null;
                if (WebConfiguration.OnFileLoadDelegate != null)
                {
                    fileModel = WebConfiguration.OnFileLoadDelegate(key.Value);
                }
                if (fileModel != null)
                {
                    context.Response.ContentType = fileModel.ContentType;
                    context.Response.BinaryWrite(fileModel.Data);
                    return;
                }
            }

            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "File not found !";
            context.Response.End();
        }
        #endregion
    }
}

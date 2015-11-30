using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Controller na spracovanie a vykreslenie PDF dokumentu
    /// </summary>
    public abstract class PdfViewController : Controller
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public PdfViewController()
        {
            this.m_htmlViewRenderer = new HtmlViewRenderer();
            this.m_standardPdfRenderer = new StandardPdfRenderer();
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Renderer na vykreslenie html obsahu
        /// </summary>
        private HtmlViewRenderer m_htmlViewRenderer = null;
        /// <summary>
        /// Renderer na vykreslenie pdf dokumentu
        /// </summary>
        private StandardPdfRenderer m_standardPdfRenderer = null;
        #endregion

        #region - Protected Methods -
        /// <summary>
        /// Spracuje a vytvori PDF dokument z pozadovaneho view
        /// </summary>
        /// <param name="title">Nazov dokumentu</param>
        /// <param name="viewName">Meno view ktore chceme vykreslit</param>
        /// <param name="model">Model dat na vykreslenie view</param>
        /// <param name="pageSize">Velkost stranky</param>
        /// <returns>ActionResult</returns>
        protected ActionResult ViewPdf(string title, string viewName, object model)
        {
            return this.ViewPdf(title, viewName, model, PageSize.A4);
        }
        /// <summary>
        /// Spracuje a vytvori PDF dokument z pozadovaneho view
        /// </summary>
        /// <param name="title">Nazov dokumentu</param>
        /// <param name="viewName">Meno view ktore chceme vykreslit</param>
        /// <param name="model">Model dat na vykreslenie view</param>
        /// <param name="pageSize">Velkost stranky</param>
        /// <returns>ActionResult</returns>
        protected ActionResult ViewPdf(string title, string viewName, object model, Rectangle pageSize)
        {
            // Render the view html to a string.
            string htmlText = this.m_htmlViewRenderer.RenderViewToString(this, viewName, model);

            // Let the html be rendered into a PDF document through iTextSharp.
            byte[] buffer = this.m_standardPdfRenderer.Render(htmlText, title, pageSize);

            // Return the PDF as a binary stream to the client.
            return new BinaryContentResult(buffer, "application/pdf");
        }
        #endregion
    }
}

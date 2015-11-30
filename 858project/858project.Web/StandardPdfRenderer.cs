using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace Project858.Web
{
    /// <summary>
    /// Renderer na vykreslenie PDF
    /// </summary>
    public sealed class StandardPdfRenderer
    {
        #region - Constants -
        /// <summary>
        /// Definuje horizontalny okraj
        /// </summary>
        private const int HORIZONTAL_MARGIN = 40;
        /// <summary>
        /// Defninuje vertikalny okraj
        /// </summary>
        private const int VERTICAL_MARGIN = 40;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vykresli data
        /// </summary>
        /// <param name="htmlText">Html text na vykreslenie</param>
        /// <param name="pageTitle">Nazov strany</param>
        /// <param name="pageSize">Velkost stranky</param>
        /// <returns>Vykreslenie v bytoch</returns>
        public byte[] Render(string htmlText, string pageTitle, Rectangle pageSize)
        {
            byte[] renderedBuffer;

            using (var outputMemoryStream = new MemoryStream())
            {
                using (var pdfDocument = new Document(pageSize, HORIZONTAL_MARGIN, HORIZONTAL_MARGIN, VERTICAL_MARGIN, VERTICAL_MARGIN))
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDocument, outputMemoryStream);
                    if (!String.IsNullOrWhiteSpace(pageTitle))
                    {
                        pdfWriter.PageEvent = new PdfHeaderAndFooterPrinter { Title = pageTitle };
                    }
                    pdfWriter.CloseStream = false;
                    pdfDocument.Open();

                    using (var htmlViewReader = new StringReader(htmlText))
                    {
                        XMLWorkerHelper worker = XMLWorkerHelper.GetInstance();
                        worker.ParseXHtml(pdfWriter, pdfDocument, htmlViewReader);
                    }
                }

                renderedBuffer = new byte[outputMemoryStream.Position];
                outputMemoryStream.Position = 0;
                outputMemoryStream.Read(renderedBuffer, 0, renderedBuffer.Length);
            }

            return renderedBuffer;
        }
        #endregion
    }
}

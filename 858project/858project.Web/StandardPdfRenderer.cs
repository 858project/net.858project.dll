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

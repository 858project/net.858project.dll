using System;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Project858.Web
{
    /// <summary>
    /// Vykresli hlavicku a patu PDF dokumentu
    /// </summary>
    public sealed class PdfHeaderAndFooterPrinter : PdfPageEventHelper
    {
        #region - Properties -
        /// <summary>
        /// Nazov dokumentu
        /// </summary>
        public string Title { get; set; }
        #endregion

        #region - Variables -
        /// <summary>
        /// Content pdf dokumentu
        /// </summary>
        private PdfContentByte m_pdfContent = null;
        /// <summary>
        /// Template strany
        /// </summary>
        private PdfTemplate m_pageNumberTemplate = null;
        /// <summary>
        /// Font na vykreslovanie
        /// </summary>
        private BaseFont m_baseFont = null;
        /// <summary>
        /// Cas a datum vykreslenia dokumentu
        /// </summary>
        private DateTime m_printTime = DateTime.MinValue;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Otvori dokument
        /// </summary>
        /// <param name="writer">Writer na zapis dat</param>
        /// <param name="document">Dokument do ktoreho chceme data vykreslit</param>
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            m_printTime = DateTime.Now;
            m_baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            m_pdfContent = writer.DirectContent;
            m_pageNumberTemplate = m_pdfContent.CreateTemplate(50, 50);
        }
        /// <summary>
        /// Prva strana dokumentu
        /// </summary>
        /// <param name="writer">Writer na zapis dat</param>
        /// <param name="document">Dokument do ktoreho chceme data vykreslit</param>
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            Rectangle pageSize = document.PageSize;

            if (Title != string.Empty)
            {
                m_pdfContent.BeginText();
                m_pdfContent.SetFontAndSize(m_baseFont, 11);
                m_pdfContent.SetRGBColorFill(0, 0, 0);
                m_pdfContent.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
                m_pdfContent.ShowText(Title);
                m_pdfContent.EndText();
            }
        }
        /// <summary>
        /// Posledna strana dokumentu
        /// </summary>
        /// <param name="writer">Writer na zapis dat</param>
        /// <param name="document">Dokument do ktoreho chceme data vykreslit</param>
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            string text = pageN + " - ";
            float len = m_baseFont.GetWidthPoint(text, 8);

            Rectangle pageSize = document.PageSize;
            m_pdfContent = writer.DirectContent;
            m_pdfContent.SetRGBColorFill(100, 100, 100);

            m_pdfContent.BeginText();
            m_pdfContent.SetFontAndSize(m_baseFont, 8);
            m_pdfContent.SetTextMatrix(pageSize.Width / 2, pageSize.GetBottom(30));
            m_pdfContent.ShowText(text);
            m_pdfContent.EndText();

            m_pdfContent.AddTemplate(m_pageNumberTemplate, (pageSize.Width / 2) + len, pageSize.GetBottom(30));

            m_pdfContent.BeginText();
            m_pdfContent.SetFontAndSize(m_baseFont, 8);
            m_pdfContent.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, m_printTime.ToString(), pageSize.GetRight(40), pageSize.GetBottom(30), 0);
            m_pdfContent.EndText();
        }
        /// <summary>
        /// Zatvori dokument
        /// </summary>
        /// <param name="writer">Writer na zapis dat</param>
        /// <param name="document">Dokument do ktoreho chceme data vykreslit</param>
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            m_pageNumberTemplate.BeginText();
            m_pageNumberTemplate.SetFontAndSize(m_baseFont, 8);
            m_pageNumberTemplate.SetTextMatrix(0, 0);
            m_pageNumberTemplate.ShowText(string.Empty + (writer.PageNumber - 1));
            m_pageNumberTemplate.EndText();
        }
        #endregion
    }
}

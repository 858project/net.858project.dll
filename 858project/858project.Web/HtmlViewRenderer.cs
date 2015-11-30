using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Project858.Web
{
    /// <summary>
    /// Renderer na vykreslovanie Html contentu
    /// </summary>
    public sealed class HtmlViewRenderer
    {
        #region - Public Class -
        /// <summary>
        /// Empty view
        /// </summary>
        public class EmptyView : IView
        {
            #region - Public Methods -
            /// <summary>
            /// Vykresli view
            /// </summary>
            /// <param name="viewContext">ViewContext</param>
            /// <param name="writer">TextWriter</param>
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vykresli view do Stringu
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="viewName">Meno view</param>
        /// <param name="viewData">Data ktore umoznia view vykreslit</param>
        /// <returns>View v stringu</returns>
        public String RenderViewToString(Controller controller, string viewName, object viewData)
        {
            var renderedView = new StringBuilder();
            using (var responseWriter = new StringWriter(renderedView))
            {
                var fakeResponse = new HttpResponse(responseWriter);
                var fakeContext = new HttpContext(HttpContext.Current.Request, fakeResponse);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(fakeContext), controller.ControllerContext.RouteData, controller.ControllerContext.Controller);

                var oldContext = HttpContext.Current;
                HttpContext.Current = fakeContext;

                using (var viewPage = new ViewPage())
                {
                    var html = new HtmlHelper(CreateViewContext(responseWriter, fakeControllerContext), viewPage);
                    html.RenderPartial(viewName, viewData);
                    HttpContext.Current = oldContext;
                }
            }
            return renderedView.ToString();
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vytvori view content
        /// </summary>
        /// <param name="responseWriter">Writer na zapis contentu</param>
        /// <param name="fakeControllerContext">Content</param>
        /// <returns>ViewContext</returns>
        private static ViewContext CreateViewContext(TextWriter responseWriter, ControllerContext fakeControllerContext)
        {
            ViewContext content = new ViewContext(fakeControllerContext, new EmptyView(), new ViewDataDictionary(), new TempDataDictionary(), responseWriter);

            return content;
        }
        #endregion
    }
}

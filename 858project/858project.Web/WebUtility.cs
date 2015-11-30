using Project858.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Utility na spravu webu
    /// </summary>
    public sealed class WebUtility
    {
        #region - Public Static Methods -
        /// <summary>
        /// Generates a fully qualified URL to an action method by using
        /// the specified action name, controller name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static String AbsoluteAction(String actionName, String controllerName, Object routeValues = null)
        {
            var context = System.Web.HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    var helper = new UrlHelper(request.RequestContext);
                    return helper.AbsoluteAction(actionName, controllerName, routeValues);
                }
            }
            return null;
        }
        /// <summary>
        /// Vypise aktualny request
        /// </summary>
        public static void TraceCurrentRequest()
        {
            var context = System.Web.HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    request.TraceRequest();
                }
            }
        }
        /// <summary>
        /// Vypise header aktualneho requestu
        /// </summary>
        public static void TraceCurrentRequestHeaders()
        {
            var context = System.Web.HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    request.TraceRequestHeaders();
                }
            }
        }
        /// <summary>
        /// Vrati aktualne id requestu
        /// </summary>
        /// <returns>Reuqest id alebo null</returns>
        public static Nullable<Guid> GetCurrentRequestId()
        {
            var context = System.Web.HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    return request.GetRequestId();
                }
            }
            return null;
        }
        /// <summary>
        /// Vrati aktualnu ip adresu klienta zasialajucehor equest alebo null
        /// </summary>
        /// <returns>ip adresa alebo null</returns>
        public static String GetCurrentUserHostAddress()
        {
            var context = System.Web.HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    return request.UserHostAddress;
                }
            }
            return null;
        }
        /// <summary>
        /// Vrati aktualnu absolutnu cestu referrera
        /// </summary>
        /// <returns>Cesta alebo null</returns>
        public static String GetCurrentReferrerAbsolutePathOrNull()
        {
            var context = System.Web.HttpContext.Current;
            if (context != null)
            {
                var request = context.Request;
                if (request != null)
                {
                    var referrer = request.UrlReferrer;
                    if (referrer != null)
                    {
                        return referrer.AbsolutePath;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Zaloguje spravu
        /// </summary>
        /// <param name="exception">Chyba ktoru chceme zalogovat</param>
        public static void Trace(Exception exception)
        {
            if (exception != null)
            {
                WebUtility.Trace(TraceTypes.Error, exception.ToString());
            }
        }
        /// <summary>
        /// Zaloguje spravu
        /// </summary>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Argumenty pre String.Format</param>
        public static void Trace(String message, params Object[] args)
        {
            WebUtility.Trace(TraceTypes.Verbose, message, args);
        }
        /// <summary>
        /// Zaloguje spravu
        /// </summary>
        /// <param name="type">Typ logovania</param>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Argumenty pre String.Format</param>
        public static void Trace(TraceTypes type, String message, params Object[] args)
        {
            if (!String.IsNullOrWhiteSpace(message))
            {
                TraceLogger.DirectoryPath = HttpContext.Current.Server.MapPath("~/App_Data/Trace/");
                TraceLogger.Trace(DateTime.Now, type, "App", ((args != null && args.Length > 0) ? String.Format(message, args) : message));
            }
        }
        /// <summary>
        /// Vrati MVC html string ktory nahradi url adresy z textu html A tagom
        /// </summary>
        /// <param name="value">String ktory chceme sprcovat</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString GetMvcHtmlStringWithUrl(String value)
        {
            value = Regex.Replace(value, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)", "<a target='_blank' href='$1'>$1</a>");
            return new MvcHtmlString(value);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type)
        {
            return WebUtility.GetJsonResult(type, null, null, null);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <param name="behavior">Specifies whether HTTP GET requests from the client are allowed.</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type, JsonRequestBehavior behavior)
        {
            return WebUtility.GetJsonResult(type, null, null, null, behavior);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <param name="message">Sprava vysvetlujuca odpoved</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type, String message)
        {
            return WebUtility.GetJsonResult(type, message, null, null);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <param name="message">Sprava vysvetlujuca odpoved</param>
        /// <param name="behavior">Specifies whether HTTP GET requests from the client are allowed.</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type, String message, JsonRequestBehavior behavior)
        {
            return WebUtility.GetJsonResult(type, message, null, null, behavior);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <param name="message">Sprava vysvetlujuca odpoved</param>
        /// <param name="redirectUrl">Url na ktoru sa ma redirectnut stranka</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type, String message, String redirectUrl)
        {
            return WebUtility.GetJsonResult(type, message, redirectUrl, null);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <param name="message">Sprava vysvetlujuca odpoved</param>
        /// <param name="redirectUrl">Url na ktoru sa ma redirectnut stranka</param>
        /// <param name="behavior">Specifies whether HTTP GET requests from the client are allowed.</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type, String message, String redirectUrl, JsonRequestBehavior behavior)
        {
            return WebUtility.GetJsonResult(type, message, redirectUrl, null, behavior);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <param name="message">Sprava vysvetlujuca odpoved</param>
        /// <param name="redirectUrl">Url na ktoru sa ma redirectnut stranka</param>
        /// <param name="data">Data ktore sa odosielaju</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type, String message, String redirectUrl, Object data)
        {
            return WebUtility.GetJsonResult(type, message, redirectUrl, data, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// Vrati odpoved v definovanom formate
        /// </summary>
        /// <param name="type">Typ odpovede</param>
        /// <param name="message">Sprava vysvetlujuca odpoved</param>
        /// <param name="redirectUrl">Url na ktoru sa ma redirectnut stranka</param>
        /// <param name="data">Data ktore sa odosielaju</param>
        /// <param name="behavior">JsonRequestBehavior</param>
        /// <returns>Navratova hodnota pre server</returns>
        public static JsonResult GetJsonResult(ResponseTypes type, String message, String redirectUrl, Object data, JsonRequestBehavior behavior)
        {
            if (type == ResponseTypes.RedirectToAction && String.IsNullOrWhiteSpace(redirectUrl))
            {
                throw new Exception("redirectUrl is null or empty!");
            }
            if (String.IsNullOrWhiteSpace(message) && WebConfiguration.OnResponseMessageDelegate != null)
            {
                message = WebConfiguration.OnResponseMessageDelegate(type);
            }
            return WebUtility.Json(new { result = type, message = message, redirectUrl = redirectUrl, data = data }, behavior);
        }
        /// <summary>
        /// Vrati image
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Image alebo null</returns>
        public static Image ConvertBlobToImage(byte[] data)
        {
            ImageConverter ic = new ImageConverter();
            return (Image)ic.ConvertFrom(data);
        }
        /// <summary>
        /// Prekonvertuje obrazok na pole bytov
        /// </summary>
        /// <param name="image">Obrazok ktory chceme konvertovat</param>
        /// <returns>Pole dat</returns>
        public static byte[] ConvertImageToBlob(Image image)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }
        /// <summary>
        /// Vygeneruje strankovanie podla poziadaviek
        /// </summary>
        /// <example>
        /// http://www.thebestdata.com/zoom.htm?menutype=1&auto=3246
        /// </example>
        /// <param name="currentPage">Aktualna strana</param>
        /// <param name="numberOfPages">Pocet stran ktore su dostupne</param>
        /// <returns>Kolekcia cisiel stran</returns>
        public static List<int> GeneratePageNumber(int currentPage, int numberOfPages)
        {
            List<int> indexes = new List<int>();

            if (numberOfPages <= 9)
            {
                for (int i = 1; i < numberOfPages + 1; i++)
                {
                    indexes.Add(i);
                }
            }
            else
            {
                if (currentPage < numberOfPages - 4)
                {
                    int From_Page;
                    int To_Page;
                    if (currentPage > 4)
                    {
                        From_Page = currentPage - 4;
                        To_Page = currentPage + 5;
                    }
                    else
                    {
                        From_Page = 1;
                        To_Page = 9;
                    }
                    if (From_Page > 1)
                    {
                        indexes.Add(1);
                    }
                    if (From_Page - 1 > 1)
                    {
                        indexes.Add(-1);
                    }
                    for (int i = From_Page; i < To_Page; i++)
                    {
                        indexes.Add(i);
                    }
                    if (numberOfPages - To_Page > 1)
                    {
                        indexes.Add(-1);
                    }
                    indexes.Add(numberOfPages);
                }
                else
                {
                    indexes.Add(1);
                    if (currentPage - 5 > 1)
                    {
                        indexes.Add(-1);
                    }
                    for (int i = currentPage - 4; i <= numberOfPages; i++)
                    {
                        indexes.Add(i);
                    }
                }
            }

            return indexes;
        }
        /// <summary>
        /// Vyparsuje ciselnu hodnotu strany
        /// </summary>
        /// <param name="page">String-ova hodnota v ktorej sa ma nachadzat cislo strany</param>
        /// <returns>Vyparsovane cislo strany alebo 0</returns>
        public static int ParsePage(String page)
        {
            int pageValue = 0;
            if (!String.IsNullOrEmpty(page))
            {
                if (!int.TryParse(page, out pageValue))
                {
                    pageValue = 0;
                }
                else
                {
                    pageValue -= 1;
                }
            }
            if (pageValue < 0)
            {
                pageValue = 0;
            }
            return pageValue;
        }
        #endregion

        #region - Private Static Methods -
        /// <summary>
        /// Vytvori json result ako odpoved
        /// </summary>
        /// <param name="data">Data ktore chceme odoslat</param>
        /// <param name="behavior">JsonRequestBehavior</param>
        /// <returns>JsonResult</returns>
        private static JsonResult Json(Object data, JsonRequestBehavior behavior)
        {
            JsonResult result = new JsonResult();
            result.Data = data;
            result.ContentType = null;
            result.ContentEncoding = null;
            result.JsonRequestBehavior = behavior;
            return result;
        }
        #endregion
    }
}

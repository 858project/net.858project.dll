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

using Project858.ComponentModel.Client;
using Project858.Data.SqlClient;
using Project858.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Windows.Threading;

namespace Project858.Web
{
    /// <summary>
    /// Web Applikacia a jej zakladne funkcie
    /// </summary>
    public abstract class WebApplication : HttpApplication, IUserBaseHandler
    {
        #region - Constants -
        /// <summary>
        /// Cookie name v ktorej si uchovavame id uzivatela
        /// </summary>
        public const String ANALYTICS_COOKIE_NAME = "page";
        #endregion
 
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public WebApplication()
        {
            UserBase.Handler = this;
            this.BeginRequest += WebApplication_BeginRequest;
        }
        #endregion

        #region - Public Static Properties -
        /// <summary>
        /// Token key na parsovanie hash hodnot
        /// </summary>
        public static String AccessTokenKey { get; set; }
        /// <summary>
        /// Cache na ukladanie dat
        /// </summary>
        public static Cache Cache
        {
            get
            {
                if (WebApplication.m_cache == null)
                {
                    WebApplication.m_cache = new Cache()
                    {
                        Timeout = new TimeSpan(0, 5, 0)
                    };
                }
                return WebApplication.m_cache;
            }
        }
        #endregion

        #region - Private Static Variable -
        /// <summary>
        /// Cache
        /// </summary>
        private static Cache m_cache = null;
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// Zaloguje pozadovanu spravu do suboru
        /// </summary>
        /// <param name="type">Typ logovania</param>
        /// <param name="message">Sprava ktoru chceme zalogovat</param>
        /// <param name="args">Argumenty pre String.Format(message, args)</param>
        public static void OnTrace(TraceTypes type, String message, params Object[] args)
        {
            WebUtility.Trace(type, message, args);
        }
        /// <summary>
        /// Zaloguje vzniknutu chybu do suboru
        /// </summary>
        /// <param name="exception">Chyba ktora vznikla</param>
        public static void OnException(Exception exception)
        {
            WebUtility.Trace(exception);
            WebUtility.TraceCurrentRequest();
        }
        /// <summary>
        /// Aktualizuje hodnotu
        /// </summary>
        /// <param name="key">Guid ktory chceme aktualizovat</param>
        /// <param name="date">Datum ktory chceme ulozit</param>
        public static void UpdateValue(Guid key, DateTimeOffset date)
        {
            if (String.IsNullOrWhiteSpace(WebApplication.AccessTokenKey))
            {
                return;
            }
            String value = key.ToString().Replace("-", "");
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                HttpRequest request = current.Request;
                if (request != null)
                {
                    HttpCookie cookie = request.Cookies.Get(value);
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(value);
                    }
                    value = Utility.EncodeValue(WebApplication.AccessTokenKey, date.ToIso8601String());
                    cookie.Value = value;
                    current.Response.SetCookie(cookie);
                }
            }
        }
        /// <summary>
        /// Overi platnost hodnoty v zavislosti od datumu
        /// </summary>
        /// <param name="key">Guid ktory cheme overit</param>
        /// <param name="date">Datum ktory chceme overit</param>
        /// <returns>True = zaznam je ok, inak false</returns>
        public static Boolean ValidateValue(Guid key, DateTimeOffset date)
        {
            if (String.IsNullOrWhiteSpace(WebApplication.AccessTokenKey))
            {
                return false;
            }
            String value = key.ToString().Replace("-", "");
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                HttpRequest request = current.Request;
                if (request != null)
                {
                    HttpCookie cookie = request.Cookies.Get(value);
                    if (cookie != null)
                    {
                        try
                        {
                            value = Utility.DecodeValue(WebApplication.AccessTokenKey, cookie.Value);
                            DateTimeOffset currentDate = DateTimeOffset.MinValue;
                            if (DateTimeOffset.TryParse(value, null, DateTimeStyles.AdjustToUniversal, out currentDate))
                            {
                                return DateTimeOffset.Compare(currentDate, date) <= 0;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Odosle emailovu spravu cez definovany smtp server
        /// </summary>
        public static Boolean SendMailMessage(String content, String subject, String copyAddress, String senderAddress, String replyAddress, List<String> addressCollection)
        {
            //initialize
            MailMessage message = new MailMessage();
            senderAddress = Utility.ValidateMailAddress(senderAddress) ? senderAddress : WebConfigurationManager.AppSettings["SmtpSenderAddress"];
            message.From = new MailAddress(senderAddress, WebConfigurationManager.AppSettings["SmtpSenderName"]);
            message.Subject = subject;
            foreach (var address in addressCollection)
            {
                message.To.Add(new MailAddress(address));
            }

            //copy address
            if (Utility.ValidateMailAddress(copyAddress))
            {
                message.Bcc.Add(copyAddress);
            }

            //copy address
            if (Utility.ValidateMailAddress(replyAddress))
            {
                message.ReplyToList.Add(replyAddress);
            }

            //vytvorime plain view
            AlternateView plainView = AlternateView.CreateAlternateViewFromString(content, Encoding.UTF8, MediaTypeNames.Text.Html);
            message.AlternateViews.Add(plainView);

            //odoslame spravu
            return WebApplication.SendMailMessage(message);
        }
        /// <summary>
        /// SendMailMessage
        /// </summary>
        public static Boolean SendMailMessage(MailMessage message)
        {
            //ziskame konfiguraciu smtp servera
            String smtpServer = WebConfigurationManager.AppSettings["SmtpServer"];
            int smtpPort = int.Parse(WebConfigurationManager.AppSettings["SmtpPort"]);
            String smtpLogin = WebConfigurationManager.AppSettings["SmtpLogin"];
            String smtpPassword = WebConfigurationManager.AppSettings["SmtpPassword"];
            Boolean smtpSslEnable = Boolean.Parse(WebConfigurationManager.AppSettings["SmtpSslEnable"]);

            //odosleme spravu
            return WebApplication.SendMailMessage(message, smtpServer, smtpPort, smtpLogin, smtpPassword, smtpSslEnable);
        }
        /// <summary>
        /// SendMailMessage
        /// </summary>
        public static Boolean SendMailMessage(MailMessage message, String smtpServer, int smtpPort, String smtpLogin, String smtpPassword, Boolean smtpSslEnable)
        {
            try
            {
                //zalogujeme
                Project858.Web.WebUtility.Trace("Odosielanie e-mailu: '{0}'", String.Join(",", message.To));

                SmtpClient client = new SmtpClient(smtpServer, smtpPort);
                client.Credentials = new NetworkCredential(smtpLogin, smtpPassword);
                client.EnableSsl = smtpSslEnable;
                client.Send(message);

                //zalogujeme
                Project858.Web.WebUtility.Trace("Odoslanie e-mailu bolo uspesne");

                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                Project858.Web.WebUtility.Trace("Odoslanie e-mailu zlyhalo. {0}", ex);

                WebApplication.OnException(ex);
                return false;
            }
        }
        #endregion

        #region - User Base Methods -
        /// <summary>
        /// Overi spravnost tokenu a to ci je uzivatel stale prihlaseny
        /// </summary>
        /// <param name="token">Token uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        public Boolean UserBaseValidateToken(String token, String hostAddress)
        {
            if (String.IsNullOrWhiteSpace(WebApplication.AccessTokenKey))
            {
                return false;
            }
            try
            {
                if (!String.IsNullOrWhiteSpace(token))
                {
                    token = Utility.DecodeValue(WebApplication.AccessTokenKey, token);
                    if (!String.IsNullOrWhiteSpace(token))
                    {
                        String[] values = token.Split(new char[] { '/' });
                        if (values.Length == 3)
                        {
                            if (values[1].CompareTo(hostAddress) == 0)
                            {
                                DateTimeOffset date = DateTimeOffset.MinValue;
                                if (DateTimeOffset.TryParse(values[2], null, DateTimeStyles.AssumeUniversal, out date)) 
                                {
                                    return DateTimeOffset.Compare(date.ToUniversalTime(), DateTimeOffset.UtcNow) >= 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WebApplication.OnException(ex);
            }
            return false;
        }
        /// <summary>
        /// Vrati id uzivatela z tokenu
        /// </summary>
        /// <param name="token">Token uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        public Nullable<Guid> UserBaseGetUserLoginIdFromToken(String token, String hostAddress)
        {
            if (String.IsNullOrWhiteSpace(WebApplication.AccessTokenKey))
            {
                return null;
            }
            try
            {
                if (!String.IsNullOrWhiteSpace(token))
                {
                    token = Utility.DecodeValue(WebApplication.AccessTokenKey, token);
                    if (!String.IsNullOrWhiteSpace(token))
                    {
                        String[] values = token.Split(new char[] { '/' });
                        if (values.Length == 3)
                        {
                            if (values[1].CompareTo(hostAddress) == 0)
                            {
                                return values[0].ToGuid();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WebApplication.OnException(ex);
            }
            return null;
        }
        /// <summary>
        /// Vygeneruje token obsahujuci dolezite data na validaciu uzivatela
        /// </summary>
        /// <param name="userId">Id uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        public String UserBaseGetUserToken(Guid userId, String hostAddress)
        {
            if (String.IsNullOrWhiteSpace(WebApplication.AccessTokenKey))
            {
                return null;
            }
            String token = String.Format("{0}/{1}/{2}", userId, hostAddress, DateTime.UtcNow.AddHours(24).ToIso8601String());
            token = Utility.EncodeValue(WebApplication.AccessTokenKey, token);
            return token;
        }
        /// <summary>
        /// Vrati host adress v pripade specialnych pripadov
        /// </summary>
        /// <returns>Host address alebo null</returns>
        public abstract String UserBaseGetHostAddress();
        /// <summary>
        /// Vrati uzivatela na zaklade id prihlasenia a IP adresy z ktorej uzivatel prichadza
        /// </summary>
        /// <param name="id">Id prihlasenia uzivatela</param>
        /// <param name="hostAddress">IP adresa z ktorej uzivatel prichadza</param>
        /// <returns>User alebo null</returns>
        public abstract UserBase UserBaseGetUserFromUserLoginId(Guid id, String hostAddress);
        /// <summary>
        /// Vykona odhlasenie uzivatela na zaklade Id prihlasenia
        /// </summary>
        /// <param name="id">Id prihlasenia uzivatela</param>
        public abstract void UserBaseUserLogOff(Guid id);
        /// <summary>
        /// Vykona prihlasenie uzivatela
        /// </summary>
        /// <param name="name">Meno uzivatela</param>
        /// <param name="password">Heslo uzivatela</param>
        /// <returns>Uzivatel ktory bol prihlaseny alebo null</returns>
        public abstract UserBase Login(String name, String password);
        #endregion

        #region - Protected Methods -
        /// <summary>
        /// Inicializuje a vrati sql klienta na pristup k databaze
        /// </summary>
        /// <returns>Sql databaza</returns>
        protected abstract SqlClient GetSourceClient();
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Spracuje prichadzajuci request a nastavi aktualny jazyk
        /// </summary>
        /// <param name="sender">Odosielatel udalosti</param>
        /// <param name="e">EventArgs</param>
        private void WebApplication_BeginRequest(object sender, EventArgs e)
        {
            Nullable<LanguageTypes> language = null;
            var request = this.Request;
            if (request != null)
            {
                //pridame do requestu id
                request.CreateRequestId();

                //ziskame aktualny jazyk,
                //ak sa v parametroch stranky nachadza 'language' tak nastavime jazyk a redirecneme stranku na rovnaku url bez parametra 'language'
                String languageValue = request.QueryString["language"];
                if (!String.IsNullOrWhiteSpace(languageValue))
                {
                    language = UserBase.GetLanguage(languageValue);
                    if (languageValue != null)
                    {
                        UserBase.CurrentLanguage = language.Value;
                    }
                    var queryString = new NameValueCollection(request.QueryString);
                    if (queryString != null) 
                    {
                        queryString.Remove("language");
                    }
                    StringBuilder newPath = new StringBuilder();
                    newPath.Append(request.Path + (queryString.Count > 0 ? "?" : String.Empty));
                    for (var i = 0; i < queryString.Count; i++)
                    {
                        if (i > 0)
                            newPath.Append("&");
                        newPath.Append(String.Format("{0}={1}", queryString.GetKey(i), queryString[i]));
                    }
                    this.Response.Redirect(newPath.ToString(), true);
                    base.CompleteRequest();
                }
            }
            language = UserBase.CurrentLanguage;
            CultureInfo culture = this.InternalGetCurrentCulture(language.Value);
            if (culture != null)
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }
        /// <summary>
        /// Vrati aktualnu kulturu requestu
        /// </summary>
        /// <param name="language">Jazyk ktoreho kulturu chceme ziskat</param>
        /// <returns>Kultura alebo null</returns>
        private CultureInfo InternalGetCurrentCulture(LanguageTypes language)
        {
            FieldInfo fieldInfo = language.GetType().GetField(language.ToString());
            LanguageAttribute[] attributes = (LanguageAttribute[])fieldInfo.GetCustomAttributes(typeof(LanguageAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                foreach (LanguageAttribute attribute in attributes)
                {
                    if (!String.IsNullOrWhiteSpace(attribute.Culture))
                    {
                        return new CultureInfo(attribute.Culture);
                    }
                }
            }
            return null;
        }
        #endregion

        #region - Destructor -
        /// <summary>
        /// Deinitialize this class
        /// </summary>
        ~WebApplication()
        {
            WebUtility.Trace("deinitialize WebApplication");
        }
        #endregion
    }
}

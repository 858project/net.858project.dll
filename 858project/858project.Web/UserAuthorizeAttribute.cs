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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Project858.Web;

namespace Project858.Web
{
    /// <summary>
    /// Atribut na automaticky redirect v pripade pristupu k metoda ktora si vyzaduje alebo nevyzaduje prihlasenie
    /// </summary>
    public sealed class UserAuthorizeAttribute : ActionFilterAttribute
    {
        #region - Properties -
        /// <summary>
        /// Typ uzivatela ktory sa ocakava
        /// </summary>
        public Type UserType { get; set; }
        /// <summary>
        /// Definuje ci ide o Base autentifikaciu
        /// </summary>
        public Boolean IsBasicAuthenticate
        {
            get;
            set;
        }
        /// <summary>
        /// Definuje ci je prihlasenie uzivatela pozadovane
        /// </summary>
        public Boolean Required
        {
            get;
            set;
        }
        /// <summary>
        /// Definuje url na ktory sa ma redirect vykonat
        /// </summary>
        public String RedirectUrl
        {
            get;
            set;
        }
        /// <summary>
        /// Definuje ci je uzivatel prihlaseny alebo nie
        /// </summary>
        public Boolean IsAuthenticated
        {
            get {
                var user = UserBase.CurrentUser;
                if (user != null)
                {
                    if (this.UserType != null)
                    {
                        return user.GetType() == this.UserType;
                    }
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Definuje ci je aktualny uzivatel blokovany
        /// </summary>
        public Nullable<Boolean> IsBlocked
        {
            get { return UserBase.CurrentUser == null ? null : (Nullable<Boolean>)UserBase.CurrentUser.Blocked; }
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Spracuje udalost
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //base authenticate
            if (this.IsBasicAuthenticate)
            {
                if (!this.IsAuthenticated && this.Required)
                {
                    //authenticate
                    if (!this.InternalBasicAuthenticate(filterContext))
                    {
                        filterContext.Result = new HttpBasicUnauthorizedResult();
                    }
                }
            }
            else
            {
                //uzivatel nie je prihlaseny pricom prihlasenie je vyzadovane
                if (!this.IsAuthenticated && this.Required)
                {
                    if (this.InternalIsPostMethod(filterContext))
                    {
                        filterContext.Result = WebUtility.GetJsonResult(ResponseTypes.UserIsNotLogged);
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(this.RedirectUrl))
                        {
                            filterContext.Result = new RedirectResult(this.RedirectUrl);
                        }
                        else
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                        }
                    }
                }
                //uzivatel je prihlaseny ale je blokovany
                else if (this.IsAuthenticated && this.IsBlocked.HasValue && this.IsBlocked.Value)
                {
                    if (this.InternalIsPostMethod(filterContext))
                    {
                        filterContext.Result = WebUtility.GetJsonResult(ResponseTypes.UserIsBlocked);
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(this.RedirectUrl))
                        {
                            filterContext.Result = new RedirectResult(this.RedirectUrl);
                        }
                        else
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                        }
                    }
                }
                //uzivatel je prihlaseny ale nieje blokovany a zaroven je zadana url na redirect
                else if (this.IsAuthenticated && this.IsBlocked.HasValue && !this.Required && !this.IsBlocked.Value && !String.IsNullOrWhiteSpace(this.RedirectUrl))
                {
                    if (this.InternalIsPostMethod(filterContext))
                    {
                        filterContext.Result = WebUtility.GetJsonResult(ResponseTypes.RedirectToAction, null, this.RedirectUrl);
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(this.RedirectUrl);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vykona base autorizaciu
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext</param>
        /// <returns></returns>
        private Boolean InternalBasicAuthenticate(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            //http context
            var context = filterContext.HttpContext;
            if (context == null)
                throw new ArgumentNullException("context");

            //get authorization data
            var authorization = context.Request.GetHeaderItem("Authorization");
            if (!String.IsNullOrWhiteSpace(authorization))
            {
                //odstranime Base hodnotu
                authorization = authorization.Substring(6);
                string[] data = Encoding.ASCII.GetString(Convert.FromBase64String(authorization)).Split(new char[] { ':' });
                if (data.Length == 2)
                {
                    //autorization
                    return this.InternalBasicAuthenticate(data[0], data[1]);
                }
            }
            //error
            return false;
        }
        /// <summary>
        /// Vykona prihlasenie uzivatela
        /// </summary>
        /// <param name="name">Meno uzivatela</param>
        /// <param name="paasowrd">Heslo uzivatela</param>
        /// <returns>True = prihlasenie bolo uspesne, inak false</returns>
        private Boolean InternalBasicAuthenticate(String name, String paasowrd)
        {
            UserBase user = UserBase.Login(name, paasowrd);
            if (user != null)
            {
                //return user.LoginUser(user.Id, DateTime.Now.AddMinutes(10));
                UserBase.CurrentUser = user;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Overi ci ide o post metodu
        /// </summary>
        /// <param name="filterContext">ActionExecutingContext</param>
        /// <returns>True = post metoda, false inak nie...</returns>
        private Boolean InternalIsPostMethod(ActionExecutingContext filterContext)
        {
            return (filterContext.HttpContext.Request.HttpMethod != null && filterContext.HttpContext.Request.HttpMethod.ToLower().IndexOf("post") > -1);
        }
        #endregion
    }
}

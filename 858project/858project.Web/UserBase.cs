using Project858.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Project858.Web
{
    /// <summary>
    /// Web uzivatel s implementaciou cookie
    /// </summary>
    public abstract class UserBase : ModelBase
    {
        #region - Constants -
        /// <summary>
        /// Konstanta popisujuca nazov cookie hodnoty na ukladanie aktualneho jazyka
        /// </summary>
        private const String LANGUAGE_COOKIE_NAME = "culture";
        /// <summary>
        /// Meno cookie pre ulozenie informacie o prihlaseni
        /// </summary>
        private const String USER_COOKIE_NAME = "UserSessionId";
        #endregion

        #region - Public Static Properties -
        /// <summary>
        /// Nastavi alebo vrati aktualne nastaveny jazyk
        /// </summary>
        public static LanguageTypes CurrentLanguage
        {
            get { return UserBase.InternalGetCurrentLanguage(); }
            set { UserBase.InternalSetCurrentLanguage(value); }
        }
        /// <summary>
        /// Handler na spracovanie udalosti
        /// </summary>
        public static IUserBaseHandler Handler
        {
            get;
            set;
        }
        /// <summary>
        /// Aktualne prihlaseny uzivatel alebo null
        /// </summary>
        public static UserBase CurrentUser
        {
            get { return UserBase.InternalGetCurrentUser(); }
            set { UserBase.InternalSetCurrentUser(value); }
        }
        /// <summary>
        /// Cookie name pre uzivatela
        /// </summary>
        public static String UserCookieName
        {
            get { return String.IsNullOrWhiteSpace(UserBase.m_userCookieName) ? UserBase.USER_COOKIE_NAME : UserBase.m_userCookieName; }
            set { UserBase.m_userCookieName = value; }
        }
        /// <summary>
        /// Cookie name pre uzivatela
        /// </summary>
        public static String LanguageCookieName
        {
            get { return String.IsNullOrWhiteSpace(UserBase.m_languageCookieName) ? UserBase.LANGUAGE_COOKIE_NAME : UserBase.m_languageCookieName; }
            set { UserBase.m_languageCookieName = value; }
        }
        /// <summary>
        /// Cookie name na ziskanie host address
        /// </summary>
        public static String HostAddressCookieName
        {
            get { return UserBase.m_hostAddressCookieName; }
            set { UserBase.m_hostAddressCookieName = value; }
        }
        #endregion

        #region - Public Properties -
        /// <summary>
        /// Definuje ci je aktualny uzivatel blokovany alebo nie
        /// </summary>
        [ScriptIgnore]
        public Boolean Blocked
        {
            get { return false; }
            set { throw new NotSupportedException(); }
        }
        /// <summary>
        /// Jedinecne Id uzivatela
        /// </summary>
        [ScriptIgnore]
        public Guid Id
        {
            get;
            set;
        }
        #endregion

        #region - Private Static Vaiables -
        /// <summary>
        /// Cookie name na ziskanie host address
        /// </summary>
        private static String m_hostAddressCookieName = String.Empty;
        /// <summary>
        /// Cookie name pre uzivatela
        /// </summary>
        private static String m_userCookieName = String.Empty;
        /// <summary>
        /// Cookie name pre jazyk
        /// </summary>
        private static String m_languageCookieName = String.Empty;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vykona prihlasenie uzivatela s pozadovanym Id
        /// </summary>
        /// <param name="id">Id uzivatela ktoreho chceme prihlasit</param>
        /// <param name="expireDate">Datum expiracie prihlasenia</param>
        /// <returns>True = prihlasenie bolo uspesne, inak false</returns>
        public Boolean LoginUser(Guid id, DateTime expireDate)
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                //ziskame token uzivatela
                String token = this.InternalGetUserToken(id, UserBase.GetHostAddress());
                if (!String.IsNullOrWhiteSpace(token))
                {
                    //vratime uzivatelovi cookie
                    current.Response.SetCookie(new HttpCookie(UserBase.UserCookieName, token)
                    {
                        Expires = expireDate
                    });
                    //uchovame si aktualneho uzivatela
                    UserBase.CurrentUser = this;

                    //prihlasenie bolo uspesne
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vygeneruje token obsahujuci dolezite data na validaciu uzivatela
        /// </summary>
        /// <param name="userId">Id uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        private String InternalGetUserToken(Guid userId, String hostAddress)
        {
            if (UserBase.Handler != null)
            {
                return UserBase.Handler.UserBaseGetUserToken(userId, hostAddress);
            }
            return null;
        }
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// Vykona prihlasenie uzivatela
        /// </summary>
        /// <param name="name">Meno uzivatela</param>
        /// <param name="password">Heslo uzivatela</param>
        /// <returns>Prihlaseny uzivatel alebo null</returns>
        public static UserBase Login(String name, String password)
        {
            return UserBase.InternalLogin(name, password);
        }
        /// <summary>
        /// Vrati host adress pre aktualneho uzivatela
        /// </summary>
        /// <returns>Host address</returns>
        public static String GetHostAddress()
        {
            return UserBase.InternalGetHostAddress();
        }
        /// <summary>
        /// Odhlasi aktualneho uzivatela
        /// </summary>
        public static UserBase CurrentUserLogOff()
        {
            UserBase user = UserBase.InternalGetCurrentUser();
            if (user != null)
            {
                HttpContext current = HttpContext.Current;
                if (current != null)
                {
                    HttpRequest request = current.Request;
                    if (request != null)
                    {
                        HttpCookie cookie = request.Cookies.Get(UserBase.UserCookieName);
                        if (cookie != null)
                        {
                            String token = cookie.Value;
                            if (!String.IsNullOrWhiteSpace(token))
                            {
                                if (UserBase.InternalUserBaseValidateToken(token, UserBase.InternalGetHostAddress()))
                                {
                                    Nullable<Guid> userId = UserBase.InternalUserBaseGetUserLoginIdFromToken(token, UserBase.InternalGetHostAddress());
                                    if (userId != null)
                                    {
                                        UserBase.InternalUserBaseUserLogOff(userId.Value);
                                    }
                                }
                                cookie.Value = String.Empty;
                                cookie.Expires = DateTime.Now;
                                current.Response.SetCookie(cookie);
                            }
                        }
                    }
                }
            }
            return user;
        }
        /// <summary>
        /// Vrati aktualne nastaveny jazyk pre prichadzajuci request
        /// </summary>
        /// <param name="language">Jazyk z hlavicky requestu</param>
        /// <returns>Aktualne nastaveny jazyk, alebo default</returns>
        public static Nullable<LanguageTypes> GetLanguage(String language)
        {
            foreach (LanguageTypes item in Enum.GetValues(typeof(LanguageTypes)))
            {
                FieldInfo fieldInfo = item.GetType().GetField(item.ToString());
                LanguageAttribute[] attributes = (LanguageAttribute[])fieldInfo.GetCustomAttributes(typeof(LanguageAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    foreach (LanguageAttribute attribute in attributes)
                    {
                        if (String.Compare(attribute.Culture, language, true) == 0 || String.Compare(attribute.IsoCode, language) == 0)
                        {
                            UserBase.InternalSetCurrentLanguage(item);
                            return item;
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region - Private Static Methods -
        /// <summary>
        /// Vrati aktualne nastaveny jazyk pre prichadzajuci request
        /// </summary>
        /// <returns>Aktualne nastaveny jazyk, alebo default</returns>
        private static LanguageTypes InternalGetCurrentLanguage()
        {
            HttpContext current = HttpContext.Current;
            var languageItem = current.Items[UserBase.LanguageCookieName];
            if (languageItem == null || languageItem.GetType() != typeof(LanguageTypes))
            {
                Nullable<LanguageTypes> language = null;
                if (current != null)
                {
                    HttpRequest request = current.Request;
                    if (request != null)
                    {
                        HttpCookie cookie = request.Cookies.Get(UserBase.LanguageCookieName);
                        if (cookie != null)
                        {
                            language = UserBase.GetLanguage(cookie.Value);
                        }
                        else if (request.UserLanguages != null && request.UserLanguages.Length > 0)
                        {
                            for (int i = 0; i < request.UserLanguages.Length; i++)
                            {
                                language = UserBase.GetLanguage(request.UserLanguages[i]);
                                if (language != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (language == null)
                {
                    language = UserBase.InternalGetDefaultLanguage();
                }
                return language.Value;
            }
            return (LanguageTypes)languageItem;
        }
        /// <summary>
        /// Nastavi definovany jazyk do cookie uzivatela
        /// </summary>
        /// <param name="language">Jazyk ktory chceme nastavit</param>
        private static void InternalSetCurrentLanguage(LanguageTypes language)
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                HttpRequest request = current.Request;
                if (request != null)
                {
                    HttpCookie cookie = request.Cookies.Get(UserBase.LanguageCookieName);
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(UserBase.LanguageCookieName);
                    }
                    cookie.Value = UserBase.GetCultureFromValue(language);
                    current.Response.SetCookie(cookie);
                    if (current.Items.Contains(UserBase.LanguageCookieName))
                    {
                        current.Items[UserBase.LanguageCookieName] = language;
                    }
                    else
                    {
                        current.Items.Add(UserBase.LanguageCookieName, language);
                    }
                }
            }
        }
        /// <summary>
        /// Nastavi a vrati default hodnotu jazyka
        /// </summary>
        /// <returns>Default jazyk</returns>
        private static LanguageTypes InternalGetDefaultLanguage()
        {
            LanguageTypes language = LanguageTypes.EN_US;
            UserBase.InternalSetCurrentLanguage(language);
            return language;
        }
        /// <summary>
        /// Ulozi aktualneho uzivatela do CurrentUser
        /// </summary>
        /// <param name="user">Uzivatel ktoreho chceme ulozit</param>
        private static void InternalSetCurrentUser(UserBase user)
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                if (current.Items.Contains(UserBase.UserCookieName))
                {
                    //update
                    current.Items[UserBase.UserCookieName] = user;
                }
                else
                {
                    //insert
                    current.Items.Add(UserBase.UserCookieName, user);
                }
            }
        }
        /// <summary>
        /// Overi ci je aktualny uzivatel prihlaseny
        /// </summary>
        /// <returns>Prihlaseny uzivatel alebo null</returns>
        private static UserBase InternalGetCurrentUser()
        {
            HttpContext current = HttpContext.Current;
            UserBase user = current.Items[UserBase.UserCookieName] as UserBase;
            if (user == null)
            {
                if (current != null)
                {
                    HttpRequest request = current.Request;
                    if (request != null)
                    {
                        HttpCookie cookie = request.Cookies.Get(UserBase.UserCookieName);
                        if (cookie != null)
                        {
                            String token = cookie.Value;
                            if (!String.IsNullOrWhiteSpace(token))
                            {
                                if (UserBase.InternalUserBaseValidateToken(token, UserBase.InternalGetHostAddress())) 
                                {
                                    Nullable<Guid> userId = UserBase.InternalUserBaseGetUserLoginIdFromToken(token, UserBase.InternalGetHostAddress());
                                    if (userId != null) 
                                    {
                                        user = UserBase.InternalGetUserFromUserLoginId(userId.Value, UserBase.InternalGetHostAddress());
                                        if (user == null)
                                        {
                                            if (cookie != null)
                                            {
                                                cookie.Value = String.Empty;
                                                cookie.Expires = DateTime.Now;
                                                current.Response.SetCookie(cookie);
                                            }
                                        }
                                        else
                                        {
                                            UserBase.InternalSetCurrentUser(user);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return user;
        }
        /// <summary>
        /// Vrati id uzivatela z tokenu
        /// </summary>
        /// <param name="token">Token uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        private static Nullable<Guid> InternalUserBaseGetUserLoginIdFromToken(String token, String hostAddress)
        {
            if (UserBase.Handler != null)
            {
                return UserBase.Handler.UserBaseGetUserLoginIdFromToken(token, hostAddress);
            }
            return null;
        }
        /// <summary>
        /// Overi spravnost tokenu a to ci je uzivatel stale prihlaseny
        /// </summary>
        /// <param name="token">Token uzivatela</param>
        /// <param name="hostAddress">IP adresa uzivatela odkial sa prihlasil</param>
        /// <returns>Token uzivatela</returns>
        private static Boolean InternalUserBaseValidateToken(String token, String hostAddress) 
        {
            if (UserBase.Handler != null)
            {
                return UserBase.Handler.UserBaseValidateToken(token, hostAddress);
            }
            return false;
        }
        /// <summary>
        /// Vrati uzivatela na zaklade id prihlasenia a IP adresy z ktorej uzivatel prichadza
        /// </summary>
        /// <param name="id">Id prihlasenia</param>
        /// <param name="hostAddress">IP adresa z ktorej uzivatel prichadza</param>
        /// <returns>User alebo null</returns>
        private static UserBase InternalGetUserFromUserLoginId(Guid id, String hostAddress)
        {
            if (UserBase.Handler != null)
            {
                return UserBase.Handler.UserBaseGetUserFromUserLoginId(id, hostAddress);
            }
            return null;
        }
        /// <summary>
        /// Vykona odhlasenie uzivatela na zaklade Id prihlasenia
        /// </summary>
        /// <param name="id">Id prihlasenia uzivatela</param>
        private static void InternalUserBaseUserLogOff(Guid id)
        {
            if (UserBase.Handler != null)
            {
                UserBase.Handler.UserBaseUserLogOff(id);
            }
        }
        /// <summary>
        /// Vykona prihlasenie uzivatela
        /// </summary>
        /// <param name="name">Meno uzivatela</param>
        /// <param name="password">Heslo uzivatela</param>
        /// <returns>Prihlaseny uzivatel alebo null</returns>
        private static UserBase InternalLogin(String name, String password)
        {
            if (UserBase.Handler != null)
            {
                return UserBase.Handler.Login(name, password);
            }
            return null;
        }
        /// <summary>
        /// Vrati host adress pre aktualneho uzivatela
        /// </summary>
        /// <returns>Host address</returns>
        private static String InternalGetHostAddress()
        {
            //ziskame adresu
            if (UserBase.Handler != null)
            {
                String hostAddress = UserBase.Handler.UserBaseGetHostAddress();
                if (!String.IsNullOrWhiteSpace(hostAddress))
                {
                    return hostAddress;
                }
            }

            //ziskame host address
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                HttpRequest request = current.Request;
                if (request != null)
                {
                    if (!String.IsNullOrWhiteSpace(UserBase.HostAddressCookieName))
                    {
                        HttpCookie cookie = request.Cookies.Get(UserBase.HostAddressCookieName);
                        if (cookie != null)
                        {
                            String value = cookie.Value;
                            if (!String.IsNullOrWhiteSpace(value))
                            {
                                return value;
                            }
                        }
                    }
                    return request.UserHostAddress;
                }
            }

            //adresa sa nenasla
            return "unknown address";
        }
        /// <summary>
        /// Vrati nazov jazyka
        /// </summary>
        /// <param name="type">LanguageTypes</param>
        /// <returns>Meno alebo null</returns>
        public static String GetNameFromValue(LanguageTypes type)
        {
            FieldInfo fieldInfo = type.GetType().GetField(type.ToString());
            LanguageAttribute[] attributes = (LanguageAttribute[])fieldInfo.GetCustomAttributes(typeof(LanguageAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                foreach (LanguageAttribute attribute in attributes)
                {
                    return attribute.Name;
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// Vrati culturu pre pozadovany typ jazyka
        /// </summary>
        /// <param name="type">LanguageTypes</param>
        /// <returns>Cultura alebo null</returns>
        public static String GetCultureFromValue(LanguageTypes type)
        {
            FieldInfo fieldInfo = type.GetType().GetField(type.ToString());
            LanguageAttribute[] attributes = (LanguageAttribute[])fieldInfo.GetCustomAttributes(typeof(LanguageAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                foreach (LanguageAttribute attribute in attributes)
                {
                    return attribute.Culture;
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// Vrati iso kod pre pozadovany typ jazyka
        /// </summary>
        /// <param name="type">LanguageTypes</param>
        /// <returns>Iso code alebo null</returns>
        public static String GetIsoCodeFromValue(LanguageTypes type)
        {
            FieldInfo fieldInfo = type.GetType().GetField(type.ToString());
            LanguageAttribute[] attributes = (LanguageAttribute[])fieldInfo.GetCustomAttributes(typeof(LanguageAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                foreach (LanguageAttribute attribute in attributes)
                {
                    return attribute.IsoCode;
                }
            }
            return String.Empty;
        }
        #endregion
    }
}

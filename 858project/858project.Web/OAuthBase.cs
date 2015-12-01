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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Project858;

namespace Project858.Web
{
    /// <summary>
    /// OAuthBase
    /// </summary>
    public abstract class OAuthBase
    {
        #region - Public Enum -
        /// <summary>
        /// AuthType
        /// </summary>
        public enum AuthType : byte
        {
            Facebook = 0
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// GetRedirectUrl
        /// </summary>
        public abstract String GetRedirectUrl(String key, String url);
        /// <summary>
        /// GetProfile
        /// </summary>
        public abstract OAuthUserProfile GetProfile(String key, String secret, String code, String url);
        #endregion

        #region - Public Statics Methods -
        /// <summary>
        /// CreateAuth
        /// </summary>
        public static OAuthBase CreateAuth(AuthType type)
        {
            switch (type)
            {
                case AuthType.Facebook:
                    return new OAuthFacebook();
                default:
                    return null;
            }
        }
        #endregion

        #region - Protected Methods -
        /// <summary>
        /// InternalParseUserProfile
        /// </summary>
        protected abstract OAuthUserProfile InternalParseUserProfile(String response);
        /// <summary>
        /// InternalHttpRequest
        /// </summary>
        protected String InternalHttpRequest(String url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    }
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.Trace(ex);
                return null;
            }
        }
        #endregion
    }
    /// <summary>
    /// OAuthFacebook
    /// </summary>
    public sealed class OAuthFacebook : OAuthBase
    {
        #region - Public Methods -
        /// <summary>
        /// GetRedirectUrl
        /// </summary>
        public override String GetRedirectUrl(String key, String url)
        {
            return String.Format("https://graph.facebook.com/oauth/authorize?type=web_server&client_id={0}&redirect_uri={1}&scope=email,user_birthday,user_hometown", key, HttpUtility.UrlEncode(url));
        }
        /// <summary>
        /// GetProfile
        /// </summary>
        public override OAuthUserProfile GetProfile(String key, String secret, String code, String url)
        {
            //get token
            url = String.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}", key, url, secret, code);
            WebUtility.Trace("Url: {0}", url);
            String response = this.InternalHttpRequest(url);
            WebUtility.Trace("Response: {0}", (String.IsNullOrWhiteSpace(response) ? "NULL" : response));
            //validate token
            if (String.IsNullOrWhiteSpace(response))
            {
                return null;
            }
            //get profile
            url = String.Format("https://graph.facebook.com/me?{0}&fields=email,first_name,last_name,gender,hometown,locale,name,id,timezone,picture", response);
            WebUtility.Trace("Url: {0}", url);
            response = this.InternalHttpRequest(url);
            WebUtility.Trace("Response: {0}", (String.IsNullOrWhiteSpace(response) ? "NULL" : response));
            //validate profile
            if (String.IsNullOrWhiteSpace(response))
            {
                return null;
            }
            //parse model
            return this.InternalParseUserProfile(response);
        }
        #endregion

        #region - Protected Methods -
        /// <summary>
        /// InternalParseUserProfile
        /// </summary>
        protected override OAuthUserProfile InternalParseUserProfile(string response)
        {
            JObject results = JsonConvert.DeserializeObject<dynamic>(response);
            OAuthUserProfile model = new OAuthUserProfile();
            model.Email = results.GetPropertyValue<String>("email");
            model.FirstName = results.GetPropertyValue<String>("first_name");
            model.LastName = results.GetPropertyValue<String>("last_name");
            model.Locale = results.GetPropertyValue<String>("locale");
            model.Name = results.GetPropertyValue<String>("name");
            return model;
        }
        #endregion
    }
    /// <summary>
    /// OAuthUserProfile
    /// </summary>
    public sealed class OAuthUserProfile
    {
        #region - Properties -
        /// <summary>
        /// Locale
        /// </summary>
        public String Locale { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// FirstName
        /// </summary>
        public String FirstName { get; set; }
        /// <summary>
        /// LastName
        /// </summary>
        public String LastName { get; set; }
        #endregion
    }
}

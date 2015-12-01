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

using Project858.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Project858.Web
{
    /// <summary>
    /// Metody na rozsirenie vlastnosti objektov
    /// </summary>
    public static class ExtensionMethod
    {
        #region - Public Static Methods -
        /// <summary>
        /// Vytvori linker z pozadovaneho stringu
        /// </summary>
        /// <param name="value">Hodnota z ktorej chceme vytvorit linker</param>
        /// <returns>Linker alebo povodna hodnota</returns>
        public static String CreateLinker(this String value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                //lowercase
                value = value.ToLower().Trim();

                //remove all char
                value = Regex.Replace(value, "[:.-]", "");

                //remove all white space
                value = Regex.Replace(value, "\\s+", "-");

                //remove all 
                value = value.RemoveDiacritics();
            }
            return value;
        }
        /// <summary>
        /// Vrati popis enumu s atributom DropDownListAttribute
        /// </summary>
        /// <param name="enumValue">Enum z ktoreho chceme popis enumu ziskat</param>
        /// <returns>Popis alebo null</returns>
        public static String GetDropDownListItemText(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            DropDownListAttribute[] attributes = (DropDownListAttribute[])fieldInfo.GetCustomAttributes(typeof(DropDownListAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                foreach (DropDownListAttribute attribute in attributes)
                {
                    if (!String.IsNullOrWhiteSpace(attribute.Name))
                    {
                        return attribute.Name;
                    }
                    else if (!String.IsNullOrWhiteSpace(attribute.ResourceName))
                    {
                        return attribute.ResourceName;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Odstrani vsetky html tagy zo stringu
        /// </summary>
        /// <param name="value">String v ktorom chceme odstranit tagy</param>
        /// <returns>String bez tagov</returns>
        public static string RemoveAllHtmlTags(this String value)
        {
            return Regex.Replace(value, "<.*?>", string.Empty);
        }
        /// <summary>
        /// Prekovertuje string na HTML string
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme konvertovat</param>
        /// <returns>Html string</returns>
        public static HtmlString ToHtmlStringWithouNewLine(this String value)
        {
            return new HtmlString(Regex.Replace(value, "<br\\s?/>", " "));
        }
        /// <summary>
        /// Prekovertuje string na HTML string
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme konvertovat</param>
        /// <returns>Html string</returns>
        public static HtmlString ToHtmlString(this String value)
        {
            return new HtmlString(value);
        }
        /// <summary>
        /// Vypise chyby aktualneho formu
        /// </summary>
        /// <param name="controller">Controller this</param>
        public static void PrintModelStateError(this ControllerBase controller)
        {
            WebUtility.Trace("---------- Errors ----------");
            foreach (ModelState modelState in controller.ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    WebUtility.Trace(error.ErrorMessage);
                }
            }
            //zalogujeme request
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                HttpRequest request = context.Request;
                if (request != null)
                {
                    WebUtility.Trace("---------- Request ----------");
                    request.TraceRequest();
                }
            }
            WebUtility.Trace("----------------------------");
        }
        /// <summary>
        /// Generates a fully qualified URL to an action method by using
        /// the specified action name, controller name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteAction(this UrlHelper url, String actionName, String controllerName, Object routeValues = null)
        {
            String scheme = url.RequestContext.HttpContext.Request.Url.Scheme;
            return url.Action(actionName, controllerName, routeValues, scheme);
        }
        /// <summary>
        /// Nastavi pozadovanu cookie do response
        /// </summary>
        /// <param name="response">Response pre ktory chceme nastavit cookie</param>
        /// <param name="name">Meno cookie</param>
        /// <param name="value">Hodnota ktoru chceme nastavit</param>
        /// <param name="expireDate">Datum expiracie</param>
        public static void SetCookieValue(this HttpResponseBase response, String name, String value, DateTime expireDate)
        {
            response.SetCookie(new HttpCookie(name, value)
            {
                Expires = expireDate
            });
        }
        /// <summary>
        /// Nastavi pozadovanu cookie do response
        /// </summary>
        /// <param name="response">Response pre ktory chceme nastavit cookie</param>
        /// <param name="name">Meno cookie</param>
        /// <param name="value">Hodnota ktoru chceme nastavit</param>
        /// <param name="expireDate">Datum expiracie</param>
        public static void SetCookieValue(this HttpResponse response, String name, String value, DateTime expireDate)
        {
            response.SetCookie(new HttpCookie(name, value)
            {
                Expires = expireDate
            });
        }
        /// <summary>
        /// Vrati cookie hodnotu pre pozadovany nazov
        /// </summary>
        /// <param name="request">Request z ktoreho chceme cookie zistit</param>
        /// <param name="name">Meno cookie ktorej hodnotu chceme ziskat</param>
        /// <returns>Hodnota cookie alebo null</returns>
        public static String GetCookieValue(this HttpRequestBase request, String name)
        {
            HttpCookie cookie = request.Cookies.Get(name);
            if (cookie != null)
            {
                return cookie.Value;
            }
            return null;
        }
        /// <summary>
        /// Vrati cookie hodnotu pre pozadovany nazov
        /// </summary>
        /// <param name="request">Request z ktoreho chceme cookie zistit</param>
        /// <param name="name">Meno cookie ktorej hodnotu chceme ziskat</param>
        /// <returns>Hodnota cookie alebo null</returns>
        public static String GetCookieValue(this HttpRequest request, String name)
        {
            HttpCookie cookie = request.Cookies.Get(name);
            if (cookie != null)
            {
                return cookie.Value;
            }
            return null;
        }
        /// <summary>
        /// Vypise vsetky podstate informacie z requestu
        /// </summary>
        /// <param name="request">Request z ktoreho chceme vypisat informacie</param>
        public static void TraceRequest(this HttpRequest request)
        {
            if (request != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Request:");
                if (request.AcceptTypes != null && request.AcceptTypes.Length > 0)
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("AcceptTypes: {0}", String.Join(",", request.AcceptTypes));
                }
                if (request.Form != null && request.Form.Count > 0)
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("Form: {0}", request.Form.ToStringAllValues());
                }
                if (!String.IsNullOrWhiteSpace(request.UserAgent))
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("UserAgent: {0}", request.UserAgent);
                }
                if (!String.IsNullOrWhiteSpace(request.HttpMethod))
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("HttpMethod: {0}", request.HttpMethod);
                }
                if (!String.IsNullOrWhiteSpace(request.UserHostAddress))
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("UserHostAddress: {0}", request.UserHostAddress);
                }
                if (request.UserLanguages != null && request.UserLanguages.Length > 0)
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("AcceptTypes: {0}", String.Join(",", request.UserLanguages));
                }
                if (request.UrlReferrer != null)
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("UrlReferrer: {0}", request.UrlReferrer);
                }
                if (!String.IsNullOrWhiteSpace(request.RawUrl))
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("RawUrl: {0}", request.RawUrl);
                }
                WebUtility.Trace(builder.ToString());
            }
        }
        /// <summary>
        /// Vytvori request Id do hlavicky
        /// </summary>
        /// <param name="request">Request do ktoreho chceme pridat id</param>
        public static void TraceRequestHeaders(this HttpRequestBase request)
        {
            if (request != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Request Header:");
                foreach (String key in request.Headers)
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("{0} -> {1}", key, request.Headers[key]);
                }
                WebUtility.Trace(builder.ToString());
            }
        }
        /// <summary>
        /// Vytvori request Id do hlavicky
        /// </summary>
        /// <param name="request">Request do ktoreho chceme pridat id</param>
        public static void TraceRequestHeaders(this HttpRequest request)
        {
            if (request != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Request Header:");
                foreach (String key in request.Headers)
                {
                    builder.Append(Environment.NewLine);
                    builder.AppendFormat("{0} -> {1}", key, request.Headers[key]);
                }
                WebUtility.Trace(builder.ToString());
            }
        }
        /// <summary>
        /// Vytvori request Id do hlavicky
        /// </summary>
        /// <param name="request">Request do ktoreho chceme pridat id</param>
        public static void CreateRequestId(this HttpRequest request)
        {
            request.CreateRequestId(Guid.NewGuid());
        }
        /// <summary>
        /// Vytvori polozku z headra requestu
        /// </summary>
        /// <param name="request">Request z ktoreho chceme ziskat polozku headra</param>
        /// <param name="name">Meno polozky</param>
        /// <returns>Polozka headra alebo null</returns>
        public static String GetHeaderItem(this HttpRequestBase request, String name)
        {
            if (request != null && !String.IsNullOrWhiteSpace(name))
            {
                if (request.Headers.AllKeys.Contains(name))
                {
                    return request.Headers[name];
                }
            }
            return null;
        }
        /// <summary>
        /// Vytvori request Id do hlavicky
        /// </summary>
        /// <param name="id">Id ktore chceme pridat do requestu</param>
        /// <param name="request">Request do ktoreho chceme pridat id</param>
        public static void CreateRequestId(this HttpRequest request, Guid id)
        {
            if (request != null)
            {
                if (!request.Headers.AllKeys.Contains("RequestId"))
                {
                    request.Headers.Add("RequestId", id.ToStringWithoutDash());
                }
            }
        }
        /// <summary>
        /// Vytvori request Id do hlavicky
        /// </summary>
        /// <param name="request">Request do ktoreho chceme pridat id</param>
        public static Nullable<Guid> GetRequestId(this HttpRequest request)
        {
            if (request != null)
            {
                String value = request.Headers["RequestId"];
                if (!String.IsNullOrWhiteSpace(value))
                {
                    return value.ToGuidWithoutDash();
                }
            }
            return null;
        }
        /// <summary>
        /// Vyhlada vsetky url adresy v texte a vytvori z nich html A tag
        /// </summary>
        /// <param name="value">String ktory chceme previest na mvc string</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString ToMvcHtmlStringWithUrl(this string value)
        {
            return WebUtility.GetMvcHtmlStringWithUrl(value);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomPasswordFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var dict = new RouteValueDictionary(htmlAttributes);
            return html.CustomPasswordFor(expression, dict);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomPasswordFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttributes = new Dictionary<string, object>();
            return html.CustomPasswordFor(expression, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomPasswordFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
            var attributes = property.GetCustomAttributes(false);
            var type = property.PropertyType;
            foreach (var attribute in attributes)
            {
                if (type == typeof(String) && attribute.GetType() == typeof(StringAttribute))
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as StringAttribute, htmlAttributes);
                }
                else if (type == typeof(Guid) && attribute.GetType() == typeof(GuidAttribute))
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as GuidAttribute, htmlAttributes);
                }
            }
            return html.PasswordFor(expression, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var dict = new RouteValueDictionary(htmlAttributes);
            return html.CustomTextBoxFor(expression, dict);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttributes = new Dictionary<string, object>();
            return html.CustomTextBoxFor(expression, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
            var attributes = property.GetCustomAttributes(false);
            var type = property.PropertyType;
            foreach (var attribute in attributes) 
            {
                if (type == typeof(String) && attribute.GetType() == typeof(StringAttribute)) 
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as StringAttribute, htmlAttributes);
                }
                else if (type == typeof(Guid) && attribute.GetType() == typeof(GuidAttribute)) 
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as GuidAttribute, htmlAttributes);
                }
                else if (type == typeof(Int32) && attribute.GetType() == typeof(IntegerAttribute))
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as IntegerAttribute, htmlAttributes);
                }
            }
            return html.TextBoxFor(expression, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var dict = new RouteValueDictionary(htmlAttributes);
            return html.CustomTextAreaFor(expression, dict);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttributes = new Dictionary<string, object>();
            return html.CustomTextAreaFor(expression, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html select so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
            var attributes = property.GetCustomAttributes(false);
            var type = property.PropertyType;
            foreach (var attribute in attributes)
            {
                if (type == typeof(String) && attribute.GetType() == typeof(StringAttribute))
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as StringAttribute, htmlAttributes);
                }
                else if (type == typeof(Guid) && attribute.GetType() == typeof(GuidAttribute))
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as GuidAttribute, htmlAttributes);
                }
                else if (type == typeof(Int32) && attribute.GetType() == typeof(IntegerAttribute))
                {
                    htmlAttributes = ExtensionMethod.InternalCustomAttributeFor(attribute as IntegerAttribute, htmlAttributes);
                }
            }
            return html.TextAreaFor(expression, htmlAttributes);
        }

        /// <summary>
        /// Vytvori html select so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttributes = new Dictionary<string, object>();
            return html.CustomDropDownListFor(expression, null, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html select so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="values">Hodnota na nastavenie vybranej polozky</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, List<ISelectListItem> values)
        {
            var htmlAttributes = new Dictionary<string, object>();
            return html.CustomDropDownListFor(expression, values, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html select so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="values">Hodnota na nastavenie vybranej polozky</param>
        /// <param name="htmlAttributes">Html atributy ktore chceme pridat</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, List<ISelectListItem> values, Object htmlAttributes)
        {
            var dict = new RouteValueDictionary(htmlAttributes);
            return html.CustomDropDownListFor(expression, values, dict);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="htmlAttributes">Html atributy ktore chceme pridat</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            return html.CustomDropDownListFor(expression, null, htmlAttributes);
        }
        /// <summary>
        /// Vytvori html input so vstupnymi parametrami
        /// </summary>
        /// <typeparam name="TModel">Model dat</typeparam>
        /// <typeparam name="TValue">Hodnota modelu</typeparam>
        /// <param name="html">Helper</param>
        /// <param name="expression">Expression na ziskanie property</param>
        /// <param name="values">Hodnota na nastavenie vybranej polozky</param>
        /// <param name="htmlAttributes">Html atributy ktore chceme pridat</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString CustomDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, List<ISelectListItem> values, IDictionary<string, object> htmlAttributes)
        {
            List<SelectListItem> collection = new List<SelectListItem>();
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
            var attributes = property.GetCustomAttributes(false);
            var type = property.PropertyType;
            //enum
            if (type.IsEnum) 
            {
                Enum currentValue = (Enum)metadata.Model;
                Array array = Enum.GetValues(type);
                foreach (Enum enumItem in array) 
                {
                    var item = new SelectListItem() { Value = enumItem.ToString(), Text = enumItem.GetDropDownListItemText() };
                    if (currentValue != null && currentValue.GetType() == type && enumItem == currentValue) 
                    {
                        item.Selected = true;
                    }
                    collection.Add(item);
                }
            }
            else 
            {
                if (values != null && values.Count > 0) 
                {
                    var model = metadata.Model;
                    String value = null;
                    if (model != null)
                    {
                        value = (model.GetType() == typeof(Guid)) ? ((Guid)model).ToStringWithoutDash() : model.ToString();
                    }
                    foreach (ISelectListItem listItem in values) 
                    {
                        var item = new SelectListItem() { Value = listItem.Value, Text = listItem.Text, Selected = false };
                        if (!String.IsNullOrWhiteSpace(value) && listItem.Value == value) 
                        {
                            item.Selected = true;
                        }
                        collection.Add(new SelectListItem() { Value = item.Value, Text = item.Text, Selected = false });
                    }
                }
            }
            return html.DropDownListFor(expression, collection);
        }

        /// <summary>
        /// Vytvori html parametre z atributy
        /// </summary>
        /// <param name="attribute">Atribut z ktoreho chceme ziskat data</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>Dictionary</returns>
        public static IDictionary<string, object> InternalCustomAttributeFor(IntegerAttribute attribute, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            htmlAttributes.Add("data-required", attribute.Required.ToString().ToLower());
            if (attribute.Required)
            {
                htmlAttributes.Add("data-maxvalue", attribute.MaxValue);
                htmlAttributes.Add("data-minvalue", attribute.MinValue);
                htmlAttributes.Add("maxlength", attribute.MaxValue.ToString().Length);
                htmlAttributes.Add("data-validation", StringAttribute.StringTypes.Numeric.ToString().ToLower());
                if (!String.IsNullOrWhiteSpace(attribute.ClientValidationHandler))
                {
                    htmlAttributes.Add("data-validationhandler", attribute.ClientValidationHandler.ToLower());
                }
            }
            return htmlAttributes;
        }
        /// <summary>
        /// Vytvori html parametre z atributy
        /// </summary>
        /// <param name="attribute">Atribut z ktoreho chceme ziskat data</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>Dictionary</returns>
        public static IDictionary<string, object> InternalCustomAttributeFor(StringAttribute attribute, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            htmlAttributes.Add("data-required", attribute.Required.ToString().ToLower());
            htmlAttributes.Add("maxlength", attribute.MaxLength);
            htmlAttributes.Add("data-minlength", attribute.MinLength);
            htmlAttributes.Add("data-validation", attribute.Type.ToString().ToLower());
            if (!String.IsNullOrWhiteSpace(attribute.PlaceHolder))
            {
                htmlAttributes.Add("placeholder", attribute.PlaceHolder);
            }
            if (!String.IsNullOrWhiteSpace(attribute.ClientValidationHandler))
            {
                htmlAttributes.Add("data-validationhandler", attribute.ClientValidationHandler.ToLower());
            }
            return htmlAttributes;
        }
        /// <summary>
        /// Vytvori html parametre z atributy
        /// </summary>
        /// <param name="attribute">Atribut z ktoreho chceme ziskat data</param>
        /// <param name="htmlAttributes">Html atributy</param>
        /// <returns>Dictionary</returns>
        public static IDictionary<string, object> InternalCustomAttributeFor(GuidAttribute attribute, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            htmlAttributes.Add("data-required", attribute.Required.ToString().ToLower());
            htmlAttributes.Add("data-validation", "guid");
            if (!String.IsNullOrWhiteSpace(attribute.ClientValidationHandler))
            {
                htmlAttributes.Add("data-validationhandler", attribute.ClientValidationHandler.ToLower());
            }
            return htmlAttributes;
        }
        #endregion
    }
}

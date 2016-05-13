using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Upraveny JsonResult so serializaciou Newtonsoft a upravou niektorych datovych typov
    /// </summary>
    public sealed class JsonNetResult : JsonResult
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public JsonNetResult()
        {
            //inicializacia
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };

            //pridame konvertory
            this.Settings.Converters.Add(new GuidJsonConverter());
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Konfiguracia serializera
        /// </summary>
        public JsonSerializerSettings Settings { get; private set; }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Spusti convet objektu do Json stringu
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data == null)
                return;

            var scriptSerializer = JsonSerializer.Create(this.Settings);
            scriptSerializer.Serialize(response.Output, this.Data);
        }
        #endregion
    }
}

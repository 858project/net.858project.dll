using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Binder na parsovanie DateTime vo formate ISO 8601
    /// </summary>
    public sealed class BinderNullableDateTimeOffsetIso8601 : DefaultModelBinder
    {
        #region - Public Methods -
        /// <summary>
        /// Spracuje bindig na model
        /// </summary>
        /// <param name="controllerContext">ControllerContext</param>
        /// <param name="bindingContext"ModelBindingContext></param>
        /// <returns>Vyparsovany objekt alebo null</returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var name = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(name);
            if (value == null) 
                return null;

            return parseIso8601Date(value) ?? base.BindModel(controllerContext, bindingContext);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vyparsuje dateTime vo formate ISO 8601
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme parsovat</param>
        /// <returns>DateTime alebo null</returns>
        private DateTimeOffset? parseIso8601Date(ValueProviderResult value)
        {
            DateTimeOffset date;
            return DateTimeOffset.TryParse(value.AttemptedValue, null, DateTimeStyles.RoundtripKind, out date) ? date : null as DateTimeOffset?;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Web
{
    /// <summary>
    /// Binder na parsovanie decimal hodnoty s ciarkou alebo bodkou
    /// </summary>
    public sealed class DecimalBinder : DefaultModelBinder
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

            return this.parseDecimal(value) ?? base.BindModel(controllerContext, bindingContext);
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vyparsuje dateTime vo formate ISO 8601
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme parsovat</param>
        /// <returns>DateTime alebo null</returns>
        private Object parseDecimal(ValueProviderResult value)
        {
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalDigits = 2;
            numberFormatInfo.NumberDecimalSeparator = ".";
            Decimal decimalValue = 0;

            if (Decimal.TryParse(value.AttemptedValue.Replace(",", "."), NumberStyles.Any, numberFormatInfo, out decimalValue))
            {
                return decimalValue;
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}

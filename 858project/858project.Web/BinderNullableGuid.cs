using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Project858;

namespace Project858.Web
{
    /// <summary>
    /// Binder na parsovanie decimal hodnoty s ciarkou alebo bodkou
    /// </summary>
    public sealed class BinderNullableGuid : DefaultModelBinder
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

            return this.parseGuid(value) ?? base.BindModel(controllerContext, bindingContext);
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vyparsuje guid bez pomlcok
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme parsovat</param>
        /// <returns>DateTime alebo null</returns>
        private Nullable<Guid> parseGuid(ValueProviderResult value)
        {
            return value.AttemptedValue.ToGuidWithoutDash();
        }
        #endregion
    }
}

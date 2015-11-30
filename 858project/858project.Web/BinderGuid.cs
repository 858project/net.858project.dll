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
    public sealed class BinderGuid : DefaultModelBinder
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

            return this.parseGuid(value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vyparsuje dateTime vo formate ISO 8601
        /// </summary>
        /// <param name="value">Hodnota ktoru chceme parsovat</param>
        /// <returns>DateTime alebo null</returns>
        private Guid parseGuid(ValueProviderResult value)
        {
            Nullable<Guid> valueGuid = value.AttemptedValue.ToGuidWithoutDash();
            return valueGuid.HasValue ? valueGuid.Value : Guid.Empty;
        }
        #endregion
    }
}

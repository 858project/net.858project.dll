using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Atribut pre nacitanie select-u
    /// </summary>
    public sealed class DropDownListAttribute : Attribute
    {
        #region - Properties -
        /// <summary>
        /// Nazov jazyka
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Menou resource odkial chceme text nacitat
        /// </summary>
        public string ResourceName { get; set; }
        #endregion
    }
}

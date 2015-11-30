using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Atribut definujuci jazyk
    /// </summary>
    public sealed class LanguageAttribute : Attribute
    {
        #region - Properties -
        /// <summary>
        /// Definovana kultura jazyka
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// Nazov jazyka
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Iso nazov jazyka
        /// </summary>
        public string IsoCode { get; set; }
        #endregion
    }
}

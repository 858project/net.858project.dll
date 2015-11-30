using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Data.SqlClient
{
    /// <summary>
    /// Atribut na definovanie informacii o view
    /// </summary>
    public sealed class ViewAttribute : Attribute
    {
        #region - Properties -
        /// <summary>
        /// Meno view tabulky
        /// </summary>
        public String Name { get; set; }
        #endregion
    }
}

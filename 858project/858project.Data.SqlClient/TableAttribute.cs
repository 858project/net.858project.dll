using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Data.SqlClient
{
    /// <summary>
    /// Atribut na definovanie informacii o tabulke
    /// </summary>
    public sealed class TableAttribute : Attribute
    {
        #region - Properties -
        /// <summary>
        /// Meno tabulky
        /// </summary>
        public String Name { get; set; }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Eror definujuci dovod zamietnutia
    /// </summary>
    public sealed class ErrorPairItem : PairItemBase
    {
        #region - Properties -
        /// <summary>
        /// Definuje popis chyby
        /// </summary>
        public String Error { get; set; }
        #endregion
    }
}

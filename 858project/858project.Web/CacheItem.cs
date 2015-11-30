using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Buffrovaci item dat aby sa data neustale necitali z DB
    /// </summary>
    public sealed class CacheItem
    {
        #region - Properties -
        /// <summary>
        /// Timeout platnosi polozky, alebo null
        /// </summary>
        public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// Jedinecny identifikator objektu
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// Casova znacka kedy bol objekt pridany
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Data ktore polozka obsahuje
        /// </summary>
        public Object Data { get; set; }
        #endregion
    }
}

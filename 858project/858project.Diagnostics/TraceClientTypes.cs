using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Diagnostics
{
    /// <summary>
    /// Definuje dostupne typy Trace klientov
    /// </summary>
    public enum TraceClientTypes
    {
        /// <summary>
        /// Ziadny / nedefinovany klient
        /// </summary>
        None,
        /// <summary>
        /// Klient logujuci do SQLite databazy
        /// </summary>
        SQLite
    }
}

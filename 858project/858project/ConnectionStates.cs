using System;
using System.Collections.Generic;
using System.Text;

namespace Project858
{
    /// <summary>
    /// Urcuje stavy v akych sa moze nachadzat komunikacia
    /// </summary>
    public enum ConnectionStates
    {
        /// <summary>
        /// Prebieha pripajanie
        /// </summary>
        Connecting,
        /// <summary>
        /// Pripojenie je aktivne
        /// </summary>
        Connected,
        /// <summary>
        /// Spojenie je zatvorene
        /// </summary>
        Closed,
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.ComponentModel.Client
{
    /// <summary>
    /// Enum definujuci stavy klienta
    /// </summary>
    public enum ClientStates
    {
        /// <summary>
        /// Klient je zastaveny
        /// </summary>
        Stop,
        /// <summary>
        /// Klient je spusteny
        /// </summary>
        Start,
        /// <summary>
        /// Klient je pozastaveny
        /// </summary>
        Pause
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Project858.Diagnostics;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Component zabezpecujuci logovanie informacii
    /// </summary>
    public interface ITraceComponent
    {
        #region - Event -
        /// <summary>
        /// Event oznamujuci poziadavku na logovanie informacii
        /// </summary>
        event TraceEventHandler TraceEvent;
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Klient zabezpecujuci logovanie data
        /// </summary>
        ITraceClient TraceClient {get; set; }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'TraceEvent' v asynchronnom mode
        /// </summary>
        Boolean TraceEventAsync {get; set; }
        /// <summary>
        /// (Get / Set) Definuje typ logovania informacii
        /// </summary>
        TraceTypes TraceType {get; set; }
        /// <summary>
        /// (Get / Set) Definuje ci je logovanie chyb zapnute za kazdych okolnosti
        /// </summary>
        Boolean TraceErrorAlways {get; set; }
        /// <summary>
        /// (Get / Set) Definuje ci je zapnute logovanie informaci v ramci klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        Boolean TraceInternalProcess {get; set; }
        #endregion
    }
}

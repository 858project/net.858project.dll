using System;
using System.Collections.Generic;
using System.Text;
using Project858.ComponentModel.Client;

namespace Project858.Diagnostics
{
    /// <summary>
    /// Interface ako predpis pre klienta zabezpecujuceho logovanie informacii
    /// </summary>
    public interface ITraceClient : IClient
    {
        #region - Method -
        /// <summary>
        /// Zaloguje pozadovane data
        /// </summary>
        /// <param name="time">Cas logovacej spravy</param>
        /// <param name="traceType">Typ logovania</param>
        /// <param name="modulName">Nazov modulu ktory spravu vytvoril</param>
        /// <param name="message">Sprava s informaciou</param>
        /// <returns>True = logovanie bolo uspesne</returns>
        Boolean Trace(DateTime time, TraceTypes traceType, String modulName, String message);
        /// <summary>
        /// Zaloguje pozadovane data
        /// </summary>
        /// <param name="time">Cas logovacej spravy</param>
        /// <param name="traceType">Typ logovania</param>
        /// <param name="modulName">Nazov modulu ktory spravu vytvoril</param>
        /// <param name="message">Sprava s informaciou</param>
        void TraceAsync(DateTime time, TraceTypes traceType, String modulName, String message);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.Diagnostics
{
    /// <summary>
    /// EventArgs pre event oznamujuci poziadavku na logovanie informacii
    /// </summary>
    public class TraceEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Chybny argument typu String
        /// </exception>
        /// <param name="time">Cas a datum vzniku logovacej spravy</param>
        /// <param name="modul">Meno modulu ktory spravu vytvorill</param>
        /// <param name="traceType">Typ logovacej spravy</param>
        /// <param name="message">Sprava s informaciou</param>
        public TraceEventArgs(DateTime time, String modul, TraceTypes traceType, String message)
        {
            //osetrime vstupne parametre
            if (String.IsNullOrEmpty(modul))
                throw new ArgumentNullException("modul");
            if (String.IsNullOrEmpty(modul))
                throw new ArgumentNullException("message");

            this.time = time;
            this.modul = modul;
            this.traceType = traceType;
            this.message = message;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Cas a datum vzniku spravy
        /// </summary>
        public DateTime Time
        {
            get { return time; }
        }
        /// <summary>
        /// (Get) Modul ktory spravu vytvoril
        /// </summary>
        public String Modul
        {
            get { return modul; }
        }
        /// <summary>
        /// (Get) Typ logovacej spravy
        /// </summary>
        public TraceTypes TraceType
        {
            get { return traceType; }
        }
        /// <summary>
        /// (Get) Sprava informacia....
        /// </summary>
        public String Message
        {
            get { return message; }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Cas a datum vzniku spravy
        /// </summary>
        private DateTime time = DateTime.MinValue;
        /// <summary>
        /// Modul ktory spravu vytvoril
        /// </summary>
        private String modul = String.Empty;
        /// <summary>
        /// Typ logovacej spravy
        /// </summary>
        private TraceTypes traceType = TraceTypes.Off;
        /// <summary>
        /// Sprava informacia....
        /// </summary>
        private String message = String.Empty;
        #endregion
    }
}

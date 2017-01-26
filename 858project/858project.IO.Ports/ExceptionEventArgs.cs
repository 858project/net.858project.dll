using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Project858.IO.Ports
{
    /// <summary>
    /// EventArgs pre event na prichod dat
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Prijate data</param>
        public ExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
        }
        #endregion
 
        #region - Get -
        /// <summary>
        /// (Get) Prijate data
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
        #endregion
    }
}

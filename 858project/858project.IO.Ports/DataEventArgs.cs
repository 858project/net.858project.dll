using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Project858.IO.Ports
{
    /// <summary>
    /// EventArgs pre event na prichod dat
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="data">Prijate data</param>
        /// <param name="comPortName">Com port from which data are received</param>
        public DataEventArgs(Byte[] data, String comPortName)
        {
            this.Data = data;
            this.ComPortName = comPortName;
        }
        #endregion
 
        #region - Properties -
        /// <summary>
        /// Com port from which data are received
        /// </summary>
        public String ComPortName
        {
            get;
            private set;
        }
        /// <summary>
        /// (Get) Prijate data
        /// </summary>
        public Byte[] Data
        {
            get;
            private set;
        }
        #endregion
    }
}

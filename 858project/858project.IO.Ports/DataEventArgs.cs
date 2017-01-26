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
        public DataEventArgs(Byte[] data)
        {
            this.Data = data;
        }
        #endregion
 
        #region - Get -
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

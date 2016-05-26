using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Project858.Net
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
        /// <param name="remoteEndPoint">EndPoint odosielatela dat</param>
        public DataEventArgs(Byte[] data, IPEndPoint remoteEndPoint)
        {
            this.m_data = data;
            this.m_remoteEndPoint = remoteEndPoint;
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// prijate data
        /// </summary>
        private Byte[] m_data = null;
        /// <summary>
        /// EndPoint odosielatela dat
        /// </summary>
        private IPEndPoint m_remoteEndPoint = null;
        #endregion

        #region - Get -
        /// <summary>
        /// (Get) Prijate _data
        /// </summary>
        public Byte[] Data
        {
            get { return this.m_data; }
        }
        /// <summary>
        /// (Get) End point z ktoreho boli data prijate
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return m_remoteEndPoint; }
        }
        #endregion
    }
}

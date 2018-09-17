using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// EventArgs pre event na prichod frame
    /// </summary>
    public class PackageEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="frame">Prijaty frame</param>
        /// <param name="comPortName">Port name from serial port client</param>
        public PackageEventArgs(IPackage frame, String comPortName)
        {
            this.Package = frame;
            this.ComPortName = comPortName;
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="frame">Prijaty frame</param>
        /// <param name="remoteEndPoint">EndPoint odosielatela dat</param>
        public PackageEventArgs(IPackage frame, IPEndPoint remoteEndPoint)
        {
            this.Package = frame;
            this.RemoteEndPoint = remoteEndPoint;
        }
        #endregion
 
        #region - Properties -
        /// <summary>
        /// Port name from serial port client
        /// </summary>
        public String ComPortName
        {
            get;
            private set;
        }
        /// <summary>
        /// (Get) Prijate _data
        /// </summary>
        public IPackage Package
        {
            get;
            private set;
        }
        /// <summary>
        /// (Get) End point z ktoreho boli data prijate
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get;
            private set;
        }
        #endregion
    }
}

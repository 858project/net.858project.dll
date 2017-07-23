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
    public class FrameEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="frame">Prijaty frame</param>
        /// <param name="comPortName">Port name from serial port client</param>
        public FrameEventArgs(IFrame frame, String comPortName)
        {
            this.Frame = frame;
            this.ComPortName = comPortName;
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="frame">Prijaty frame</param>
        /// <param name="remoteEndPoint">EndPoint odosielatela dat</param>
        public FrameEventArgs(IFrame frame, IPEndPoint remoteEndPoint)
        {
            this.Frame = frame;
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
        public IFrame Frame
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

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
        /// <param name="remoteEndPoint">EndPoint odosielatela dat</param>
        public FrameEventArgs(Frame frame, IPEndPoint remoteEndPoint)
        {
            this.Frame = frame;
            this.RemoteEndPoint = remoteEndPoint;
        }
        #endregion
 
        #region - Get -
        /// <summary>
        /// (Get) Prijate _data
        /// </summary>
        public Frame Frame
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

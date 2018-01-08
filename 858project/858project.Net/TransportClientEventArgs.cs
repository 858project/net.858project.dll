using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project858.Net
{
    /// <summary>
    /// EventArgs with Transport Client
    /// </summary>
    public class TransportClientEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="client">Client</param>
        public TransportClientEventArgs(ITransportClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            this.Client = client;
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Client 
        /// </summary>
        public ITransportClient Client { get; private set; }
        #endregion
    }
}

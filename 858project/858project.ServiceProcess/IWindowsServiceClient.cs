using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858.ComponentModel.Client;

namespace Project858.ServiceProcess
{
    /// <summary>
    /// Client ktory sa automaticky spusta pre WndowsService
    /// </summary>
    public interface IWindowsServiceClient : IClient
    {
        #region - Properties -
        /// <summary>
        /// Priority for starting clients
        /// </summary>
        UInt16 ServiceClientPriority { get; }
        #endregion
    }
}

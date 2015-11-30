using System;
using System.Collections.Generic;
using System.Text;
using Project858.Data.SqlClient;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using Project858.Diagnostics;
using Project858.ComponentModel.Client;
using System.Net.Mail;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Component zabezpecujuci pristup k vykonavaniu poziadaviek na sql server
    /// </summary>
    public interface ISqlComponent
    {
        #region - Properties -
        /// <summary>
        /// (Get / Set) Klient zabezpecujuci pristup k SQL serveru a vykonavanie prikazov
        /// </summary>
        ISqlClient SqlClient { get; set; }
        #endregion
    }
}

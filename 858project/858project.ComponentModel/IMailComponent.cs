using System;
using System.Collections.Generic;
using System.Text;
using Project858.Net.Mail;
using System.ComponentModel;
using System.Net.Mail;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Interface predpisujuci vzhlad komponenty s podporov odosielania emailov
    /// </summary>
    public interface IMailComponent
    {
        #region - Event -
        /// <summary>
        /// Event oznamujuci poziadavku na odoslanie emailu
        /// </summary>
        event MailEventHandler MailEvent;
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje ci je event 'EmailEvent' v asynchronnom mode
        /// </summary>
        Boolean MailEventAsync { get; set; }
        /// <summary>
        /// (Get / Set) Definuje ci je zapnute  odosielanie emailovych sprav v ramci klienta
        /// </summary>
        Boolean MailMessageInternalProcess { get; set; }
         /// <summary>
        /// (Get / Set) Klient zabezpecujuci odosielanie emailovych správ
        /// </summary>
        IMailClient MailClient { get; set; }
        #endregion
    }
}

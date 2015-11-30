using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using Project858.ComponentModel.Client;

namespace Project858.Net.Mail
{
    /// <summary>
    /// Interface ako predpis pre klienta zabezpecujuceho odosielani emailovych správ
    /// </summary>
    public interface IMailClient : IClient
    {
        #region - Properties -
        /// <summary>
        /// (Get) Meno smtp servera na odosielanie sprav
        /// </summary>
        String SmtpServer { get; }
        /// <summary>
        /// (Get) Login k smtp serveru
        /// </summary>
        String SmtpLogin { get; }
        /// <summary>
        /// (Get) Heslo k smtp serveru
        /// </summary>
        String SmtpPassword { get; }
        /// <summary>
        /// (Get) Definuje ci je odosielanie zabezpecene pomocou SSL
        /// </summary>
        Boolean SmtpSsl { get; }
        /// <summary>
        /// Kolekcia prijemcov emailovych sprav
        /// </summary>
        MailAddressCollection Recipients { get; }
        /// <summary>
        /// Odosielatel emailovej spravy
        /// </summary>
        MailAddress Sender { get; }
        #endregion

        #region - Method -
        /// <summary>
        /// Odosle emailovu spravu
        /// </summary>
        /// <param name="message">Sprava ktoru chceme odoslat</param>
        Boolean Send(MailMessage message);
        /// <summary>
        /// Odosle emailovu spravu v asynchronnom móde
        /// </summary>
        /// <param name="message">Sprava ktoru chceme odoslat</param>
        void SendAsync(MailMessage message);
        #endregion
    }
}

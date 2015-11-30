using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace Project858.Net.Mail
{    
    /// <summary>
    /// EventArgs pre event oznamujuci poziadavku na odoslanie emailovej spravy
    /// </summary>
    public class MailEventArgs : EventArgs
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovana sprava
        /// </exception>
        /// <param name="message">Sprava emailu</param>
        public MailEventArgs(MailMessage message)
        {
            //osetrenie vstupneho argumentu
            if (message == null)
                throw new ArgumentNullException("message", "Message cannot be null.");

            //uchovame si spravu
            this.message = message;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Sprava ktoru chceme odoslat
        /// </summary>
        public MailMessage Message
        {
            get { return message; }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Sprava ktoru chceme odoslat
        /// </summary>
        private MailMessage message = null;
        #endregion
    }
}

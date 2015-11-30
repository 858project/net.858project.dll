using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using Project858.ComponentModel.Client;
using Project858.Diagnostics;

namespace Project858.Net.Mail
{
    /// <summary>
    /// ClientBase zabezpecujuci odosielanie emailovych sprav
    /// </summary>
    public sealed class MailClient : ClientBase, IMailClient
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chyba vstupnych argumentov
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Nepovoleny rozsah portu
        /// </exception>
        /// <param name="SmtpServer">SMTP server na odosielanie emailov</param>
        /// <param name="SmtpPort">Port SMTP servera</param>
        /// <param name="SmtpLogin">Prihlasovacie meno k SMTP serveru</param>
        /// <param name="SmtpPassword">Prihlasovacie heslo k SMTP serveru</param>
        /// <param name="Sender">Odosielatel emailu</param>
        /// <param name="Recipients">Zoznam prijemcov emailu</param>
        public MailClient(String SmtpServer, Int32 SmtpPort, String SmtpLogin, String SmtpPassword, MailAddress Sender, MailAddressCollection Recipients)
            : base()
        {
            //osetrenie argumentov
            if (String.IsNullOrEmpty(SmtpServer))
                throw new ArgumentException("The 'SmtpServer' string was not found.");
            if (IPEndPoint.MaxPort < SmtpPort || IPEndPoint.MinPort > SmtpPort)
                throw new ArgumentOutOfRangeException("SmtpPort must be between 0 and 65535.");
            if (String.IsNullOrEmpty(SmtpLogin))
                throw new ArgumentException("The 'SmtpLogin' string was not found.");
            if (String.IsNullOrEmpty(SmtpPassword))
                throw new ArgumentException("The 'SmtpPassword' string was not found.");
            if (Sender == null)
                throw new ArgumentException("The 'Sender' string was not found.");
            if (Recipients == null || Recipients.Count == 0)
                throw new ArgumentException("The 'Recipients' list was not found.");

            //uchovame si vstupne hodnoty
            smtpServer = SmtpServer;
            smtpPort = SmtpPort;
            smtpLogin = SmtpLogin;
            smtpPassword = SmtpPassword;
            sender = Sender;
            recipients = new MailAddressCollection();
            foreach (MailAddress address in Recipients)
                recipients.Add(address);
        }
        #endregion

        #region - Delegate -
        /// <summary>
        /// Pomocny delegat na asinchronne posielanie sprav
        /// </summary>
        /// <param name="message">Sprava na odoslanie</param>
        private delegate Boolean SendDelegate(MailMessage message);
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) SMTP server cez ktory sa odosielaju emaily
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Nedovoleny pristup. Objekt je v stave _disposed
        /// </exception>
        public String SmtpServer
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return smtpServer;
            }
        }
        /// <summary>
        /// (Get) Port k SMTP serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Nedovoleny pristup. Objekt je v stave _disposed
        /// </exception>
        public Int32 SmtpPort
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return smtpPort;
            }
        }
        /// <summary>
        /// (Get) Prihlasovacie meno k SMTP serveru.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Nedovoleny pristup. Objekt je v stave _disposed
        /// </exception>
        public string SmtpLogin
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return smtpLogin;
            }
        }
        /// <summary>
        /// (Get) Prihlasovacie heslo k SMTP serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Nedovoleny pristup. Objekt je v stave _disposed
        /// </exception>
        public string SmtpPassword
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return smtpPassword;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je odosielanie emailov zabezpecene pomocou SSL
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Nedovoleny pristup. Objekt je v stave _disposed
        /// </exception>
        public bool SmtpSsl
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return smtpSsl;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                smtpSsl = value;
            }
        }
        /// <summary>
        /// (Get) Kolekcia prijemcov emailovej spravy
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Nedovoleny pristup. Objekt je v stave _disposed
        /// </exception>
        public MailAddressCollection Recipients
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this.recipients;
            }
        }
        /// <summary>
        /// (Get) Emailova adresa odosielatela emailu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Nedovoleny pristup. Objekt je v stave _disposed
        /// </exception>
        public MailAddress Sender
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return sender;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Definuje ci je zapnute sifrovanie pripojenie k serveru
        /// </summary>
        private Boolean smtpSsl = false;
        /// <summary>
        /// Zoznam prijemcov emailov
        /// </summary>
        private MailAddressCollection recipients = null;
        /// <summary>
        /// Emailova adresa odosielatela emailu
        /// </summary>
        private MailAddress sender = null;
        /// <summary>
        /// SMTP server cez ktory sa odosielaju emaily
        /// </summary>
        private String smtpServer = String.Empty;
        /// <summary>
        /// Port k SMTP serveru
        /// </summary>
        private Int32 smtpPort = 0;
        /// <summary>
        /// Prihlasovacie meno k serveru
        /// </summary>
        private String smtpLogin = String.Empty;
        /// <summary>
        /// Prihlasovacie heslo k serveru
        /// </summary>
        private String smtpPassword = String.Empty;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Odosle spravu. Ak sa spravu nepodari odoslat a je aktivovane 'AutoRepeat' sprava bude opakovane odosielana
        /// v pravidelnych intervaloch. Platnost spravy definuje 'ValidityInterval'
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Chyba vstupneho argumentu. Sprava neobsahuje potrebne udaje
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Chyba chybnej inicializacie spravy alebo jej poloziek
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Snaha o odoslanie spravy ak modul nebol spusteny
        /// </exception>
        /// 
        /// <param name="message">Sprava ktoru chceme odoslat</param>
        /// <returns>True = sprava bola uspesne odoslana</returns>
        public void SendAsync(MailMessage message)
        {
            //ak uz bola vrstva spustena
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Invalid operation.");

            //osetrenie vstupneho argumentu
            if (message == null)
                throw new ArgumentNullException("message", "Message cannot be null.");
            if (message.To.Count == 0)
                throw new ArgumentException("message", "Message.To must be entered.");
            if (message.From == null)
                throw new ArgumentNullException("message", "Message.From cannot be null.");
            if (String.IsNullOrEmpty(message.Subject))
                throw new ArgumentException("message", "Message.Subject must be entered.");
            if (String.IsNullOrEmpty(message.Body))
                throw new ArgumentException("message", "Message.Body must be entered.");

            //zalogujeme
            this.InternalTrace(TraceTypes.Verbose, "Poziadavka na asynchronne odoslanie spravy. {0}", message.Subject);

            //pomocny delegat
            SendDelegate sendDelegate = new SendDelegate(this.Send);
            sendDelegate.BeginInvoke(message, null, null);
        }
        /// <summary>
        /// Odosle spravu. Ak sa spravu nepodari odoslat a je aktivovane 'AutoRepeat' sprava bude opakovane odosielana
        /// v pravidelnych intervaloch. Platnost spravy definuje 'ValidityInterval'
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Chyba vstupneho argumentu. Sprava neobsahuje potrebne udaje
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Chyba chybnej inicializacie spravy alebo jej poloziek
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Snaha o odoslanie spravy ak modul nebol spusteny
        /// </exception>
        /// 
        /// <param name="message">Sprava ktoru chceme odoslat</param>
        /// <returns>True = sprava bola uspesne odoslana</returns>
        public Boolean Send(MailMessage message)
        {
            //ak uz bola vrstva spustena
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Invalid operation.");

            //osetrenie vstupneho argumentu
            if (message == null)
                throw new ArgumentNullException("message", "Message cannot be null.");
            if (message.To.Count == 0)
                throw new ArgumentNullException("message", "Message.From cannot be null.");
            if (message.From == null)
                throw new ArgumentException("message", "Message.To must be entered.");
            if (String.IsNullOrEmpty(message.Subject))
                throw new ArgumentException("message", "Message.Subject must be entered.");
            if (String.IsNullOrEmpty(message.Body))
                throw new ArgumentException("message", "Message.Body must be entered.");

            //zalogujeme
            this.InternalTrace(TraceTypes.Verbose, "Poziadavka na odoslanie spravy. {0}", message.Subject);

            //odosleme spravu
            return this.SendMessage(message);
        }
        /// <summary>
        /// Vrati meno / popis triedy / modulu
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("MailClient");
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Prepis / implementacia metody na vykonanie startu klienta / modulu
        /// </summary>
        /// <returns>True = klient bol uspesne iniializovany</returns>
        protected override bool InternalStart()
        {
            //start klienta bol uspesny
            return true;
        }
        /// <summary>
        /// Prepis / implementacia metody na vykonanie pozastavenia klienta / modulu
        /// </summary>
        protected override void InternalPause()
        {
            base.InternalPause();
        }
        /// <summary>
        /// Prepis / implementacia metody na vykonanie ukoncenia klienta / modulu
        /// </summary>
        protected override void InternalStop()
        {
        }
        /// <summary>
        /// Deinicializuje cely objekt
        /// </summary>
        protected override void InternalDispose()
        {
        }
        /// <summary>
        /// Odosle spravy na smtp server.
        /// </summary>
        /// <param name="message">Spravy ktore chceme odoslat</param>
        /// <returns>Pole sprav ktore sa nam nepodarilo odoslat</returns>
        private Boolean SendMessage(MailMessage message)
        {
            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Inicializacia SMTP klienta na odoslanie spravy... {0}", message.Subject);

                //inicializujeme smtp klienta
                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                //aktivujeme zabezpecene pripojenie
                smtpClient.EnableSsl = smtpSsl;
                //autorizacia k serveru
                smtpClient.Credentials = new NetworkCredential(smtpLogin, smtpPassword);

                //pokusime sa odoslat spravu
                smtpClient.Send(message);

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Sprava '{0}' bola uspesne odoslana.", message.Subject);
                //sprava bola uspesne odoalana
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri odosielani spravy '{0}'.", ex.Message);
                //chyba pri odosielani spravy
                return false;
            }
        }
        #endregion
    }
}

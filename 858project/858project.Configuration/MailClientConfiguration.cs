using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Net;
using System.Security.Cryptography;

namespace Project858.Configuration
{
    /// <summary>
    /// Konfiguracia Mail klienta zabezpecujuceho odosielanie emailov
    /// </summary>
    [Serializable]
    public class MailClientConfiguration
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public MailClientConfiguration()
        {
            this.recipients = new List<String>();
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje ci je odosielani mailov aktivne alebo nie
        /// </summary>
        [XmlElement(ElementName = "enable")]
        public Boolean Enable
        {
            get { return enable; }
            set { enable = value; }
        }
        /// <summary>
        /// (Get / Set) Meno smtp servera na odosielanie emailov
        /// </summary>
        [XmlElement(ElementName = "smtpServer")]
        public String SmtpServer
        {
            get
            {
                if (this.enable == false)
                    return String.Empty;
                else
                    return this.smtpServer;
            }
            set
            {
                smtpServer = value;
            }
        }
        /// <summary>
        /// (Get / Set) Smtp smtpPort na odosielanie emailov
        /// </summary>
        [XmlElement(ElementName = "smtpPort")]
        public Int32 SmtpPort
        {
            get { return smtpPort; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Argument 'SmtpPort' is not valid !");

                smtpPort = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je zapnute sifrovanie pripojenie k serveru
        /// </summary>
        [XmlElement(ElementName = "smtpSsl")]
        public Boolean SmtpSsl
        {
            get { return smtpSsl; }
            set { smtpSsl = value; }
        }
        /// <summary>
        ///(Get / Set) Login k smtp serveru
        /// </summary>
        [XmlElement(ElementName = "smtpLogin")]
        public String SmtpLogin
        {
            get
            {
                if (this.enable == false)
                    return String.Empty;
                else
                    return this.smtpLogin;
            }
            set
            {
                smtpLogin = value;
            }
        }
        /// <summary>
        /// (Get / Set) Heslo k smtp serveru
        /// </summary>
        [XmlElement(ElementName = "smtpPassword")]
        public String SmtpPassword
        {
            get
            {
                if (this.enable == false)
                    return String.Empty;
                else
                    return this.smtpPassword;
            }
            set
            {
                this.smtpPassword = value;
            }
        }
        /// <summary>
        /// (Get / Set) Adresa odosielatela emailu
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chyba pri nacitavani serializacii, zly argument
        /// </exception>
        [XmlElement(ElementName = "sender")]
        public String Sender
        {
            get
            {
                if (this.enable == false)
                    return String.Empty;
                else
                    return this.sender;
            }
            set
            {
                //len ak je mailove notifikacia aktivna
                if (this.enable)
                {
                    if (!Utility.ValidateMailAddress(value))
                        throw new ArgumentException("Argument 'Sender' is not valid !");

                    this.sender = value;
                }
            }
        }
        /// <summary>
        /// Meno odosielatela sprav
        /// </summary>
        [XmlElement(ElementName = "senderName")]
        public String SenderName
        {
            get {
                if (this.enable == false)
                    return String.Empty;
                else
                    return senderName;
            }
            set {
                //len ak je mailove notifikacia aktivna
                if (this.enable)
                {
                    if (String.IsNullOrEmpty(value))
                        throw new ArgumentException("Argument 'SenderName' is not valid !");

                    this.senderName = value;
                } 
            }
        }
        /// <summary>
        /// (Get / Set) Ip smtpPort ktory klient registruje
        /// </summary>
        [XmlElement(ElementName = "recipient")]
        public List<String> Recipients
        {
            get { return this.recipients; }
            set { this.recipients = value; }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Meno odosielatela sprav
        /// </summary>
        private String senderName = String.Empty;
        /// <summary>
        /// Definuje ci je zapnute sifrovanie pripojenie k serveru
        /// </summary>
        private Boolean smtpSsl = false;
        /// <summary>
        /// Definuje ci je odosielanie mailov aktivne alebo nie
        /// </summary>
        private Boolean enable = false;
        /// <summary>
        /// Meno smtp servera na odosielanie emailov
        /// </summary>
        private String smtpServer = String.Empty;
        /// <summary>
        /// Smtp smtpPort na odosielanie emailov
        /// </summary>
        private Int32 smtpPort = 25;
        /// <summary>
        /// Login k smtp serveru
        /// </summary>
        private String smtpLogin = String.Empty;
        /// <summary>
        /// Heslo k smtp serveru
        /// </summary>
        private String smtpPassword = String.Empty;
        /// <summary>
        /// Emailova adresa odosielatela emailu
        /// </summary>
        private String sender = String.Empty;
        /// <summary>
        /// Kolekcia emailovych adries prijemcov emailov systemu
        /// </summary>
        private List<String> recipients = null;
        #endregion

        #region - Method -
        /// <summary>
        /// Overi spravnost poloziek z konfiguracie
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybny argument
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Cryptovacie heslo nie je zadane
        /// </exception>
        /// <param name="cryptoPassword">Cryptovacie heslo na overenie konfiguracie</param>
        /// <returns>True = konfiguracia je spravna</returns>
        public Boolean Validate(String cryptoPassword)
        {
            //overnie vstupneho argumentu
            if (String.IsNullOrEmpty(cryptoPassword))
                throw new ArgumentNullException("cryptoPassword");

            //ak je mailove odosielanie aktivne
            if (this.enable)
            {
                //overime adresy prijemcov
                Boolean _recipients = false;
                foreach (String address in this.recipients)
                {
                    if (!Utility.ValidateMailAddress(address))
                        throw new ArgumentException("Argument 'Recipients' is not valid !");
                    else
                        _recipients = true;
                }

                //ak su nejake adresy k dispozicii
                if (!_recipients)
                    throw new ArgumentException("Argument 'Recipients' is not valid !");

                //overime ci je zadana
                if (String.IsNullOrEmpty(this.sender))
                    throw new ArgumentException("Argument 'Sender' is not valid !");

                //overime ci je zadany smtp _sqlServer
                if (String.IsNullOrEmpty(this.smtpServer))
                    throw new ArgumentException("Argument 'SmtpServer' is not valid !");

                //overime zadanie loginu
                if (String.IsNullOrEmpty(this.smtpLogin))
                    throw new ArgumentException("Argument 'SmtpLogin' is not valid !");

                //overime zadanie hesla
                if (String.IsNullOrEmpty(this.smtpPassword))
                    throw new ArgumentException("Argument 'SmtpPassword' is not valid !");

                //overime ci je heslo v subore spravne zacryptovane
                String _password = String.Empty;
                if (!Utility.TryStringDecrypt(this.smtpPassword, cryptoPassword, out _password))
                    throw new CryptographicException("Argument 'SmtpPassword' is not valid !");

                //overime meno odosielatela
                if (String.IsNullOrEmpty(this.senderName))
                    throw new ArgumentException("Argument 'SenderName' is not valid !");

                //overime rozsah portu
                if (this.smtpPort < IPEndPoint.MinPort || this.smtpPort > IPEndPoint.MaxPort)
                    throw new ArgumentOutOfRangeException("Argument 'SmtpPort' is not valid !");
            }

            //konfiguracia je spravna
            return true;
        }
        #endregion
    }
}

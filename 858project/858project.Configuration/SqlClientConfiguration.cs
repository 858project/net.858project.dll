using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace Project858.Configuration
{
    /// <summary>
    /// Konfiguracia SQL klienta zabezpecujuceho pristup k databaze
    /// </summary>
    [Serializable]
    public class SqlClientConfiguration
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public SqlClientConfiguration()
        {
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje ci je klient zabezpecujuci pristup k SQL potrebny a aktivny
        /// </summary>
        [XmlElement(ElementName = "enable")]
        public Boolean Enable
        {
            get { return enable; }
            set { enable = value; }
        }
        /// <summary>
        /// (Get / Set) Meno servera ku ktoremu sa pripajame
        /// </summary>
        [XmlElement(ElementName = "sqlServer")]
        public String SqlServer
        {
            get
            {
                return ((this.enable) ? this.sqlServer : String.Empty);
            }
            set
            {
                //osetrime argument
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Argument 'SqlServer' is not valid !");

                sqlServer = value;
            }
        }
        /// <summary>
        /// (Get / Set) Databaza nad ktorou pracujeme
        /// </summary>
        [XmlElement(ElementName = "sqlDatabase")]
        public String SqlDatabase
        {
            get
            {
                return ((this.enable) ? this.sqlDatabase : String.Empty);
            }
            set
            {
                //osetrime argument
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Argument 'SqlDatabase' is not valid !");

                sqlDatabase = value;
            }
        }
        /// <summary>
        /// (Get / Set) Login k SQL serveru
        /// </summary>
        [XmlElement(ElementName = "sqlLogin")]
        public String SqlLogin
        {
            get
            {
                return ((this.enable) ? this.sqlLogin : String.Empty);
            }
            set
            {
                //osetrime argument
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Argument 'SqlLogin' is not valid !");

                sqlLogin = value;
            }
        }
        /// <summary>
        /// (Get / Set) Heslo k SQL serveru
        /// </summary>
        [XmlElement(ElementName = "sqlPassword")]
        public String SqlPassword
        {
            get
            {
                return ((this.enable) ? this.sqlPassword : String.Empty);
            }
            set
            {
                //osetrime argument
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Argument 'SqlPassword' is not valid !");

                this.sqlPassword = value;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Definuje ci je klient zabezpecujuci pristup k SQL potrebny a aktivny
        /// </summary>
        private Boolean enable = false;
        /// <summary>
        /// Meno databazy ku ktorej sa pripajame
        /// </summary>
        private String sqlDatabase = null;
        /// <summary>
        /// Login k smtp serveru
        /// </summary>
        private String sqlLogin = null;
        /// <summary>
        /// Heslo k smtp serveru
        /// </summary>
        private String sqlPassword = null;
        /// <summary>
        /// Meno servera ku ktoremu sa pripajame
        /// </summary>
        private String sqlServer = null;
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

            //ak je cklient na pristup k SQL aktivny
            if (this.enable)
            {
                //overime ci je zadana
                if (String.IsNullOrEmpty(this.sqlServer))
                    throw new ArgumentException("Argument 'SqlServer' is not valid !");

                //overime ci je zadany smtp _sqlServer
                if (String.IsNullOrEmpty(this.sqlDatabase))
                    throw new ArgumentException("Argument 'SqlDatabase' is not valid !");

                //overime zadanie loginu
                if (String.IsNullOrEmpty(this.sqlLogin))
                    throw new ArgumentException("Argument 'SqlLogin' is not valid !");

                //overime zadanie hesla
                if (String.IsNullOrEmpty(this.sqlPassword))
                    throw new ArgumentException("Argument 'SqlPassword' is not valid !");

                //overime ci je heslo v subore spravne zacryptovane
                String _password = String.Empty;
                if (!Utility.TryStringDecrypt(this.sqlPassword, cryptoPassword, out _password))
                    throw new CryptographicException("Argument 'SqlPassword' is not valid !");
            }

            //konfiguracia je spravna
            return true;
        }
        #endregion
    }
}

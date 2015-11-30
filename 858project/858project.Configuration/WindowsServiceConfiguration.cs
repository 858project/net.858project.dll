using System;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Project858.ComponentModel.Client;
using Project858.Diagnostics;
using System.Diagnostics;

namespace Project858.Configuration
{
    /// <summary>
    /// Konfiguracia WindowsService so zakladnymi nastaveniami
    /// </summary>
    [Serializable()]
    [XmlRoot("windowsServiceConfiguration")]
    public class WindowsServiceConfiguration
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public WindowsServiceConfiguration()
        {
            this.sqlClientConfiguration = new SqlClientConfiguration();
            this.mailClientConfiguration = new MailClientConfiguration();
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Meno / Popis klienta
        /// </summary>
        [XmlElement(ElementName = "serviceName", IsNullable = false)]
        public String ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }
        /// <summary>
        /// (Get / Set) Popis Windows Service
        /// </summary>
        [XmlElement(ElementName = "description", IsNullable = true)]
        public String Description
        {
            get { return description; }
            set { description = value; }
        }
        /// <summary>
        /// (Get / Set) Definuje typ trace klienta pre service
        /// </summary>
        [XmlElement(ElementName = "traceClientType", IsNullable = false)]
        public TraceClientTypes TraceClientType
        {
            get { return traceClientType; }
            set { traceClientType = value; }
        }
        /// <summary>
        /// (Get / Set) Definuje typ logovania ktore su povolene
        /// </summary>
        [XmlElement(ElementName = "traceType", IsNullable = false)]
        public TraceTypes TraceType
        {
            get { return traceType; }
            set { traceType = value; }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je logovanie chyb zapnute za kazdych okolnosti
        /// </summary>
        [XmlElement(ElementName = "traceErrorAlways", IsNullable = false)]
        public Boolean TraceErrorAlways
        {
            get { return traceErrorAlways; }
            set { traceErrorAlways = value; }
        }
        /// <summary>
        /// (Get / Set) Konfiguracia SQL klienta
        /// </summary>
        [XmlElement(ElementName = "sqlClientConfiguration", IsNullable = false)]
        public SqlClientConfiguration SqlClientConfiguration
        {
          get { return sqlClientConfiguration; }
          set { sqlClientConfiguration = value; }
        }
        /// <summary>
        /// (Get / Set) Konfiguracia Mail klienta
        /// </summary>
        [XmlElement(ElementName = "mailClientConfiguration", IsNullable = false)]
        public MailClientConfiguration MailClientConfiguration
        {
          get { return mailClientConfiguration; }
          set { mailClientConfiguration = value; }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Meno / Popis klienta
        /// </summary>
        private String serviceName = String.Empty;
        /// <summary>
        /// Konfiguracia Mail klienta
        /// </summary>
        private MailClientConfiguration mailClientConfiguration = null;
        /// <summary>
        /// Konfiguracia SQL klienta
        /// </summary>
        private SqlClientConfiguration sqlClientConfiguration = null;
        /// <summary>
        /// Popis Windows Service
        /// </summary>
        private String description = String.Empty;
        /// <summary>
        /// Definuje typ trace klienta pre service
        /// </summary>
        private TraceClientTypes traceClientType = TraceClientTypes.None;
        /// <summary>
        /// Definuje typ logovania ktore su povolene
        /// </summary>
        private TraceTypes traceType = TraceTypes.Off;
        /// <summary>
        /// Definuje ci je logovanie chyb zapnute za kazdych okolnosti
        /// </summary>
        private Boolean traceErrorAlways = false;
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

            //overenie mena klienta
            if (String.IsNullOrEmpty(this.serviceName))
                throw new ArgumentNullException("Argument 'ClientName' is not valid !");

            //overime konfiguracie
            this.sqlClientConfiguration.Validate(cryptoPassword);
            this.mailClientConfiguration.Validate(cryptoPassword);

            //konfiguracia je spravna
            return true;
        }
        #endregion

        #region - Serialization Method -
        /// <summary>
        /// Metoda ktora serializuje konfiguraciu do XML suboru
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <param name="fileName">Meno suboru kam sa konfiguracia uklada</param>
        /// <param name="configuration">Data ktore chceme serializovat</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        public static void XMLSerialize(String fileName, WindowsServiceConfiguration configuration, String cryptoPassword)
        {
            //osetrime vstupne data
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            try
            {
                //serializujeme data
                XmlSerializer serializer = new XmlSerializer(typeof(WindowsServiceConfiguration));
                TextWriter tw = new StreamWriter(fileName);
                //odstranime xml schcemu pomocou namespaces
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(String.Empty, String.Empty);
                serializer.Serialize(tw, configuration, namespaces);
                tw.Close();
            }
            catch (Exception ex)
            {
                //zalogujeme
                ConsoleLogger.WriteLine("Chyba pri XML serializacii. {0}", ex.Message);
#if DEBUG
                Debug.WriteLine(ex);
#endif
                //preposleme vynimku
                throw;
            }
        }
        /// <summary>
        /// Metoda ktora serializuje konfiguraciu do XML suboru
        /// </summary>
        /// <param name="fileName">Meno suboru kam sa konfiguracia uklada</param>
        /// <param name="configuration">Data ktore chceme serializovat</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        /// <returns>True = serializacia bola uspesna</returns>
        public static Boolean TryXMLSerialize(String fileName, WindowsServiceConfiguration configuration, String cryptoPassword)
        {
            try
            {
                //serializujeme konfiguraciu
                WindowsServiceConfiguration.XMLSerialize(fileName, configuration, cryptoPassword);

                //serializacia bola uspesna
                return true;
            }
            catch (Exception)
            {
                //serializacia zlyhala
                return false;
            }
        }
        /// <summary>
        /// Metoda ktora deserializuje konfiguraciu z XML suboru
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Zadany subor neexistuje
        /// </exception>
        /// <param name="fileName">Meno suboru odkial chceme data deserializovat</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        /// <returns>Deserializovana konfiguracia</returns>
        public static WindowsServiceConfiguration XMLDeserialize(String fileName, String cryptoPassword)
        {
            //osetrime vstupny argument
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            //overime existenciu suboru
            if (!File.Exists(fileName))
            {
                //zalogujeme
                ConsoleLogger.WriteLine("Konfiguracny subor CONFIG neexistuje !");

                //vynimka oznamujuca neexistujuci subor
                throw new FileNotFoundException(String.Format("File does not exist: {0}.", fileName));
            }

            try
            {
                //pomocna premenna
                WindowsServiceConfiguration configuration = null;

                //pokus o deserializaciu dat
                XmlSerializer serializer = new XmlSerializer(typeof(WindowsServiceConfiguration));
                TextReader tr = new StreamReader(fileName);
                configuration = (WindowsServiceConfiguration)serializer.Deserialize(tr);
                tr.Close();

                //overime konfiguraciu
                configuration.Validate(cryptoPassword);

                //vratime nacitanu konfiguraciu
                return configuration;
            }
            catch (Exception ex)
            {
#if DEBUG
                //zalogujeme
                ConsoleLogger.WriteLine("Chyba pri XML deserializacii. {0}", ex);
#endif
                //preposleme vynimku
                throw ex;
            }
        }
        /// <summary>
        /// Metoda ktora deserializuje konfiguraciu z XML suboru
        /// </summary>
        /// <param name="fileName">Meno suboru kam sa konfiguracia uklada</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        /// <param name="configuration">Data ktore chceme serializovat</param>
        /// <returns>True = serializacia bola uspesna</returns>
        public static Boolean TryXMLDeserialize(String fileName, String cryptoPassword, out WindowsServiceConfiguration configuration)
        {
            //zabezpecime out
            configuration = null;

            try
            {
                //deserializujeme konfiguraciu
                configuration = WindowsServiceConfiguration.XMLDeserialize(fileName, cryptoPassword);

                //serializacia bola uspesna
                return true;
            }
            catch (Exception)
            {
                //serializacia zlyhala
                return false;
            }
        }
        /// <summary>
        /// Metoda ktora serializuje konfiguraciu do binary suboru
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <param name="fileName">Meno suboru kam sa konfiguracia uklada</param>
        /// <param name="configuration">Data ktore chceme serializovat</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        public static void BinarySerialize(String fileName, WindowsServiceConfiguration configuration, String cryptoPassword)
        {
            //osetrime vstupne data
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            try
            {
                //serializujeme data
                Stream stream = File.Create(fileName);
                BinaryFormatter binaryWrite = new BinaryFormatter();
                binaryWrite.Serialize(stream, configuration);
                stream.Close();
            }
            catch (Exception ex)
            {
                //zalogujeme
                ConsoleLogger.WriteLine("Chyba pri Binary serializacii. {0}", ex);
                //preposleme vynimku
                throw;
            }
        }
        /// <summary>
        /// Metoda ktora serializuje konfiguraciu do binary suboru
        /// </summary>
        /// <param name="fileName">Meno suboru kam sa konfiguracia uklada</param>
        /// <param name="configuration">Data ktore chceme serializovat</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        /// <returns>True = serializacia bola uspesna</returns>
        public static Boolean TryBinarySerialize(String fileName, WindowsServiceConfiguration configuration, String cryptoPassword)
        {
            try
            {
                //serializujeme konfiguraciu
                WindowsServiceConfiguration.BinarySerialize(fileName, configuration, cryptoPassword);

                //serializacia bola uspesna
                return true;
            }
            catch (Exception)
            {
                //serializacia zlyhala
                return false;
            }
        }
        /// <summary>
        /// Metoda ktora deserializuje konfiguraciu z binary suboru
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Zadany subor neexistuje
        /// </exception>
        /// <param name="fileName">Meno suboru odkial chceme data deserializovat</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        /// <returns>Deserializovana konfiguracia</returns>
        public static WindowsServiceConfiguration BinaryDeserialize(String fileName, String cryptoPassword)
        {
            //osetrime vstupny argument
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            //overime existenciu suboru
            if (!File.Exists(fileName))
            {
                //zalogujeme
                ConsoleLogger.WriteLine("Konfiguracny subor BIN neexistuje !");

                //vynimka oznamujuca neexistujuci subor
                throw new FileNotFoundException(String.Format("File does not exist: {0}.", fileName));
            }

            try
            {
                //pomocna premenna
                WindowsServiceConfiguration configuration = null;

                //pokus o deserializaciu dat
                BinaryFormatter binaryRead = new BinaryFormatter();
                binaryRead.Binder = new Version1ToVersion2DeserializationBinder();
                Stream stream = File.OpenRead(fileName);
                configuration = (WindowsServiceConfiguration)binaryRead.Deserialize(stream);
                stream.Close();

                //overime konfiguraciu
                configuration.Validate(cryptoPassword);

                //vratime nacitanu konfiguraciu
                return configuration;
            }
            catch (Exception ex)
            {
#if DEBUG
                //zalogujeme
                ConsoleLogger.WriteLine("Chyba pri Binary deserializacii. {0}", ex);
#endif
                //preposleme vynimku
                throw ex;
            }
        }
        /// <summary>
        /// Metoda ktora deserializuje konfiguraciu z binary suboru
        /// </summary>
        /// <param name="fileName">Meno suboru kam sa konfiguracia uklada</param>
        /// <param name="cryptoPassword">Heslo na cryptovanie prihlasovacich hesiel</param>
        /// <param name="configuration">Data ktore chceme serializovat</param>
        /// <returns>True = serializacia bola uspesna</returns>
        public static Boolean TryBinaryDeserialize(String fileName, String cryptoPassword, out WindowsServiceConfiguration configuration)
        {
            //zabezpecime out
            configuration = null;

            try
            {
                //deserializujeme konfiguraciu
                configuration = WindowsServiceConfiguration.BinaryDeserialize(fileName, cryptoPassword);

                //serializacia bola uspesna
                return true;
            }
            catch (Exception)
            {
                //serializacia zlyhala
                return false;
            }
        }
        #endregion
    }
    /// <summary>
    /// Pomocna trieda pre BinarySerializer pomocou ktorej ignorujeme verziu assembly v ninarnom subore
    /// </summary>
    internal sealed class Version1ToVersion2DeserializationBinder : SerializationBinder
    {
        #region - Method -
        /// <summary>
        /// Vrati typ na ktory sa pretypovava serializovany objekt
        /// </summary>
        /// <param name="assemblyName">Meno assembly</param>
        /// <param name="typeName">Meno povodneho typu</param>
        /// <returns>Typ</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Get the type using the typeName and assemblyName
            return Type.GetType(typeName);
        }
        #endregion
    }
}

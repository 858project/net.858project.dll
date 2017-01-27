using Project858.Diagnostics;
using Project858.ComponentModel.Client;
using System;
using System.IO.Ports;

namespace Project858.IO.Ports
{
    /// <summary>
    /// Zakladna komunikacna vrstva, pracujuca na linkovej urovni seriovej linky
    /// </summary>
    public class SerialPortClient : ClientBase
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        public SerialPortClient(String portName) :
            this(portName, 9600, Parity.None, 8, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortClient(String portName, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, 9600, Parity.None, 8, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentu baudRate
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        public SerialPortClient(String portName, Int32 baudRate) :
            this(portName, baudRate, Parity.None, 8, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortClient(String portName, Int32 baudRate, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, baudRate, Parity.None, 8, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentu baudRate 
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        public SerialPortClient(String portName, Int32 baudRate, Parity parity) :
            this(portName, baudRate, parity, 8, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortClient(String portName, Int32 baudRate, Parity parity, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, baudRate, parity, 8, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        public SerialPortClient(String portName, Int32 baudRate, Parity parity, int dataBits) :
            this(portName, baudRate, parity, dataBits, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortClient(String portName, Int32 baudRate, Parity parity, int dataBits, Int32 readTimeout, Int32 writeTimeout) :
            this(portName, baudRate, parity, dataBits, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        /// <param name="stopBits">StopBits</param>
        public SerialPortClient(String portName, Int32 baudRate, Parity parity, Int32 dataBits, StopBits stopBits) :
            this(portName, baudRate, parity, dataBits, stopBits, 1000, 2000)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        /// <param name="stopBits">StopBits</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortClient(String portName, Int32 baudRate, Parity parity, int dataBits, StopBits stopBits, Int32 readTimeout, Int32 writeTimeout)
        {
            //osetrenie portu
            if (String.IsNullOrEmpty(portName) || portName.StartsWith("\\\\"))
                throw new ArgumentException("portName");

            //osetrenie baudRate
            if (baudRate <= 0)
                throw new ArgumentOutOfRangeException("baudRate must be > 0.");

            //osetrenie dataBits
            if (dataBits < 5 || dataBits > 8)
                throw new ArgumentOutOfRangeException("dataBits must be between 6 and 8.");

            //osetrenie read timeoutu
            if (readTimeout <= 0 && readTimeout != SerialPort.InfiniteTimeout)
                throw new ArgumentOutOfRangeException("readTimeout must be > 0");

            //osetrenie write timeoutu
            if (writeTimeout <= 0 && writeTimeout != SerialPort.InfiniteTimeout)
                throw new ArgumentOutOfRangeException("writeTimeout must be > 0.");

            this.port = null;
            this.portName = portName;
            this.baudRate = baudRate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;
            this.readTimeout = readTimeout;
            this.writeTimeout = writeTimeout;
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="port">Port, cez ktory sa bude komunikovat</param>
        public SerialPortClient(SerialPort port)
        {
            //osetrenie portu
            if (port == null)
                throw new ArgumentException("port null");

            //osetrenie baudRate
            if (port.BaudRate <= 0)
                throw new ArgumentOutOfRangeException("baudRate must be > 0");

            //osetrenie dataBits
            if (port.DataBits < 5 || port.DataBits > 8)
                throw new ArgumentOutOfRangeException("dataBits must be between 6 and 8.");

            //osetrenie read timeoutu
            if (port.ReadTimeout <= 0 && port.ReadTimeout != SerialPort.InfiniteTimeout)
                throw new ArgumentOutOfRangeException("readTimeout must be > 0.");

            //osetrenie write timeoutu
            if (port.WriteTimeout <= 0 && port.WriteTimeout != SerialPort.InfiniteTimeout)
                throw new ArgumentOutOfRangeException("writeTimeout must be > 0.");

            this.port = port;
        }

        #endregion

        #region - Event -
        /// <summary>
        /// Event na oznamenie spustenia spojenia pre vyssu vrstvu
        /// </summary>
        private event EventHandler transportConnected = null;
        /// <summary>
        /// Event na oznamenie spustenia spojenia pre vyssu vrstvu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public event EventHandler ConnectedEvent
        {
            add
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    lock (this.eventLock)
                        this.transportConnected += value;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
            remove
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    lock (this.eventLock)
                        this.transportConnected -= value;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// Event na oznamenie ukoncenia spojenia pre vyssu vrstvu
        /// </summary>
        private event EventHandler transportDisconnected = null;
        /// <summary>
        /// Event na oznamenie ukoncenia spojenia pre vyssu vrstvu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public event EventHandler TransportDisconnected
        {
            add
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    lock (this.eventLock)
                        this.transportDisconnected += value;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
            remove
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    lock (this.eventLock)
                        this.transportDisconnected -= value;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// Event na oznamenue prichodu dat na transportnej vrstve
        /// </summary>
        private event DataEventHandler transportDataReceived = null;
        /// <summary>
        /// Event na oznamenue prichodu dat na transportnej vrstve
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public event DataEventHandler TransportDataReceived
        {
            add
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    lock (this.eventLock)
                        this.transportDataReceived += value;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
            remove
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    //netusim preco ale to mi zvykne zamrznut thread a ja fakt neviem preco
                    lock (this.eventLock)
                        this.transportDataReceived -= value;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Detekcia pripojenia
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public Boolean IsConnected
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.isConnected;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get) BaudRate
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public Int32 BaudRate
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.baudRate;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get) DataBits
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public int DataBits
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.dataBits;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get / Set) Handshake
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public Handshake Handshake
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.handshake;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
            set
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    this.handshake = value;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get) Parity
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public Parity Parity
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.parity;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get) Meno COM portu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public String PortName
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.portName;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get) Timeout na citanie z portu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public int ReadTimeout
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.readTimeout;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get) StopBits
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public StopBits StopBits
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.stopBits;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        /// <summary>
        /// (Get) Timeout na zapis na port
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        public int WriteTimeout
        {
            get
            {
                try
                {
                    //je objekt disposed ?
                    if (this.disposed)
                        throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                    return this.writeTimeout;
                }
                catch (Exception ex)
                {
                    TraceLogger.Info(ex.ToString());
                    throw;
                }
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Handshake
        /// </summary>
        private Handshake handshake = Handshake.None;
        /// <summary>
        /// Timeout na zapis na port
        /// </summary>
        private Int32 writeTimeout = SerialPort.InfiniteTimeout;
        /// <summary>
        /// Timeout na citanie z portu
        /// </summary>
        private Int32 readTimeout = SerialPort.InfiniteTimeout;
        /// <summary>
        /// StopBits
        /// </summary>
        private StopBits stopBits = StopBits.None;
        /// <summary>
        /// Data Bits
        /// </summary>
        private Int32 dataBits = 0;
        /// <summary>
        /// Parity
        /// </summary>
        private Parity parity = Parity.None;
        /// <summary>
        /// Prenosova rychlost
        /// </summary>
        private Int32 baudRate = 0;
        /// <summary>
        /// Meno COM portu cez ktory komunikujeme
        /// </summary>
        private String portName = String.Empty;
        /// <summary>
        /// Detekuje ci je funkcia vrstvy spustena
        /// </summary>
        private volatile Boolean isRun = false;
        /// <summary>
        /// Detekuje ci logovania errorov pozadovane stale
        /// </summary>
        private Boolean traceErrorAlways = false;
        /// <summary>
        /// Typ logovania ktory je nastaveny
        /// </summary>
        private TraceTypes traceType = TraceTypes.Off;
        /// <summary>
        /// Pomocny objekt na synchronizaciu pristupu k eventom
        /// </summary>
        private readonly Object eventLock = new Object();
        /// <summary>
        /// detekcia pripojenia
        /// </summary>
        private bool isConnected = false;
        /// <summary>
        /// Track if dispose has been called
        /// </summary>
        private Boolean disposed = false;
        /// <summary>
        /// Seriovy port cez ktory komunikujeme
        /// </summary>
        private SerialPort port = null;
        #endregion
   
        #region - Private Method -
        /// <summary>
        /// This function is raised when client is starting
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        /// <returns>NotImplementedException</returns>
        protected override Boolean InternalStart()
        {
            return this.InternalInitialize();
        }
        /// <summary>
        /// This function is raised when client is stoping
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected override void InternalStop()
        {
            this.InternalDeinitialize();
        }
        /// <summary>
        /// This function is raised when client is pausing
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected override void InternalPause()
        {

        }
        /// <summary>
        /// This function is raised when client is disposing
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected override void InternalDispose()
        {

        }
        /// <summary>
        /// Podla inicializacia otvori pozadovany COM port
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Osetrenie, pripadu nastavenie spojenia an typ none
        /// </exception>
        /// <returns>True = uspesne otvorene spojenie, False = chyba pri otvarani spojenia</returns>
        private Boolean InternalInitialize()
        {
            //zalogujeme
            this.InternalTrace(TraceTypes.Info, "Inicializacia ComPortClient....");

            try
            {
                if (port == null)
                {
                    //inicializujeme port
                    this.port = new SerialPort(this.portName);
                    this.port.BaudRate = this.baudRate;
                    this.port.Parity = this.parity;
                    this.port.DataBits = this.dataBits;
                    this.port.StopBits = this.stopBits;
                    this.port.WriteTimeout = this.writeTimeout;
                    this.port.ReadTimeout = this.readTimeout;
                }

                //otvorime port
                if (!this.port.IsOpen)
                    this.port.Open();

                //vyprazdnime buffre
                this.port.DiscardInBuffer();
                this.port.DiscardOutBuffer();

                //asynchronne citanie z portu
                this.port.DataReceived += new SerialDataReceivedEventHandler(this.port_DataReceived);

                //nastavime detekciu spjenia
                this.isConnected = true;
                this.OnConnected(new EventArgs());

                //uspesne ukoncenie metody
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri inicializacii ComPortClient. {0}", ex.Message);

                //nastavime detekciu spjenia
                this.isConnected = false;
                this.isRun = false;

                //preposleme vynimku
                return false;
            }
        }
        /// <summary>
        /// Zatvori spojenie
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        /// <returns>True = uspesne zatvorene spojenie, False = chyba pri zatvarani spojenia</returns>
        public void InternalDeinitialize()
        {

            try
            {

                //toto volanie eventu len ak je volana metoda stupo pocas behu
                if (this.isConnected)
                {
                    //nastavime detekciu spojenia
                    this.isConnected = false;

                    //event o ukonceni spojenia
                    this.OnDisconnected(new EventArgs());
                }

                //ak je port otvoreny tak ho ukoncime
                if (this.port != null)
                {
                    //odstranime event oznnamujuci prijatie dat
                    this.port.DataReceived -= new SerialDataReceivedEventHandler(this.port_DataReceived);
                    //ak je port ukonceny tak ho otvorime
                    if (this.port.IsOpen)
                        this.port.Close();
                }
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, ex.ToString());
            }
        }
        /// <summary>
        /// Zapise data na komunikacnu linku
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Vynimka v pripade ze sa snazime zapisat data, ale spojenie nie je.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave disposed
        /// </exception>
        /// <returns>True = data boli uspesne zapisane, False = chyba pri zapise dat</returns>
        public Boolean Write(Byte[] data)
        {
            try
            {
                //je objekt disposed ?
                if (this.disposed)
                    throw new ObjectDisposedException("ComPortClient", "Object was disposed.");

                //otvorenie nie je mozne ak je connection == true
                if (!this.isConnected)
                    throw new InvalidOperationException("Zapis dat nie je mozny ! Spojenie nie je !");

                try
                {
                    //len ak je port inicializovany a ak je otvoreny
                    if (this.port != null && this.port.IsOpen)
                    {
                        //zapiseme data na port
                        this.port.Write(data, 0, data.Length);

                        //zalogujeme prijate dat
                        this.InternalTrace(TraceTypes.Verbose, "Send data: [{0}]", this.GetStringData(data));
                    }
                    else
                    {
                        //vynimka
                        throw new Exception("Chyba pri zapise na port. SerialPort nie je !");
                    }

                    //uspesne ukoncenie metody
                    return true;
                }
                catch (Exception ex)
                {
                    //spojenie uz nie je aktivne
                    this.isConnected = false;
                    this.isRun = false;

                    //zalogujeme
                    this.InternalTrace(TraceTypes.Error, ex.ToString());

                    //chybne ukoncenie metody
                    return false;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Info(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Metoda obsluhujuca event oznamujuci prichod dat na port
        /// </summary>
        /// <param name="sender">Odosielatel udalosti</param>
        /// <param name="e">SerialDataReceivedEventArgs</param>
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //synchronny pristup k metode
            lock (this)
            {
                try
                {
                    //len ak port existuje
                    if (this.port == null)
                    {
                        //vynimka oznamujuca nedostupnost portu
                        throw new Exception("SerialPort uz nie je dostupny");
                    }
                    else if (!this.port.IsOpen)
                    {
                        //vynimka oznamujuca nedostupnost portu
                        throw new Exception("SerialPort je zatvoreny");
                    }
                    else
                    {
                        //je nieco na citanie ???
                        if (this.port.BytesToRead != 0)
                        {
                            //pocet bytov ktore su dostupne na citanie
                            Int32 count = this.port.BytesToRead;

                            //inicializujeme referenciu na pozadovanu dlzku
                            Byte[] data = new byte[count];

                            //vycitame data
                            this.port.Read(data, 0, count);

                            //zalogujeme prijate dat
                            this.InternalTrace(TraceTypes.Verbose, "Received data: [{0}]", this.GetStringData(data));

                            //vytvorime udalost a posleme data
                            if (this.IsRun)
                            {
                                this.OnReceivedData(new DataEventArgs(data, this.PortName));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //spojenie je ukoncene
                    this.isRun = false;
                    this.isConnected = false;

                    //zalogujeme prijate dat
                    this.InternalTrace(TraceTypes.Error, "Chyba pri citani dat z portu. {0}", ex.Message);

                    //error comunication
                    this.OnDisconnected(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Vrati string dat ktore sa prijali alebo odoslali
        /// </summary>
        /// <param name="data">Data ktore chceme spracovat</param>
        /// <returns>String dat</returns>
        private String GetStringData(Byte[] data)
        {
            try
            {
                //pomocny string na ulozenie obrazu dat
                String str = String.Empty;

                if (data != null)
                {
                    //prejdeme vsetky byty a urobime z nich string
                    foreach (Byte b in data)
                        str += String.Format("{0:X2} ", b);

                    //odstranime posledny prazdny znak
                    str = str.Remove(str.Length - 1, 1);
                }

                //vratime data
                return str;
            }
            catch (Exception ex)
            {
                TraceLogger.Info(ex.ToString());
                throw;
            }
        }
        #endregion

        #region - Call Event Method -
        /// <summary>
        /// Virtualna metoda na obsluhu eventu (posielanie dat do vyssej urovne)
        /// </summary>
        /// <param name="e">EventArgs obsahujuci data</param>
        protected virtual void OnReceivedData(DataEventArgs e)
        {
            DataEventHandler handler = this.transportDataReceived;

            if (handler != null)
                handler.BeginInvoke(this, e, null, null);
        }
        /// <summary>
        /// Trieda generujuca event na oznamenie vytvorenia spojenia
        /// </summary>
        /// <param name="e">EventArgs</param>
        private void OnConnected(EventArgs e)
        {
            EventHandler handler = this.transportConnected;

            if (handler != null)
                handler.BeginInvoke(this, e, null, null);
        }
        /// <summary>
        /// Trieda generujuca event na oznamenie padu spojenia
        /// </summary>
        /// <param name="e">EventArgs</param>
        private void OnDisconnected(EventArgs e)
        {
            EventHandler handler = this.transportDisconnected;

            if (handler != null)
                handler.BeginInvoke(this, e, null, null);
        }
        #endregion

        #region - Public Override Method -
        /// <summary>
        /// Vrati meno triedy
        /// </summary>
        /// <returns>Meno triedy</returns>
        public override string ToString()
        {
            return "ComPortClient";
        }
        #endregion
    }
}

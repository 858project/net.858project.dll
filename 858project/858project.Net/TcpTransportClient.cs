using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Project858.Diagnostics;
using Project858.ComponentModel.Client;
using System.ComponentModel;

namespace Project858.Net
{
	/// <summary>
	/// Zakladna komunikacna vrstva, pracujuca na linkovej urovni TCP
	/// </summary>
	public class TcpTransportClient : ClientBase, ITransportClient
	{
		#region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovana IPAddress
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybny argument / rozsah IP portu
        /// </exception>
        /// <param name="ipEndPoint">IpPoint ku ktoremu sa chceme pripojit</param>
        public TcpTransportClient(IPEndPoint ipEndPoint)
            : this(new IpEndPointCollection(ipEndPoint), 1000, 1000, 1000, 300)
        {

        }
		/// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovana IPAddress
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument / rozsah IP portu
		/// </exception>
        /// <param name="ipEndPoints">Kolekcia IpEndPointov ku ktory sa bude klient striedavo pripajat pri autoreconnecte</param>
        public TcpTransportClient(IpEndPointCollection ipEndPoints)
            : this(ipEndPoints, 1000, 1000, 1000, 300)
		{

		}
        /// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovana IPAddress
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov, alebo IP portu
		/// </exception>
        /// <param name="ipEndPoint">IpPoint ku ktoremu sa chceme pripojit</param>
		/// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
		/// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
		/// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
		/// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
        public TcpTransportClient(IPEndPoint ipEndPoint, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
            : this(new IpEndPointCollection(ipEndPoint), tcpReadTimeout, tcpWriteTimeout, nsReadTimeout, nsWriteTimeout)
        {

        }
		/// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovana IPAddress
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov, alebo IP portu
		/// </exception>
        /// <param name="ipEndPoints">Kolekcia IpEndPointov ku ktory sa bude klient striedavo pripajat pri autoreconnecte</param>
		/// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
		/// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
		/// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
		/// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
		public TcpTransportClient(IpEndPointCollection ipEndPoints, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
		{
            //osetrime vstup
            if (ipEndPoints == null)
                throw new ArgumentNullException("ipEndPoints");
            if (ipEndPoints.Count == 0)
                throw new ArgumentException("ipEndPoints.Count can not be 0 !");
			if (tcpReadTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpReadTimeout must be >= -1");
			if (tcpWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpWriteTimeout must be >= -1");
			if (nsReadTimeout < -1)
				throw new ArgumentOutOfRangeException("nsReadTimeout must be >= -1");
			if (nsWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("nsWriteTimeout must be >= -1");

			//nastavime timeoty
			this.m_tcpReadTimeout = tcpReadTimeout;
			this.m_tcpWriteTimeout = tcpWriteTimeout;
			this.m_nsReadTimeout = nsReadTimeout;
			this.m_nsWriteTimeout = nsWriteTimeout;
			this.m_ipEndPoints = ipEndPoints;
            this.m_tcpClient = null;
			this.m_canAutoReconnect = true;
		}
		/// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovany TcpClient
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		/// Ak je RemoteEndPoint v tcpClientovi Disposed
		/// </exception>
		/// <param name="tcpClient">Client zabezpecujuci tcp komunikaciu</param>
		public TcpTransportClient(TcpClient tcpClient)
			: this(tcpClient, 1000, 1000, 1000, 300)
		{
		}
		/// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovany TcpClient
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		/// Ak je RemoteEndPoint v tcpClientovi Disposed
		/// </exception>
		/// <param name="tcpClient">Client zabezpecujuci tcp komunikaciu</param>
		/// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
		/// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
		/// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
		/// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
		public TcpTransportClient(TcpClient tcpClient, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
		{
			if (tcpClient == null)
				throw new ArgumentNullException("tcpClient", "Value cannot be null.");
			if (tcpReadTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpReadTimeout must be >= -1");
			if (tcpWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("tcpWriteTimeout must be >= -1");
			if (nsReadTimeout < -1)
				throw new ArgumentOutOfRangeException("nsReadTimeout must be >= -1");
			if (nsWriteTimeout < -1)
				throw new ArgumentOutOfRangeException("nsWriteTimeout must be >= -1");

			//nastavime timeoty
			this.m_tcpReadTimeout = tcpReadTimeout;
			this.m_tcpWriteTimeout = tcpWriteTimeout;
			this.m_nsReadTimeout = nsReadTimeout;
			this.m_nsWriteTimeout = nsWriteTimeout;
			this.m_tcpClient = tcpClient;
			this.m_ipEndPoints = null;
			this.m_canAutoReconnect = false;
		}
		#endregion

		#region - State Class -
		/// <summary>
		/// pomocna trieda na event na streame
		/// </summary>
		private class SocketState
		{
			#region - Constructor -
			/// <summary>
			/// Initialize this class
			/// </summary>
			public SocketState()
			{
				this._data = new byte[65536];
			}
			#endregion

			#region - Properties -
            /// <summary>
            /// Klient cez ktoreho prebieha komunikacia
            /// </summary>
            public TcpClient Client { get; set; }
			/// <summary>
			/// Buffer na citanie _data
			/// </summary>
			public Byte[] Data
			{
				get { return _data; }
				set { _data = value; }
			}
			/// <summary>
			/// Stream cez ktory sa komunikuje
			/// </summary>
			public NetworkStream Stream
			{
				get { return _stream; }
				set { _stream = value; }
			}
			#endregion

			#region - Variable -
			/// <summary>
			/// Buffer na citanie _data
			/// </summary>
			private Byte[] _data = null;
			/// <summary>
			/// Stream cez ktory sa komunikuje
			/// </summary>
			private NetworkStream _stream = null;
			#endregion
		}
		#endregion

		#region - Event -
		/// <summary>
		/// Event na oznamenie spustenia spojenia pre vyssu vrstvu
		/// </summary>
		private event EventHandler _connectedEvent = null;
		/// <summary>
		/// Event na oznamenie spustenia spojenia pre vyssu vrstvu
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public event EventHandler ConnectedEvent
		{
			add
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				lock (this.m_eventLock)
					this._connectedEvent += value;
			}
			remove
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
					this._connectedEvent -= value;
			}
		}
		/// <summary>
		/// Event na oznamenie ukoncenia spojenia pre vyssu vrstvu
		/// </summary>
		private event EventHandler disconnectedEvent = null;
		/// <summary>
		/// Event na oznamenie ukoncenia spojenia pre vyssu vrstvu
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public event EventHandler DisconnectedEvent
		{
			add
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
					this.disconnectedEvent += value;
			}
			remove
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
					this.disconnectedEvent -= value;
			}
		}
		/// <summary>
		/// Event na oznamenue prichodu dat na transportnej vrstve
		/// </summary>
		private event DataEventHandler m_receivedDataEvent = null;
		/// <summary>
		/// Event na oznamenue prichodu dat na transportnej vrstve
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
        public event DataEventHandler ReceivedDataEvent
		{
			add
			{

				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				lock (this.m_eventLock)
					this.m_receivedDataEvent += value;
			}
			remove
			{

				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				//netusim preco ale to mi zvykne zamrznut thread a ja fakt neviem preco
                lock (this.m_eventLock)
					this.m_receivedDataEvent -= value;
			}
		}
		/// <summary>
		/// Event oznamujuci zmenu stavu pripojenia
		/// </summary>
		private event EventHandler _changeConnectionStateEvent = null;
		/// <summary>
		/// Event oznamujuci zmenu stavu pripojenia
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Ak nie je autoreconnect povoleny
		/// </exception>
		public event EventHandler ChangeConnectionStateEvent
		{
			add
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");
				//ak je povolena moznost reconnectu
				if (!this.m_canAutoReconnect)
					throw new InvalidOperationException();

                lock (this.m_eventLock)
					this._changeConnectionStateEvent += value;
			}
			remove
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");
				//ak je povolena moznost reconnectu
				if (!this.m_canAutoReconnect)
					throw new InvalidOperationException();

                lock (this.m_eventLock)
					this._changeConnectionStateEvent -= value;
			}
		}
		#endregion

		#region - Properties -
		/// <summary>
		/// (Get / Set) Definuje ci je zapnute automaticke pripajanie klienta
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave isDisposed
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Ak nie je autoreconnect povoleny
		/// </exception>
		public Boolean AutoReconnect
		{
			get
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				return  (this.m_canAutoReconnect) ? m_autoReconnect : false;
			}
			set
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");
				//ak je povolena moznost reconnectu
				if (!this.m_canAutoReconnect)
					throw new InvalidOperationException();

				m_autoReconnect = (this.m_canAutoReconnect) ? value : false;
			}
		}
		/// <summary>
		/// (Get / Set) Stav v akom sa nachadza komunikacia / komunikacny kanal
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public ConnectionStates ConnectionState
		{
			get
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				return m_connectionState;
			}
		}
		/// <summary>
		/// (Get / Set) Timeout v akom sa pravidelne vykonava pokus o pripojenie. Minute = 2000, Max = 60000.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Nepovoleny rozsah timeotu
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Ak nie je autoreconnect povoleny
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public Int32 ReconnectTimeout
		{
			get
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				return this.m_autoReconnectTimeout;
			}
			set
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");
				//ak je povolena moznost reconnectu
				if (!this.m_canAutoReconnect)
					throw new InvalidOperationException();

				//overime rozsah timeoutu
				if (value < 2000 || value > 60000)
					throw new ArgumentOutOfRangeException("value");

				m_autoReconnectTimeout = value;
			}
		}
		/// <summary>
		/// Kolekcia IPEndPointov ku ktorym sa klient pri reconnecte pripaja
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public IpEndPointCollection IpEndPoints
		{
			get
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				return this.m_ipEndPoints;
			}
		}
		/// <summary>
		/// (Get) Detekcia pripojenia
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public Boolean IsConnected
		{
			get
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

				return this.m_isConnectied;
			}
		}
		/// <summary>
		/// Definuje ci je mozne vyuzit auto recconect spojenia
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		public Boolean CanAutoReconnect
		{
			get {
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");
				
				return m_canAutoReconnect;
			}
			protected set {
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");
				
				m_canAutoReconnect = value;
			}
		}
        /// <summary>
        /// Aktualny ip end point na ktory sa pripajame alebo na ktorom sme pripojeny
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        public IPEndPoint ActualIpEndPoint
        {
            get {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed"); 
                
                return m_actualIpEndPoint;
            }
            set {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed"); 
                
                m_actualIpEndPoint = value;
            }
        }
		#endregion

		#region - Variable -
        /// <summary>
        /// Index posledneho IpEndPointu na ktory sa pokusilo pripojit
        /// </summary>
        private int m_lastIpEndPointIndex = 0;
		/// <summary>
		/// Definuje ci je mozne vyuzit auto recconect spojenia
		/// </summary>
		private Boolean m_canAutoReconnect = false;
		/// <summary>
		/// Stream na komunikaciu
		/// </summary>
		private NetworkStream m_networkStream = null;
		/// <summary>
		/// TcpClient Read Timeout
		/// </summary>
		private Int32 m_tcpReadTimeout = 0;
		/// <summary>
		/// TcpClient Write Timeout
		/// </summary>
		private Int32 m_tcpWriteTimeout = 0;
		/// <summary>
		/// NetworkStream Read Timeout
		/// </summary>
		private Int32 m_nsReadTimeout = 0;
		/// <summary>
		/// NetworkStream Write Timeout
		/// </summary>
		private Int32 m_nsWriteTimeout = 0;
		/// <summary>
		/// detekcia pripojenia
		/// </summary>
		private volatile Boolean m_isConnectied = false;
		/// <summary>
		/// Client na pripojenie cez TCP/IP
		/// </summary>
		private TcpClient m_tcpClient;
		/// <summary>
		/// pristupovy bod cez ktory sme pripojeny
		/// </summary>
		private IpEndPointCollection m_ipEndPoints = null;
		/// <summary>
		/// Pomocny timer na vykonavanie automatikeho reconnectu v pripade padu spojenia
		/// </summary>
		private Timer m_autoReconnectTimer = null;
		/// <summary>
		/// Timeout v akom sa pravidelne vykonava pokus o pripojenie
		/// </summary>
		private Int32 m_autoReconnectTimeout = 5000;
		/// <summary>
		/// Definuje ci je zapnute automaticke pripajanie klienta
		/// </summary>
		private Boolean m_autoReconnect = false;
		/// <summary>
		/// Stav v akom sa nachadza komunikacia / komunikacny kanal
		/// </summary>
		private ConnectionStates m_connectionState = ConnectionStates.Closed;
        /// <summary>
        /// Aktualny ip end point na ktory sa pripajame alebo na ktorom sme pripojeny
        /// </summary>
        private IPEndPoint m_actualIpEndPoint = null;
		#endregion

		#region - Public Method -
        /// <summary>
        /// Vrati meno, popis klienta
        /// </summary>
        /// <returns>Popis klienta</returns>
        public override string ToString()
        {
            if (this.m_actualIpEndPoint == null)
            {
                return base.ToString();
            }

            return String.Format("{0}_[{1}]", base.ToString(), this.ActualIpEndPoint.ToString().Replace(":", "_"));
        }
        /// <summary>
        /// This function reads data from stream
        /// </summary>
        /// <param name="buffer">Buffer to insert data</param>
        /// <param name="size">Max size for buffer</param>
        /// <param name="result">Result, data count from stream</param>
        /// <returns>True | false</returns>
        public Boolean Read(Byte[] buffer, int size, out int result)
        {
            //je objekt _isDisposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            //otvorenie nie je mozne ak je connection == true
            if (!this.IsConnected)
                throw new InvalidOperationException("Writing data is not possible! The client is not connected!");

            //reset value
            result = 0x00;

            try
            {
                //zapiseme data
                result = this.m_networkStream.Read(buffer, 0x00, size);

                //successfully
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalException(ex, "Error during reading data to socket. {0}", ex.Message);

                //ukoncime klienta
                this.InternalDisconnect(false);

                //chybne ukoncenie metody
                return false;
            }
        }
        /// <summary>
        /// Zapise _data na komunikacnu linku
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Vynimka v pripade ze sa snazime zapisat _data, ale spojenie nie je.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        /// <returns>True = _data boli uspesne zapisane, False = chyba pri zapise dat</returns>
        public Boolean Write(Byte[] data)
		{
			//je objekt _isDisposed ?
			if (this.IsDisposed)
				throw new ObjectDisposedException("Object was disposed");

			//otvorenie nie je mozne ak je connection == true
			if (!this.IsConnected)
                throw new InvalidOperationException("Writing data is not possible! The client is not connected!");

			try
			{
                //zalogujeme prijate dat
                this.InternalTrace(TraceTypes.Verbose, "Sending data: [{0}]", data.ToHexaString());

				//zapiseme data
				this.m_networkStream.Write(data, 0, data.Length);
				this.m_networkStream.Flush();

                //zalogujeme prijate dat
                this.InternalTrace(TraceTypes.Verbose, "Sending the data has been successfully");

				//uspesne ukoncenie metody
				return true;
			}
			catch (Exception ex)
			{
				//zalogujeme
                this.InternalException(ex, "Error during sending data to socket. {0}", ex.Message);

				//ukoncime klienta
				this.InternalDisconnect(false);

				//chybne ukoncenie metody
				return false;
			}
		}
		#endregion

		#region - Protected and Private Method -
		/// <summary>
		/// Podla inicializacie otvori TCP spojenie na pripojeneho clienta
		/// </summary>
		/// <returns>True = uspesne otvorene spojenie, False = chyba pri otvarani spojenia</returns>
		protected override bool InternalStart()
		{
            //zmenime stav
            this.m_connectionState = ConnectionStates.Connecting;

            //doslo k zmene stavu
            this.OnChangeConnectionState(EventArgs.Empty);

            //pokusime sa vytvorit spojenie
            if (!this.InternalConnect(!this.AutoReconnect))
            {
                if (this.IsRun)
                {
                    //spustime automaticky reconnec
                    this.StartReconnect();
                }
                else
                {
                    //start nebol uspesny
                    return false;
                }
            }

            //spojenie bolo uspesne vytvorene
            return true;
		}
		/// <summary>
		/// Zatvori spojenie
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		/// <returns>True = uspesne zatvorene spojenie, False = chyba pri zatvarani spojenia</returns>
        protected override void InternalStop()
        {
            //ak bol stav connecting tak bol spusteny reconnect
            if (this.m_connectionState == ConnectionStates.Connecting)
                this.StopReconnect();

            //ziskame stav
            Boolean state = this.IsConnected;

            //deinicializacia base
            this.InternalDisconnect(true);

            //ak sme pripojeny
            if (state)
            {
                //spojenie bolo ukoncene
                this.OnDisconnected(EventArgs.Empty);
            }

            //uz nie sme pripojeny
            this.m_actualIpEndPoint = null;

            //spojenie nie ja aktivne
            if (this.m_connectionState != ConnectionStates.Closed)
            {
                //zmena stavu
                this.m_connectionState = ConnectionStates.Closed;
                //event oznamujuci zmenu stavu
                this.OnChangeConnectionState(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Vrati ipEndPoint ku ktoremu sa nasledne klient pokusi pripojit
        /// </summary>
        /// <returns>IpPoint ku ktoremu sa pokusime pripojit</returns>
        private IPEndPoint GetIpEndPoint()
        {
            //ak sme prekrocili index dostupnych poloziek
            if (this.m_lastIpEndPointIndex > this.m_ipEndPoints.Count - 1)
                this.m_lastIpEndPointIndex = 0;

            //ziskame index ipEndPointu
            IPEndPoint ipEndPoint = this.m_ipEndPoints[this.m_lastIpEndPointIndex];

            //dalsi index
            this.m_lastIpEndPointIndex += 1;

            //vratime ipEndPoint na pripojenie
            return ipEndPoint;
        }
        /// <summary>
        /// Vykona interny pokus o pripojenie k vzdialenemu bodu
        /// </summary>
        /// <param name="starting">Definuje ci ide o pripajanie pri starte alebo reconnecte</param>
        /// <returns>True = spojenie bolo uspesne vytvorene</returns>
        private Boolean InternalConnect(Boolean starting)
        {
            try 
            {
				lock (this)
				{
					//pokusime sa vytvorit spojenie
					this.InternalOnConnect();

					//ak bol aktivny reconnect tak ho ukoncime
					this.StopReconnect();

                    //start komunikacie sa podaril
                    this.m_connectionState = ConnectionStates.Connected;

					//spojenie bolo vytvorene
					this.OnConnected(EventArgs.Empty);

					//doslo k zmene stavu
					this.OnChangeConnectionState(EventArgs.Empty);
				}
                
                //inicializacia komunikacie bola uspesna
                return true;
            }
            catch (Exception)
            {
                //ak doslo k chybe pri starte klienta
                if (starting == true)
                {
                    throw;
                }

                //inicializacia nebola uspesna
                return false;
            }
        }
		/// <summary>
		/// Interna snaha o pripojenie
		/// </summary>
		private void InternalOnConnect()
		{
			try
			{
				//zalogujeme
                this.InternalTrace(TraceTypes.Info, "Initializing socket...");

				//ak inicializujeme triedu na zaklade IP a portu
                if (this.m_tcpClient == null)
                {
                    //ziskame ipEndPoint na pripojenie
                    this.m_actualIpEndPoint = this.GetIpEndPoint();

                    //inicializujeme a pripojime klienta
                    this.m_tcpClient = new TcpClient();
                    this.m_tcpClient.Connect(this.m_actualIpEndPoint);
                }
                else
                {
                    //adresa ku ktorej som pripojeny
                    this.m_actualIpEndPoint = (IPEndPoint)this.m_tcpClient.Client.RemoteEndPoint;
                }

                //nastavime timeoty pre socket
                this.m_tcpClient.ReceiveTimeout = this.m_tcpReadTimeout;
                this.m_tcpClient.SendTimeout = this.m_tcpWriteTimeout;

				//ziskame komunikacny stream
				this.m_networkStream = m_tcpClient.GetStream();
                this.m_networkStream.ReadTimeout = this.m_nsReadTimeout;
                this.m_networkStream.WriteTimeout = this.m_nsWriteTimeout;

				//inicializujeme state
				SocketState state = new SocketState()
                {
				    Stream = this.m_networkStream,
                    Client = this.m_tcpClient
                };

				//zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Initialization socket was successful");

				//nastavime detekciu spjenia
				this.m_isConnectied = true;

				//otvorime asynchronne citanie na streame
				this.m_networkStream.BeginRead(state.Data, 0, state.Data.Length, tcp_DataReceived, state);
			}
			catch (Exception ex)
			{
                //zalogujeme
                this.InternalException(ex, "Error during initializing socket. {0}", ex.Message);

				//nastavime detekciu spjenia
				this.m_isConnectied = false;
				this.m_tcpClient = null;
				this.m_networkStream = null;

                //preposleme vynimku
                throw;
			}
		}
        /// <summary>
        /// Interny disconnect
        /// </summary>
        /// <param name="closing">Definuje ci doslo u ukonceniu alebo len preruseniu spojenia</param>
		private void InternalDisconnect(Boolean closing)
		{
            //ukoncime komunikaciu
            this.InternalOnDisconnect();

            //ak doslo len k padu spojenia
            if (this.IsDisposed == false && closing == false)
            {
                //vykoname start recconectu
                this.ValidateConnection();
            }
        }
        /// <summary>
        /// Overi nastavenia triedy. Vykona start automatickehor reconnectu alebo ukonci klienta
        /// </summary>
        private void ValidateConnection()
        {
            //overime ci je mozne spustit reconnect
            if (this.CanAutoReconnect && this.AutoReconnect)
            {
                //event o ukonceni spojenia len ak uz bolo oznamenie o connecte
                if (this.m_connectionState == ConnectionStates.Connected)
                {
                    //doslo k ukonceniu spojenia
                    this.OnDisconnected(EventArgs.Empty);
                }

                //zmenime stav
                this.m_connectionState = ConnectionStates.Connecting;
                //doslo k zmene stavu
                this.OnChangeConnectionState(EventArgs.Empty);
                //spustime automaticky reconnect
                this.StartReconnect();
            }
            else
            {
                //event o ukonceni spojenia len ak uz bolo oznamenie o connecte
                if (this.m_connectionState == ConnectionStates.Connected)
                {
                    //doslo k ukonceniu spojenia
                    this.OnDisconnected(EventArgs.Empty);
                }

                 //zmenime stav
                this.m_connectionState = ConnectionStates.Closed;
                //doslo k zmene stavu
                this.OnChangeConnectionState(EventArgs.Empty);

                //ukoncime klienta
                if (!this.IsDisposed)
                    if (this.IsRun)
                    {
                        //ukoncime klienta
                        this.Stop();
                    }
            }
        }
        /// <summary>
        /// Interny disconnect
        /// </summary>
		private void InternalOnDisconnect()
		{
			try
			{
                //spojenie bolo ukoncene
                this.m_isConnectied = false;

				//ukoncime stream ak je vytvoreny
				if (this.m_networkStream != null)
				{
					this.m_networkStream.Close();
					this.m_networkStream = null;
				}

				//ukoncime tcp spojenie
				if (this.m_tcpClient != null)
				{
					this.m_tcpClient.Close();
					this.m_tcpClient = null;
				}
			}
			catch (Exception)
			{
				//preposleme vynimku vyssie
				throw;
			}
			finally
			{
				//nova inicializaia / deinicializacia premennych
				this.m_isConnectied = false;
				this.m_tcpClient = null;
				this.m_networkStream = null;
			}
		}
		/// <summary>
		/// Calback / prichod dat an streame
		/// </summary>
		/// <param name="ar">IAsyncResult</param>
		private void tcp_DataReceived(IAsyncResult ar)
		{
            try
            {
                this.DataReceived(ar);
            }
            catch (Exception ex)
            {
                //trace message
                this.InternalException(ex, "Error during receiving asynchronous data. {0}", ex.Message);
            }
		}
		/// <summary>
		/// Calback / prichod dat an streame
		/// </summary>
		/// <param name="ar">IAsyncResult</param>
		private void DataReceived(IAsyncResult ar)
		{
			lock (this)
			{
				//inicializacia
				SocketState state = (SocketState)ar.AsyncState;
				NetworkStream stream = state.Stream;
				Int32 r = 0;

				// kontrola ci mozme zo streamu citat
				if (!stream.CanRead)
					return;

				try
				{
					//prerusime asynchronne citanie
					r = stream.EndRead(ar);

					//ak neboli nacitane ziadne _data asi ide o pad spojenia
					if (r == 0)
					{
						//niekedy dochadza k oneskoreniu vlakna zo streamu ktory oznamuje pad spojenia
						if (this.IsDisposed == false)
						{
							//zalogujeme.
                            this.InternalTrace(TraceTypes.Error, "Loss connection with the remote end point!");

							//ukoncime komunikaciu
							this.InternalDisconnect(false);
						}

						//nebolo by vhodne nahodit t stav disconnected ???
						return;
					}
				}
				catch (Exception ex)
				{
					//zalogujeme
                    this.InternalException(ex, "Error during exiting asynchronous reading from the stream. {0}.", ex.Message);

					//ukoncime komunikaciu
					this.InternalDisconnect(false);

					//ukoncime
					return;
				}

				//skopirujeme pole bytov
				Byte[] rdata = new Byte[r];
				Buffer.BlockCopy(state.Data, 0, rdata, 0, r);

				//zalogujeme prijate dat
				//this.InternalTrace(TraceTypes.Verbose, "Received _data: [{0}]", BitConverter.ToString(rdata));
				this.InternalTrace(TraceTypes.Verbose, "Received data: [{0}]", rdata.Length);

				//_data su akeptovane len ak sme pripojeny
				if (this.IsConnected)
				{
					//vytvorime udalost a posleme data
					OnReceivedData(new DataEventArgs(rdata, state.Client.Client.RemoteEndPoint as IPEndPoint));
				}

				try
				{
					//otvorime asynchronne citanie na streame
					stream.BeginRead(state.Data, 0, state.Data.Length, tcp_DataReceived, state);
				}
				catch (Exception ex)
				{
					//zalogujeme
					this.InternalTrace(TraceTypes.Error, "Chyba pri opatovnom spusteni asynchronneho citania zo streamu. {0}", ex.Message);

					//ukoncime
					return;
				}
			}
		}
		/// <summary>
		/// Spusti timer na automatiky reconnect
		/// </summary>
		private void StartReconnect()
		{
            //check reconnect mode
            if (this.IsRun && this.CanAutoReconnect)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Spustanie obsluhy automtickeho reconnectu...");

                //spustime timer
                this.m_autoReconnectTimer = new Timer(new TimerCallback(TimerTick),
                                                 null,
                                                 this.m_autoReconnectTimeout,
                                                 this.m_autoReconnectTimeout);

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Obsluha bola spustena.");
            }
		}
		/// <summary>
		/// Ukonci timer na automaticky reconnect
		/// </summary>
		private void StopReconnect()
		{
            //check reconnect mode
            if (this.CanAutoReconnect)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Ukoncovanie obsluhy automtickeho reconnectu...");

                //ukoncime timer
                if (this.m_autoReconnectTimer != null)
                    this.m_autoReconnectTimer.Dispose();

                //zrusime objekt
                this.m_autoReconnectTimer = null;

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Obsluha bola ukoncena.");
            }
		}
		/// <summary>
		/// Obsluha pre timer vykonavajuci automatiky recconnect
		/// </summary>
		/// <param name="obj">Object</param>
		private void TimerTick(Object obj)
		{
			//overime ci je timer aktivny
			if (this.m_autoReconnectTimer == null || this.IsRun == false)
				return;

			try
			{
				//overime timer
				if (this.m_autoReconnectTimer != null)
					this.m_autoReconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
			}
			catch (ObjectDisposedException)
			{
				//chybu ignorujeme
			}

			try
			{

				//vykoname reconnect
				this.ReconnectProcessing();
			}
			catch (Exception ex)
			{
				//zalogujeme
				this.InternalTrace(TraceTypes.Error, "Chyba pri automatickom reconnecte. {0}", ex.Message);
			}

			try
			{
				//spustime timer na povodnu hodnotu
				if (this.m_autoReconnectTimer != null && this.IsRun)
					this.m_autoReconnectTimer.Change(this.m_autoReconnectTimeout, this.m_autoReconnectTimeout);
			}
			catch (ObjectDisposedException)
			{
				//chybu ignorujeme
			}
		}
		/// <summary>
		/// Vykona automaticky reconnect
		/// </summary>
		private void ReconnectProcessing()
		{
			//obnova len ak nebol klient ukonceny
			if (this.IsRun)
			{
				//zalogujeme
				this.InternalTrace(TraceTypes.Info, "Obnovovanie spojenia....");

					//vykoname obnovu
					if (this.InternalConnect(false))
                    {
                        //inicializacia spojenia bola uspesna
                        this.StopReconnect();
                    }
			}
		}
		#endregion

		#region - Call Event Method -
		/// <summary>
		/// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
		/// oznamujuceho prijatie dat
		/// </summary>
		/// <param name="e">EventArgs obsahujuci data</param>
		protected virtual void OnReceivedData(DataEventArgs e)
		{
			DataEventHandler handler = this.m_receivedDataEvent;

			if (handler != null)
				handler(this, e);
		}
		/// <summary>
		/// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
		/// oznamujuceho pripojenie
		/// </summary>
		/// <param name="e">EventArgs</param>
		protected virtual void OnConnected(EventArgs e)
		{
			EventHandler handler = this._connectedEvent;

			if (handler != null)
				handler(this, e);
		}
		/// <summary>
		/// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
		/// oznamujuceho pad spojenia
		/// </summary>
		/// <param name="e">EventArgs</param>
		protected virtual void OnDisconnected(EventArgs e)
		{
			EventHandler handler = this.disconnectedEvent;

			if (handler != null)
				handler(this, e);
		}
		/// <summary>
		/// Vygeneruje event oznamujui zmenu stavu pripojenia
		/// </summary>
		/// <param name="e">EventArgs</param>
		protected virtual void OnChangeConnectionState(EventArgs e)
		{
			//ziskame pristup
			EventHandler handler = this._changeConnectionStateEvent;

			//vyvolame event
			if (handler != null)
				handler(this, e);
		}
		#endregion
	}
}

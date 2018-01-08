using System;
using System.Collections.Generic;
using System.Text;
using Project858.ComponentModel.Client;
using System.Net;
using System.Net.Sockets;
using Project858.Diagnostics;
using System.ComponentModel;
using System.Threading;

namespace Project858.Net
{
	/// <summary>
	/// Tcp server zabezpecujuci akceptovanie klientov na zvolenej adrese a porte
	/// </summary>
	public abstract class TcpServer : ClientBase
	{
		#region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public TcpServer()
        {
        }
		/// <summary>
		/// Initialize this class
		/// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovany argument
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Rozsah portu mimo povolene hranice
        /// </exception>
		/// <param name="ipAddress">Ip adresa servera</param>
		/// <param name="ipPort">Ip port servera</param>
		public TcpServer(IPAddress ipAddress, Int32 ipPort)
		{
            this.Change(ipAddress, ipPort);
		}
        #endregion

        #region - Event -
        /// <summary>
        /// Event oznamujuci akceptovanie klienta serverom
        /// </summary>
        private event TcpClientEventHandler m_tcpClientReceivedEvent = null;
		/// <summary>
		/// Event oznamujuci akceptovanie klienta serverom
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Ak je object v stave _isDisposed
		/// </exception>
		[EditorBrowsable(EditorBrowsableState.Always)]
		public event TcpClientEventHandler TcpClientReceivedEvent
		{
			add
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
					this.m_tcpClientReceivedEvent += value;
			}
			remove
			{
				//je objekt _isDisposed ?
				if (this.IsDisposed)
					throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
					this.m_tcpClientReceivedEvent -= value;
			}
		}
		#endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje ci server pocuva na uvedenom porte alebo nie
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        public Boolean IsListened
        {
            get
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this.m_isListened;
            }
            set
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                //nastavime hodnotu
                m_isListened = value;
            }
        }
        #endregion

		#region - Variable -
        /// <summary>
        /// Definuje ci server pocuva na uvedenom porte alebo nie
        /// </summary>
        private volatile Boolean m_isListened = false;
		/// <summary>
		/// Pracovne vlakno zabezpecujuce akceptovanie klientov
		/// </summary>
		private Thread m_processThread = null;
		/// <summary>
		/// Listener zabezpecujuci akceptovanie klientov
		/// </summary>
		private TcpListener m_listener = null;
		/// <summary>
		/// Ip adresa na ktorej pocuva server
		/// </summary>
		private volatile IPAddress m_ipAddress = null;
		/// <summary>
		/// Ip port na akom pocuva server
		/// </summary>
        private volatile Int32 m_ipPort = 0;
		#endregion

        #region - Public Method -
        /// <summary>
        /// Umozni zenu ip adresy a ip portu. Zmena sa prejavi az po opatovnom spusteni
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovany argument
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Rozsah portu mimo povolene hranice
        /// </exception>
        /// <param name="ipAddress">Ip adresa servera</param>
        /// <param name="ipPort">Ip port servera</param>
        public void Change(IPAddress ipAddress, Int32 ipPort)
        {
            //overime vstupne parametre
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");
            //overime rozsah portu
            if (IPEndPoint.MaxPort < ipPort || IPEndPoint.MinPort > ipPort)
                throw new ArgumentOutOfRangeException("ipPort");

            //uchovame si argumenty
            this.m_ipAddress = ipAddress;
            this.m_ipPort = ipPort;
        }
        #endregion

        #region - Protected and Private Method -
        /// <summary>
		/// Inicializuje server na pzoadovanej adrese a porte
		/// </summary>
		/// <returns>True = start klienta bol uspesny</returns>
		protected override bool InternalStart()
		{
            //overime adresu
            if (this.m_ipAddress == null)
                throw new InvalidOperationException("IpAddress and IpPort is not initialized !");

			try
			{
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Iniializacia listenera...");

				//inicializacia listenera
				this.m_listener = new TcpListener(this.m_ipAddress, this.m_ipPort);
				
                //spustime listener
				this.m_listener.Start();

                //accept client
                this.m_listener.BeginAcceptTcpClient(this.InternalOnAccept, null);

                /*
				//spustime thread na prijimanie klientov
				this.m_processThread = new Thread(new ThreadStart(() => 
                {
                    this.InternalListenerProcess();
                }));
                this.m_processThread.IsBackground = true;
				this.m_processThread.Start();
                */

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Iniializacia listenera bola uspesna.");

				//start bol uspesny
				return true;
			}
			catch (Exception ex)
			{
				//zalogujeme
				this.InternalTrace(TraceTypes.Error, "Chyba pri spustani listenera. {0}", ex.Message);
				//start sa nepodaril
				return false;
			}
		}
		/// <summary>
		/// Ukonci server a akceptovanie klientov
		/// </summary>
		protected override void InternalStop()
		{
            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Ukoncovanie listenera...");

                //ukoncime listener
                if (this.m_listener != null)
                {
                    //ukoncime
                    this.m_listener.Stop();
                }

                //ukoncime pracovne vlakno
                if (this.m_processThread != null)
                {
                    try
                    {
                        //ukoncime
                        this.m_processThread.Abort();
                    }
                    catch (Exception)
                    {
                        //chybu ignorujeme
                    }
                }

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Ukoncenie listenera bolo uspesne...");
            }
            catch (Exception)
            {
                //zalogujeme
                //this.InternalTrace(TraceTypes.Verbose, "Chyba pri ukoncovani listenera. {0}", ex.Message);

                //preposleme vynimku vyssie
                throw;
            }
			finally
			{
				//deinicializujeme
				this.m_listener = null;
				this.m_processThread = null;
                this.m_isListened = false;
			}
		}
        /// <summary>
        /// Vykona akceptaciu prijateho klienta
        /// </summary>
        /// <param name="result">Asynchronny result</param>
        private void InternalOnAccept(IAsyncResult result)
        {
            //check listener
            if (this.m_listener != null)
            {
                //get tcp client
                TcpClient client = result != null ? this.m_listener.EndAcceptTcpClient(result) : null;

                //overime klienta
                if (client != null)
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Info, "Akceptovanie klienta: '{0}'", client.Client.RemoteEndPoint);

                    //odosleme event s prijatym klientom
                    this.OnTcpClientReceived(new TcpClientEventArgs(client));

                    //new accept
                    this.m_listener.BeginAcceptTcpClient(this.InternalOnAccept, null);
                }
                else
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Warning, "Akceptovanie klienta zalyalo. Vlakno bude ukoncene.");
                }
            }
        }
		/// <summary>
		/// Obsluha pracovneho vlakna
		/// </summary>
		private void InternalListenerProcess()
		{
            try
            {
                //pocuvanie je aktivne
                this.m_isListened = true;

                //cakanie na pripojenie klientov
                this.InternalListen();
            }
            catch (ThreadAbortException)
            {
                //ignorujeme chybu pri ukonceni vlakna
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pracovneho vlakna. {0}", ex.Message);
            }
            finally
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Pracovne vlakno na akceptovanie klientov bolo ukoncne.");

                //pocuvanie nie je aktivne
                this.m_isListened = false;
            }
		}
		/// <summary>
		/// Metoda zabezpecujuca akceptovanie klientov
		/// </summary>
		private void InternalListen()
		{
			//zalogujeme
			this.InternalTrace(TraceTypes.Verbose, "Start pracovneho vlakna pre akceptovanie klientov");

			//nekonecna slucka pocas behu klienta
			while (this.ClientState == ClientStates.Start)
			{
				//akceptujeme pripojenie
				TcpClient client = this.m_listener.AcceptTcpClient();

				//overime klienta
                if (client != null)
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Info, "Akceptovanie klienta: '{0}'", client.Client.RemoteEndPoint);

                    //odosleme event s prijatym klientom
                    this.OnTcpClientReceived(new TcpClientEventArgs(client));
                }
                else
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Warning, "Akceptovanie klienta zalyalo. Vlakno bude ukoncene.");
                    //ukoncime
                    break;
                }
			}
		}
        #endregion

        #region - Call Event Method -
        /// <summary>
        /// Vygeneruje event oznamujuci prijatie xml dokumentu
        /// </summary>
        /// <param name="e">XmlDocumentEventArgs</param>
        protected virtual void OnTcpClientReceived(TcpClientEventArgs e)
		{
			TcpClientEventHandler handler = this.m_tcpClientReceivedEvent;

			if (handler != null)
				handler(this, e);
		}
		#endregion
	}
}

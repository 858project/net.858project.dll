using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Project858.ComponentModel.Client;
using System.Net.Sockets;
using Project858.Diagnostics;
using System.Threading;

namespace Project858.Net
{
    /// <summary>
    /// Klient zabezpecujuci komunikaciu pomocou udp protokolu
    /// </summary>
    public class UdpTransportClient : ClientBase, ITransportClient
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="localAddress">Ip adresa na ktorej upd klient pocuva</param>
        /// <param name="localPort">Ip port na ktorom upd klient pocuva</param>
        /// <param name="remoteAddress">Ip adresa na ktoru sa data odosielaju</param>
        /// <param name="remotePort">Ip port na ktory sa data odosielaju</param>
        public UdpTransportClient(IPAddress localAddress, int localPort, IPAddress remoteAddress, int remotePort)
            : this(new IPEndPoint(localAddress, localPort), new IPEndPoint(remoteAddress, remotePort))
        {

        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="localIpEndPoint">Ip end point servera</param>
        /// <param name="remoteIpEndPoint">Ip end point kde klient pocuva</param>
        public UdpTransportClient(IPEndPoint localIpEndPoint, IPEndPoint remoteIpEndPoint)
        {
            if (localIpEndPoint == null)
                throw new ArgumentNullException("localIpEndPoint");
            if (remoteIpEndPoint == null)
                throw new ArgumentNullException("remoteIpEndPoint");

            this.m_localIpEndPoint = localIpEndPoint;
            this.m_remoteIpEndPoint = remoteIpEndPoint;
        }
        #endregion

        #region - Event -
        /// <summary>
        /// Event na oznamenie spustenia spojenia pre vyssu vrstvu
        /// </summary>
        private event EventHandler m_connectedEvent = null;
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
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this.m_connectedEvent += value;
            }
            remove
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this.m_connectedEvent -= value;
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
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this.disconnectedEvent += value;
            }
            remove
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

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
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this.m_receivedDataEvent += value;
            }
            remove
            {

                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                //netusim preco ale to mi zvykne zamrznut thread a ja fakt neviem preco
                lock (this.m_eventLock)
                    this.m_receivedDataEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci zmenu stavu pripojenia
        /// </summary>
        private event EventHandler m_changeConnectionStateEvent = null;
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
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this.m_changeConnectionStateEvent += value;
            }
            remove
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this.m_changeConnectionStateEvent -= value;
            }
        }
        #endregion

        #region - Proeprties -
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
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this.m_connectionState;
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
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this.m_isConnectied;
            }
        }
        /// <summary>
        /// Ip end point kam je klient pripojeny
        /// </summary>
        public IPEndPoint RemoteIpEndPoint
        {
            get {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                return m_remoteIpEndPoint;
            }
        }
        /// <summary>
        /// Ip end point kde klient pocuva
        /// </summary>
        public IPEndPoint LocalIpEndPoint
        {
            get {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return m_localIpEndPoint; }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// detekcia pripojenia
        /// </summary>
        private volatile Boolean m_isConnectied = false;
        /// <summary>
        /// Thred zabezpecujuci citani dat.
        /// </summary>
        private Thread m_thread = null;
        /// <summary>
        /// Stav v akom sa nachadza komunikacia / komunikacny kanal
        /// </summary>
        private ConnectionStates m_connectionState = ConnectionStates.Closed;
        /// <summary>
        /// Ip end point kam je klient pripojeny
        /// </summary>
        private IPEndPoint m_remoteIpEndPoint = null;
        /// <summary>
        /// Ip end point kde klient pocuva
        /// </summary>
        private IPEndPoint m_localIpEndPoint = null;
        /// <summary>
        /// Klient na udp komunikaciu
        /// </summary>
        private UdpClient m_client = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Vrati meno triedy
        /// </summary>
        /// <returns>Meno triedy</returns>
        public override string ToString()
        {
            return String.Format("TUdpTransportClient");
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
                throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

            //otvorenie nie je mozne ak je connection == true
            if (!this.m_isConnectied)
                throw new InvalidOperationException("Zapis dat nie je mozny ! Spojenie nie je !");

            try
            {
                //zalogujeme
                //this.InternalTrace(TraceTypes.Verbose, "Odosielanie dat: [{0}]", BitConverter.ToString(_data));
                this.InternalTrace(TraceTypes.Verbose, "Odosielanie dat: [{0}]", data.Length);

                //zapiseme _data
                this.m_client.Send(data, data.Length);

                //zalogujeme prijate dat
                this.InternalTrace(TraceTypes.Verbose, "Data boli uspesne odoslane...");

                //uspesne ukoncenie metody
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri zapise dat. {0}", ex.Message);

                //ukoncime klienta
                this.InternalStop();

                //chybne ukoncenie metody
                return false;
            }
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Vykona interny start klienta
        /// </summary>
        /// <returns>True = start klienta bol uspesny</returns>
        protected override bool InternalStart()
        {
            //zacina pripajanie
            this.m_connectionState = ConnectionStates.Connecting;

            //doslo k zmene stavu
            this.OnChangeConnectionState(EventArgs.Empty);

            //inicializujeme spojenie
            return this.InternalConnect();
        }
        /// <summary>
        /// Vykona interny stop klienta
        /// </summary>
        protected override void InternalStop()
        {
            //ukoncime pracovne vlakno
            this.InternalDeinitializeThread();

            //ukoncime spojenie
            this.InternalDisconnect();

            //doslo ku konektu
            this.m_isConnectied = false;
            //ukoncenie spojenia
            this.OnDisconnected(EventArgs.Empty);

            //zmena stavu
            this.m_connectionState = ConnectionStates.Closed;
            //event oznamujuci zmenu stavu
            this.OnChangeConnectionState(EventArgs.Empty);
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Vykona interny pokus o pripojenie k vzdialenemu bodu
        /// </summary>
        /// <returns>True = spojenie bolo uspesne vytvorene</returns>
        private Boolean InternalConnect()
        {
            try
            {
                lock (this)
                {
                    //inicializujeme klienta
                    this.m_client = new UdpClient(this.m_localIpEndPoint);
                    this.m_client.Connect(this.m_remoteIpEndPoint);
                  
                    //spustime vlakno na citanie dat
                    this.InternalInitializeThread();

                    //doslo ku konektu
                    this.m_isConnectied = true;

                    //spojenie bolo vytvorene
                    this.OnConnected(EventArgs.Empty);

                    //start komunikacie sa podaril
                    this.m_connectionState = ConnectionStates.Connected;

                    //doslo k zmene stavu
                    this.OnChangeConnectionState(EventArgs.Empty);
                }

                //inicializacia komunikacie bola uspesna
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vytvarani spojenia. {0}", ex);

                //inicializacia nebola uspesna
                return false;
            }
        }
        /// <summary>
        /// Interny disconnect
        /// </summary>
        private void InternalDisconnect()
        {
            try
            {
                //ukoncime klienta
                if (this.m_client != null)
                {
                    this.m_client.Close();
                }
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri ukoncovani spojenia spojenia. {0}", ex);
            }
            finally
            {
                //deinicializujeme klienta
                this.m_client = null;
            }
        }
        /// <summary>
        /// Inicializuje pracovne vlakno zabezpecujuce citanie dat
        /// </summary>
        private void InternalInitializeThread()
        {
            //inicializujeme pracovne vlakno
            this.m_thread = new Thread(new ThreadStart(this.InternalReceive));
            this.m_thread.IsBackground = true;

            //spustime pracovne vlakno
            this.m_thread.Start();
        }
        /// <summary>
        /// Deinicializuje pracovne vlakno
        /// </summary>
        private void InternalDeinitializeThread()
        {
            try
            {
                if (this.m_thread != null)
                {
                    this.m_thread.Abort();
                    this.m_thread.Join();
                }
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri ukoncovani pracovneho vlakna. {0}", ex);
            }
            finally
            {
                this.m_thread = null;
            }
        }
        /// <summary>
        /// Pracovne vlakno zabezpecujuce citanie dat
        /// </summary>
        private void InternalReceive()
        {
            try
            {
                while (this.IsRun)
                {
                    //ip end point z ktoreho prijmame data
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
              
                    //nacitame data
                    byte[] data = this.m_client.Receive(ref remoteEndPoint);
                    
                    //ak boli nejake data nacitane
                    if (data != null || data.Length > 0) 
                    {
                        //odosleme data na spracovanie
                        this.OnReceivedData(new DataEventArgs(data, remoteEndPoint));
                    }
                }
            }
            catch (ThreadAbortException)
            {
                //chybu ignorujeme
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pracovneho vlakna aplikacie. {0}", ex);

                //ukoncime
                this.InternalStop();
            }
        }
        #endregion

        #region - Call Event Method -
        /// <summary>
        /// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
        /// oznamujuceho prijatie dat
        /// </summary>
        /// <param name="e">EventArgs obsahujuci _data</param>
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
            EventHandler handler = this.m_connectedEvent;

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
            EventHandler handler = this.m_changeConnectionStateEvent;

            //vyvolame event
            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}

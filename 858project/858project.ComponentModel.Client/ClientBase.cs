using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Net.Mail;
using System.Data;
using System.Data.SqlClient;
using Project858;
using Project858.Diagnostics;
using Project858.ComponentModel;
using Project858.Net.Mail;
using Project858.Data.SqlClient;

namespace Project858.ComponentModel.Client
{
    /// <summary>
    /// ClientBase / predpis a implementacia zakladnych vlastnosti klienta / modulu
    /// </summary>traceTypes
    public abstract class ClientBase : ITraceComponent, IMailComponent, ISqlComponent, IClient
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public ClientBase()
        {
        }
        #endregion

        #region - Event -
        /// <summary>
        /// Event oznamujuci poziadavku na odoslanie emailu
        /// </summary>
        private event MailEventHandler _mailEvent = null;
        /// <summary>
        /// Event oznamujuci poziadavku na odoslanie emailu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event MailEventHandler MailEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this._mailEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this._mailEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci uspesny start klienta
        /// </summary>
        private event EventHandler clientStartEvent = null;
        /// <summary>
        /// Event oznamujuci uspesny start klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler ClientStartEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.clientStartEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.clientStartEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci uspesne pozastavenie klienta
        /// </summary>
        private event EventHandler clientPauseEvent = null;
        /// <summary>
        /// Event oznamujuci uspesne pozastavenie klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler ClientPauseEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.clientPauseEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.clientPauseEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci uspesne ukoncanie klienta
        /// </summary>
        private event EventHandler clientStopEvent = null;
        /// <summary>
        /// Event oznamujuci uspesne ukoncanie klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler ClientStopEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.clientStopEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.clientStopEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci poziadavku na logovanie informacii
        /// </summary>
        private event TraceEventHandler traceEvent = null;
        /// <summary>
        /// Event oznamujuci poziadavku na logovanie informacii
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event TraceEventHandler TraceEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.traceEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.traceEvent -= value;
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Klient zabezpecujuci pristup k SQL serveru a vykonavanie prikazov
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public ISqlClient SqlClient
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._sqlClient;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                _sqlClient = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'EmailEvent' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean MailEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._mailEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._mailEventAsync = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je zapnute  odosielanie emailovych sprav v ramci klienta
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean MailMessageInternalProcess
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                //ak nie je nastaveny ziadny klient na logovanie
                if (this._mailClient == null)
                    throw new InvalidOperationException();

                return this._mailMessageInternalProcess;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                _mailMessageInternalProcess = value;
            }
        }
        /// <summary>
        /// (Get / Set) Klient zabezpecujuci odosielanie emailovych správ
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public IMailClient MailClient
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._mailClient;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                //ak klient nie je inicializovany
                if (value == null)
                    this._mailMessageInternalProcess = false;

                _mailClient = value;
            }
        }
        /// <summary>
        /// (Get / Set) Klient zabezpecujuci logovanie data
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public ITraceClient TraceClient
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return _traceClient;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                //ak klient nie je inicializovany
                if (value == null)
                    this._traceInternalProcess = false;

                _traceClient = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'TraceEvent' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean TraceEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._traceEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._traceEventAsync = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje typ logovania informacii
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public TraceTypes TraceType
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._traceType;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._traceType = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je logovanie chyb zapnute za kazdych okolnosti
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean TraceErrorAlways
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._traceErrorAlways;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._traceErrorAlways = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je zapnute logovanie informaci v ramci klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean TraceInternalProcess
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                //ak nie je dostupny klient tak nie ani interne logovanie
                if (this._traceClient == null)
                    return false;

                //klient je dostupny ale berieme stav aky je nastaveny
                return _traceInternalProcess;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                //ak nie je nastaveny ziadny klient na logovanie
                if (this._traceClient == null && value == true)
                    throw new InvalidOperationException();

                _traceInternalProcess = value;
            }
        }
        /// <summary>
        /// (Get) Definuje ci bolo na tiedu zavolane Dispose()
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool IsDisposed
        {
            get { return this._disposed; }
        }
        /// <summary>
        /// (Get) Urcuje ci je klient spusteny. Ak ano stav klienta je 'start' alebo 'pause'
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool IsRun
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._clientState != ClientStates.Stop;
            }
        }
        /// <summary>
        /// (Get) Definuje stav klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public ClientStates ClientState
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return _clientState;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'ClientStart' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean ClientStartEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._clientStartEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._clientStartEventAsync = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'ClientPause' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean ClientPauseEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._clientPauseEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._clientPauseEventAsync = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'ClientStop' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean ClientStopEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                return this._clientStopEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this._disposed)
                    throw new ObjectDisposedException("Object was disposed");

                this._clientStopEventAsync = value;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Definuje ci je zapnute odosielanie emailov v ramci klienta
        /// </summary>
        private Boolean _mailMessageInternalProcess = false;
        /// <summary>
        /// Klient zabezpecujuci odosielanie emailových sprav
        /// </summary>
        private IMailClient _mailClient = null;
        /// <summary>
        /// Definuje ci je event 'EmailEvent' v asynchronnom mode
        /// </summary>
        private Boolean _mailEventAsync = false;
        /// <summary>
        /// Definuje ci je event 'ClientStart' v asynchronnom mode
        /// </summary>
        private Boolean _clientStartEventAsync = false;
        /// <summary>
        /// Definuje ci je event 'ClientPause' v asynchronnom mode
        /// </summary>
        private Boolean _clientPauseEventAsync = false;
        /// <summary>
        /// Definuje ci je event 'ClientStop' v asynchronnom mode
        /// </summary>
        private Boolean _clientStopEventAsync = false;
        /// <summary>
        /// Definuje stav klienta
        /// </summary>
        private ClientStates _clientState = ClientStates.Stop;
        /// <summary>
        /// Track if dispose has been called
        /// </summary>
        private Boolean _disposed = false;
        /// <summary>
        /// Pomocny objekt na synchronizaciu pristupu k eventom
        /// </summary>
        protected readonly Object m_eventLock = new Object();
        /// <summary>
        /// Definuje ci je event 'TraceEvent' v asynchronnom mode
        /// </summary>
        private Boolean _traceEventAsync = false;
        /// <summary>
        /// Klient zabezpecujuci logovanie data
        /// </summary>
        private ITraceClient _traceClient = null;
        /// <summary>
        /// Definuje ci je zapnute logovanie informaci v ramci klienta
        /// </summary>
        private Boolean _traceInternalProcess = false;
        /// <summary>
        /// Detekuje ci logovania errorov pozadovane stale
        /// </summary>
        private Boolean _traceErrorAlways = false;
        /// <summary>
        /// Typ logovania ktory je nastaveny
        /// </summary>
        private TraceTypes _traceType = TraceTypes.Off;
        /// <summary>
        /// Klient zabezpecujuci pristup k SQL serveru a vykonavanie prikazov
        /// </summary>
        private ISqlClient _sqlClient = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Inicializuje klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak je klient v stave 'start'
        /// </exception>
        /// <returns>True = start klienta bol uspesny</returns>
        public Boolean Start()
        {
            //je objekt _disposed ?
            if (this._disposed)
                throw new ObjectDisposedException("Object was disposed");

            //ak je klient spusteny
            if (this._clientState == ClientStates.Start)
                throw new InvalidOperationException(String.Format("{0} is already running.", this));

            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Initializing {0}...", this);

                //interny start klienta
                if (!this.InternalStart())
                    return false;

                //prechod do ineho stavu
                this._clientState = ClientStates.Start;

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "{0} was initialized successfully", this);

                //event oznamujuci uspesny start klienta
                this.OnClientStart(EventArgs.Empty);

                //uspesny start klienta
                return true;
            }
            catch (Exception ex)
            {
                //trace message
                this.InternalTrace(TraceTypes.Error, "Error during initializing {0}. {1}", this, ex.Message);
                this.InternalException(ex);

                //chybna
                return false;
            }
        }
        /// <summary>
        /// Ukonci funkciu vrstvy / klienta
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        public void Stop()
        {
            //je objekt _disposed ?
            if (this._disposed)
                throw new ObjectDisposedException("Object was disposed");

            try
            {
                //ak ide o prechod z ineho stavu
                if (this._clientState != ClientStates.Stop)
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Verbose, "Exiting {0}...", this);
                }

                //interny stop klienta
                this.InternalStop();

                //ak ide o prechod z ineho stavu
                if (this._clientState != ClientStates.Stop)
                {
                    //zmena satvu
                    this._clientState = ClientStates.Stop;

                    //zalogujeme
                    this.InternalTrace(TraceTypes.Verbose, "{0} was exited successfully", this);

                    //event o ukonceni klienta
                    this.OnClientStop(EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Error during exiting the client {0}. {1}", this, ex);
                this.InternalException(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v stave 'start'
        /// </exception>
        public void Pause()
        {
            //je objekt _disposed ?
            if (this._disposed)
                throw new ObjectDisposedException("Object was disposed");

            //ak je klient spusteny
            if (this._clientState != ClientStates.Start)
                throw new InvalidOperationException(String.Format("{0} is not already running.", this));

            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Info, "Pausing {0}...", this);

                //interny stop klienta
                this.InternalPause();

                //zmena satvu
                this._clientState = ClientStates.Pause;

                //zalogujeme
                this.InternalTrace(TraceTypes.Info, "{0} has been successfully paused", this);

                //event o ukonceni klienta
                this.OnClientPause(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                //trace message
                this.InternalTrace(TraceTypes.Error, "Error during pausing the client {0}. {1}", this, ex);
                this.InternalException(ex);
            }
        }
        /// <summary>
        /// Deinicializuje cely objekt
        /// </summary>
        public void Dispose()
        {
            //internal call
            this.InternalDispose();

            //dispose
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Vrati meno, popis klienta
        /// </summary>
        /// <returns>Popis klienta</returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// This function is raised when client is starting
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        /// <returns>NotImplementedException</returns>
        protected virtual Boolean InternalStart()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This function is raised when client is stoping
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected virtual void InternalStop()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This function is raised when client is pausing
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected virtual void InternalPause()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This function is raised when client is disposing
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Metoda nie je implementovana
        /// </exception>
        protected virtual void InternalDispose()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Metoda volana vzdy pri internej chybe klienta
        /// </summary>
        /// <param name="exception">Chyba ktora vznikla</param>
        protected virtual void InternalException(Exception exception)
        {
            this.InternalTraceExceptionToLog(exception);
        }
        /// <summary>
        /// Spracuje emailovu spravu ktora sa ma odoslat
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupnt argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Polozka Subject, alebo Body nie je v sprave vyplnena
        /// </exception>
        /// <param name="message">Sprava na odoslanie</param>
        protected void InternalMailMessage(MailMessage message)
        {
            //osetrenie
            if (message == null)
                throw new ArgumentNullException("message");
            if (String.IsNullOrEmpty(message.Subject) || String.IsNullOrEmpty(message.Body))
                throw new ArgumentException("Argument 'message' is not valid.");

            //vykoname odoslanie spravy
            this.OnInternalMailMessage(message);
        }
        /// <summary>
        /// Spracuje emailovu spravu ktora sa ma odoslat
        /// </summary>
        /// <param name="message">Sprava na odoslanie</param>
        private void OnInternalMailMessage(MailMessage message)
        {
            //ak je poziadavka na interne odosielanie emailov
            if (this._mailMessageInternalProcess)
            {
                //len pre osetrenie
                if (this._mailClient != null)
                    if (!this._mailClient.IsDisposed)
                        if (this._mailClient.IsRun)
                        {
                            //upsravime spravu a dostupne udaje
                            //odosielatel spravy
                            if (message.From == null)
                                message.From = this._mailClient.Sender;

                            //prijemcovia spravy
                            if (message.To == null || message.To.Count == 0)
                                foreach (MailAddress addres in this._mailClient.Recipients)
                                    message.To.Add(addres);

                            //odosleme spravu
                            this._mailClient.SendAsync(message);
                        }
            }

            //odosleme spravu eventom
            this.OnMailMessage(new MailEventArgs(message));
        }
        /// <summary>
        /// Spracuje logovacie spravy. Ak sprava vyhovuje nastaveniam odosle ju vyssej vrstve
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Argument 'message' je null alebo empty
        /// </exception>
        /// <param name="type">Typ spravy</param>
        /// <param name="message">Text spravy</param>
        /// <param name="msgArgs">Dalsie argumenty do String.Format k sprave</param>
        protected void InternalTrace(TraceTypes type, String message, params Object[] msgArgs)
        {
            //preposleme dalej
            this.InternalTrace(DateTime.Now, type, message, msgArgs);
        }
        /// <summary>
        /// Spracuje logovacie spravy. Ak sprava vyhovuje nastaveniam odosle ju na logovanie
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Argument 'message' je null alebo empty
        /// </exception>
        /// <param name="time">Cas kedy sprava vznikla</param>
        /// <param name="type">Typ spravy</param>
        /// <param name="message">Text spravy</param>
        /// <param name="msgArgs">Dalsie argumenty do String.Format k sprave</param>
        protected void InternalTrace(DateTime time, TraceTypes type, String message, params Object[] msgArgs)
        {
            //osetrenie
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            //sk sprava vyhovuje jednymu z typov logovania
            if (((this._traceType & TraceTypes.Error) == type) ||
                ((this._traceType & TraceTypes.Info) == type) ||
                ((this._traceType & TraceTypes.Verbose) == type) ||
                ((this._traceType & TraceTypes.Warning) == type) ||
                (this._traceErrorAlways && type == TraceTypes.Error) ||
                (this._traceType == TraceTypes.Verbose))
            {
                //odosleme spravu na logovanie
                this.OnInternalTrace(time, type, message, msgArgs);
            }
        }
        /// <summary>
        /// Vykona logovanie spravy
        /// </summary>
        /// <param name="time">Cas kedy sprava vznikla</param>
        /// <param name="type">Typ spravy</param>
        /// <param name="message">Text spravy</param>
        /// <param name="msgArgs">Dalsie argumenty do String.Format k sprave</param>
        private void OnInternalTrace(DateTime time, TraceTypes type, String message, params Object[] msgArgs)
        {
            //ak je poziadavka na interne logovanie do suboru
            if (this._traceInternalProcess)
            {
                //len pre osetrenie
                if (this._traceClient != null)
                    if (!this._traceClient.IsDisposed)
                        if (this._traceClient.IsRun)
                        {
                            //zalogujeme
                            this._traceClient.TraceAsync(time, type, this.ToString(), String.Format(message, msgArgs));
                        }
            }

            //odosleme spravu
            this.OnTrace(new TraceEventArgs(time, this.ToString(), type, String.Format(message, msgArgs)));
        }
        /// <summary>
        /// Zaloguje chybu do log suboru
        /// </summary>
        /// <param name="exception">Exception</param>
        private void InternalTraceExceptionToLog(Exception exception)
        {
            try
            {
                TraceLogger.TraceAsync(DateTime.Now, TraceTypes.Error, this.ToString(), exception.ToString());
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Internal system error. {0}", ex.Message);
                this.InternalException(ex);
            }
        }
        #endregion

        #region - Event Call Method -
        /// <summary>
        /// Vygeneruje event oznamujuci uspesny start klienta
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnClientStart(EventArgs e)
        {
            EventHandler handler = this.clientStartEvent;

            if (handler != null)
            {
                if (this._clientStartEventAsync)
                    handler.BeginInvoke(this, e, null, null);
                else
                    handler(this, e);
            }
        }
        /// <summary>
        /// Vygeneruje event oznamujuci pozastavenie klienta
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnClientPause(EventArgs e)
        {
            EventHandler handler = this.clientPauseEvent;

            if (handler != null)
            {
                if (this._clientPauseEventAsync)
                    handler.BeginInvoke(this, e, null, null);
                else
                    handler(this, e);
            }
        }
        /// <summary>
        /// Vygeneruje event oznamujuci uspesne ukoncenie klienta
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnClientStop(EventArgs e)
        {
            EventHandler handler = this.clientStopEvent;

            if (handler != null)
            {
                if (this._clientStopEventAsync)
                    handler.BeginInvoke(this, e, null, null);
                else
                    handler(this, e);
            }
        }
        /// <summary>
        /// Vygeneruje event oznamujuci poziadavku na odoslanie emailovej spravy
        /// </summary>
        /// <param name="e">MailEventArgs</param>
        protected virtual void OnMailMessage(MailEventArgs e)
        {
            MailEventHandler handler = this._mailEvent;

            if (handler != null)
            {
                if (this._mailEventAsync)
                    handler.BeginInvoke(this, e, null, null);
                else
                    handler(this, e);
            }
        }
        /// <summary>
        /// Vygeneruje event oznamujuci poziadavku na logovanie informacii
        /// </summary>
        /// <param name="e">TraceEventArgs</param>
        protected virtual void OnTrace(TraceEventArgs e)
        {
            TraceEventHandler handler = this.traceEvent;

            if (handler != null)
            {
                if (this._traceEventAsync)
                    handler.BeginInvoke(this, e, null, null);
                else
                    handler(this, e);
            }
        }
        #endregion

        #region - IDisposable Members -
        /// <summary>
        /// Releases the unmanaged resources used by the Transport object.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check recipients see if Dispose has already been called. 
            if (disposing)
            {
                //ak je vrstva spustena
                if (this.IsRun)
                {
                    //ukoncime funkciu vrstvy
                    this.Stop();
                    //
                    //TODO: ukoncenie vrstvy
                    //
                }
            }

            this._disposed = true;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~ClientBase()
        {
            this.Dispose(false);
        }
        #endregion
    }
}

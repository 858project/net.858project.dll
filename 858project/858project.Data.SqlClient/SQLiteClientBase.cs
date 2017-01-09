using System;
using Project858.ComponentModel.Client;
using System.Data;
using Project858.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.SQLite;

namespace Project858.Data.SqlClient
{
    /// <summary>
    /// Klient zabezpecujuci vykonavanie prikazov do SQL databazy 
    /// </summary>
    public abstract class SQLiteClientBase : ClientBase, ISqlClient
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="builder">Strng builder na vytvorenie SQL connection stringu</param>
        public SQLiteClientBase(SQLiteConnectionStringBuilder builder)
        {
            //nastavime pozadovane hodnoty
            this.InternalChange(builder);
            this.m_lockObj = new Object();
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="dataSource">Data source database</param>
        /// <param name="password">Password fort sqlite database</param>
        public SQLiteClientBase(String dataSource, String password)
            : this(new SQLiteConnectionStringBuilder()
            {
                DataSource = dataSource,
                Password = password
            })
        {
        }
        #endregion

        #region - Event -
        /// <summary>
        /// Event oznamujuci uplynutie intervalu na pripojenie k sql serveru
        /// </summary>
        private event EventHandler _connectingFaultEvent = null;
        /// <summary>
        /// Event oznamujuci uplynutie intervalu na pripojenie k sql serveru
        /// </summary>
        public event EventHandler ConnectingFaultEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this._connectingFaultEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this._connectingFaultEvent -= value;
            }
        }
        /// <summary>
        /// Event oznamujuci zmenu stavu pripojenia
        /// </summary>
        private event EventHandler _connectionStateChangeEvent = null;
        /// <summary>
        /// Event oznamujuci zmenu stavu pripojenia
        /// </summary>
        public event EventHandler ConnectionStateChangeEvent
        {
            add
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this._connectionStateChangeEvent += value;
            }
            remove
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this._connectionStateChangeEvent -= value;
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Definuje ci dojde k skracovaniu hodnot pri vlozeni do DB
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean TruncateValue
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this.m_truncateValue;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                this.m_truncateValue = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'ConnectionStateChangeEvent' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean ConnectionStateChangeEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this._connectionStateChangeEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                this._connectionStateChangeEventAsync = value;
            }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je event 'ConnectingFaultEvent' v asynchronnom mode
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Boolean ConnectingFaultEventAsync
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this._connectingFaultEventAsync;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                this._connectingFaultEventAsync = value;
            }
        }
        /// <summary>
        /// (Get) String builder na vytvoreie connection stringu
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        public SQLiteConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this.m_connectionStringBuilder;
            }
        }
        /// <summary>
        /// (Get) Vrati stav aktualneho pripojenia k sql serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        public ConnectionStates ConnectionState
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this._state;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Sql transakcia ktora prebieha
        /// </summary>
        private SQLiteTransaction m_transaction = null;
        /// <summary>
        /// Definuje ci dojde k skracovaniu hodnot pri vlozeni do DB
        /// </summary>
        private Boolean m_truncateValue = false;
        /// <summary>
        /// Definuje ci je event 'ConnectionStateChangeEvent' v asynchronnom mode
        /// </summary>
        private Boolean _connectionStateChangeEventAsync = false;
        /// <summary>
        /// Definuje ci je event 'ConnectingFaultEvent' v asynchronnom mode
        /// </summary>
        private Boolean _connectingFaultEventAsync = false;
        /// <summary>
        /// Stav spojenia v aktualnom stave
        /// </summary>
        private volatile ConnectionStates _state = ConnectionStates.Closed;
        /// <summary>
        /// Urcuje cas kedy doslo k spusteniu pripajania
        /// </summary>
        private DateTime _startConnecting = DateTime.MinValue;
        /// <summary>
        /// Pomocny synchronizacny objekt na pristup k pripojeniu
        /// </summary>
        private readonly Object m_lockObj = null;
        /// <summary>
        /// Sql pripojenie k serveru
        /// </summary>
        private SQLiteConnection m_connection = null;
        /// <summary>
        /// String builder na vytvorenie SQL connection stringu
        /// </summary>
        private SQLiteConnectionStringBuilder m_connectionStringBuilder = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Zaciatok transakcie
        /// </summary>
        /// <param name="level">The isolation level under which the transaction should run.</param>
        /// <exception cref="InvalidOperationException">
        /// Chyba vyvolana v pripade ze konekcia nie je aktivna
        /// </exception>
        public void BeginTransaction(IsolationLevel level)
        {
            if (this.m_connection == null || this.m_connection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection is not valid!");
            }
            this.m_transaction = this.m_connection.BeginTransaction(level);
        }
        /// <summary>
        /// Koniec transakcie
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Chyba vyvolana v pripade ze nie je aktivna ziadna transakcia
        /// </exception>
        public void EndTransaction()
        {
            if (this.m_transaction == null)
            {
                throw new InvalidOperationException("Transaction is not valid!");
            }
            this.m_transaction.Commit();
        }
        /// <summary>
        /// Vratenie zmien transakcie
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Chyba vyvolana v pripade ze nie je aktivna ziadna transakcia
        /// </exception>
        public void RollbackTransaction()
        {
            if (this.m_transaction == null)
            {
                throw new InvalidOperationException("Transaction is not valid!");
            }
            this.m_transaction.Rollback();
        }
        /// <summary>
        /// Vykona pozadovany prikaz na aktivne pripojenie k SQL serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="SQLiteException">
        /// Chyba tykajuca sa SQL servera alebo commandu
        /// </exception>
        /// <exception cref="Exception">
        /// Ina chyba v ramci metody
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Command neobsahuje ziadny CommandText
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Vynimka nastavne v pripade ze je metoda volana ak je klient v inom stave ako 'Start'
        /// </exception>
        /// <param name="command">Prikaz ktory chceme vykonat</param>
        /// <returns>The first column of the first row in the result set, or a null reference</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public SQLiteDataReader ExecuteReader(SQLiteCommand command)
        {
            //overime stav klienta
            this.InternalCheckClientState();

            //osetrenie vstupneho argumentu
            if (command == null)
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(command.CommandText))
                throw new ArgumentException("Argument 'command' is not valid.");

            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "SQL Command: '{0}'", command.CommandText);

                //pridame aktivne spojenie do priakzu
                command.Connection = this.m_connection;
                if (this.m_transaction != null)
                {
                    command.Transaction = this.m_transaction;
                }

                //vykoname pozadovany prikaz
                return command.ExecuteReader();
            }
            catch (SQLiteException ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
        }
        /// <summary>
        /// Vykona pozadovany prikaz na aktivne pripojenie k SQL serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="SQLiteException">
        /// Chyba tykajuca sa SQL servera alebo commandu
        /// </exception>
        /// <exception cref="Exception">
        /// Ina chyba v ramci metody
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Command neobsahuje ziadny CommandText
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Vynimka nastavne v pripade ze je metoda volana ak je klient v inom stave ako 'Start'
        /// </exception>
        /// <param name="command">Prikaz ktory chceme vykonat</param>
        /// <returns>The number of rows affected.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int ExecuteNonQuery(SQLiteCommand command)
        {
            //overime stav klienta
            this.InternalCheckClientState();

            //osetrenie vstupneho argumentu
            if (command == null)
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(command.CommandText))
                throw new ArgumentException("Argument 'command' is not valid.");

            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, command.ToTraceString());

                //pridame aktivne spojenie do priakzu
                command.Connection = this.m_connection;
                if (this.m_transaction != null)
                {
                    command.Transaction = this.m_transaction;
                }

                //vykoname pozadovany prikaz
                return command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
        }
        /// <summary>
        /// Vykona vstupnu query a vrati scalarnu hodnotu
        /// </summary>
        /// <param name="query">Query na vykonanie prikazu</param>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <returns>The first column of the first row in the result set, or a null reference</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Object ExecuteScalarWithQuery(String query)
        {
            if (String.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException("query");

            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = query;
                return this.ExecuteScalar(command);
            }
        }
        /// <summary>
        /// Vykona pozadovany prikaz na aktivne pripojenie k SQL serveru
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="SQLiteException">
        /// Chyba tykajuca sa SQL servera alebo commandu
        /// </exception>
        /// <exception cref="Exception">
        /// Ina chyba v ramci metody
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Command neobsahuje ziadny CommandText
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Vynimka nastavne v pripade ze je metoda volana ak je klient v inom stave ako 'Start'
        /// </exception>
        /// <param name="command">Prikaz ktory chceme vykonat</param>
        /// <returns>The first column of the first row in the result set, or a null reference</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object ExecuteScalar(SQLiteCommand command)
        {
            //overime stav klienta
            this.InternalCheckClientState();

            //osetrenie vstupneho argumentu
            if (command == null)
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(command.CommandText))
                throw new ArgumentException("Argument 'command' is not valid.");

            try
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "SQL Command: '{0}'", command.ToTraceString());

                //pridame aktivne spojenie do priakzu
                command.Connection = this.m_connection;
                if (this.m_transaction != null)
                {
                    command.Transaction = this.m_transaction;
                }

                //vykoname pozadovany prikaz
                return command.ExecuteScalar();
            }
            catch (SQLiteException ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
        }
        /// <summary>
        /// Nacita pozadovane data z databazy
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="SQLiteException">
        /// Chyba tykajuca sa SQL servera alebo commandu
        /// </exception>
        /// <exception cref="Exception">
        /// Ina chyba v ramci metody
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Vstupny argument nie je inicializovany
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Command neobsahuje ziadny CommandText
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Vynimka nastavne v pripade ze je metoda volana ak je klient v inom stave ako 'Start'
        /// </exception>
        /// <param name="command">prikaz na nacitanie dat</param>
        /// <returns>Data ktore boli nacitane z databazy</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DataTable DataFill(SQLiteCommand command)
        {
            //overime stav klienta
            this.InternalCheckClientState();

            //osetrenie vstupneho argumentu
            if (command == null)
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(command.CommandText))
                throw new ArgumentException("Argument 'command' is not valid.");

            try
            {
                //pridame aktivne spojenie do priakzu
                command.Connection = this.m_connection;
                if (this.m_transaction != null)
                {
                    command.Transaction = this.m_transaction;
                }

                //inicializujeme adpater data
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);

                //pomocna tabulka na nacitanie data
                DataTable table = new DataTable();

                //nacitame data
                adapter.Fill(table);

                //vratime nacitane data
                return table;
            }
            catch (SQLiteException ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vykonavani SQL prikazu. {0}", ex);

                //preposleme vynimku vyssie
                throw;
            }
        }
        /// <summary>
        /// Vrati meno, popis triedy
        /// </summary>
        /// <returns>Meno</returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
        #endregion

        #region - Protected Method -
        /// <summary>
        /// Interne spustenie klienta
        /// </summary>
        /// <returns>True = spustenie klienta bolo uspesne</returns>
        protected override bool InternalStart()
        {
            //vykoname start pripajania k serveru
            return this.InternalConnect();
        }
        /// <summary>
        /// Pozastavi funkciu klienta
        /// </summary>
        protected override void InternalPause()
        {
            //ukoncime komunikaciu
            this.InternalStop();
        }
        /// <summary>
        /// Ukonci funkciu klienta
        /// </summary>
        protected override void InternalStop()
        {
            //ukoncime komunikaciu
            this.InternalDisconnect();
        }
        /// <summary>
        /// Vykona pred ukoncenim klienta
        /// </summary>
        protected override void InternalDispose()
        {
            //ukoncime klienta
            this.InternalStop();
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="builder">Builder na vytvorenie SQL connection stringu</param>
        private void InternalChange(SQLiteConnectionStringBuilder builder)
        {
            //regex na overenie
            if (builder == null)
                throw new ArgumentNullException("builder");

            this.m_connectionStringBuilder = builder;
        }
        /// <summary>
        /// Overi stav klienta a komunikaciu pri volani public metody
        /// </summary>
        private void InternalCheckClientState()
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

            //overime stav klienta
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Client state is not 'Start'");
            if (this.ConnectionState != ConnectionStates.Connected)
                throw new InvalidOperationException("ConnectionState is not 'Connected'");

            //overime stav spojenia
            if (!this.InternalCheckConnection())
                throw new Exception("Connection is lost.");
        }
        /// <summary>
        /// Overi stav spojenia. Ak spojenie nie je vykona start automatickeho reconnectu
        /// </summary>
        /// <returns>True = spojenie je aktivne</returns>
        private Boolean InternalCheckConnection()
        {
            //ak spojenie nie je aktivne
            if (this.m_connection != null && this.m_connection.State == System.Data.ConnectionState.Open)
                return true;

            //zatvorime existujuce spojenie
            this.InternalCloseConnection();

            //spustime nove pripajanie k sql serveru
            this.InternalConnect();

            //spojenie nieje aktivne
            return false;
        }
        /// <summary>
        /// Spusti automaticky connect k sql serveru
        /// </summary>
        /// <returns>True = spojenie bolo vytvorene, inak false</returns>
        private bool InternalConnect()
        {
            //zalogujeme
            this.InternalTrace(TraceTypes.Verbose, "Vytvaranie spojenia so serverom....");

            //zmena stavu
            this._state = ConnectionStates.Connecting;

            //zmena stavu 
            this.OnConnectionStateChange(EventArgs.Empty);

            //pokus o vytvorenie spojenia
            if (this.InternalOpenConnection())
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Spojenie so serverom bolo uspesne vytvorene.");

                //zmena stavu spojenia
                this._state = ConnectionStates.Connected;

                //oznamenie o zmene stavu
                this.OnConnectionStateChange(EventArgs.Empty);

                //spojenie je vytvorene
                return true;
            }
            else
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Spojenie so serverom sa nepodarilo vytvorit.");

                //zmena stavu spojenia
                this._state = ConnectionStates.Closed;

                //oznamenie o zmene stavu
                this.OnConnectionStateChange(EventArgs.Empty);

                //spojenie nei je vytvorene
                return false;
            }
        }
        /// <summary>
        /// Ukonci automaticky reconnect alebo aktivne spojenie k sql serveru
        /// </summary>
        private void InternalDisconnect()
        {
            //zalogujeme
            this.InternalTrace(TraceTypes.Verbose, "Ukoncenie spojenia k SQL serveru.");

            //ukoncime komunikaciu
            this.InternalCloseConnection();

            //zmena stavu
            this._state = ConnectionStates.Closed;

            //zmena stavu 
            this.OnConnectionStateChange(EventArgs.Empty);
        }
        /// <summary>
        /// Inicializuje a otvori spojenie na definovany _sqlServer a databazu
        /// </summary>
        /// <returns>True = connect bol uspesny</returns>
        private Boolean InternalOpenConnection()
        {
            try
            {
                //inicializujeme spojenie
                this.m_connection = new SQLiteConnection();
                this.m_connection.ConnectionString = this.GetConnectionString();

                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, this.m_connection.ConnectionString);

                //otvorime spojenie
                this.m_connection.Open();

                //spojenie bolo uspesne vytvorene
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Chyba pri vytvarani spojenia k SQL serveru. {0}", ex.Message);

                //chuba
                return false;
            }
        }
        /// <summary>
        /// Ukonci spojenie s sql serverom
        /// </summary>
        private void InternalCloseConnection()
        {
            //ukoncime pripojenie
            if (this.m_connection != null)
                if (this.m_connection.State != System.Data.ConnectionState.Closed)
                {
                    this.m_connection.Close();
                    this.m_connection.Dispose();
                    this.m_connection = null;
                }
        }
        /// <summary>
        /// Vrati connection string na pripojeni k SQL serveru
        /// </summary>
        /// <returns>ConnectionString</returns>
        private String GetConnectionString()
        {
            return this.m_connectionStringBuilder.ToString();
        }
        #endregion

        #region - Event Call Method -
        /// <summary>
        /// Vygeneruje event oznamujuci zmenu stavu pripojenia
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnConnectionStateChange(EventArgs e)
        {
            EventHandler handler = this._connectionStateChangeEvent;

            if (handler != null)
            {
                if (!this._connectionStateChangeEventAsync)
                    handler(this, e);
                else
                    handler.BeginInvoke(this, e, null, null);
            }
        }
        /// <summary>
        /// Vygeneruje event oznamujuci uplynutie timeoutu na pripojenie k sql serveru
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnConnectingFault(EventArgs e)
        {
            EventHandler handler = this._connectingFaultEvent;

            if (handler != null)
            {
                if (!this._connectionStateChangeEventAsync)
                    handler(this, e);
                else
                    handler.BeginInvoke(this, e, null, null);
            }
        }
        #endregion
    }
}

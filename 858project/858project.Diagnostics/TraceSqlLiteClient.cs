using System;
using System.Collections.Generic;
using System.Text;
using Project858.ComponentModel.Client;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;

namespace Project858.Diagnostics
{
    /// <summary>
    /// Klient zabezpecujuci logovanie informaii do SQLite databazy
    /// </summary>
    public sealed class TraceSqlLiteClient : ClientBase, ITraceClient
    {
        #region - Constant -
        /// <summary>
        /// Interval na periodicku pracu timra vykonavajuceho ukladanie dat
        /// </summary>
        private const Int32 TIMER_INTERVAL = 5000;
        #endregion

        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public TraceSqlLiteClient()
        {
            //inicializacia
            this._lockObj = new Object();
            this._deleteOlderThanTimeout = new TimeSpan(7, 0, 0, 0);
            this._items = new List<TraceItem>();
        }
        #endregion

        #region - Struct -
        /// <summary>
        /// Struktura predstavujuca jeden trace zaznam
        /// </summary>
        public struct TraceItem
        {
            #region - Constructor -
            /// <summary>
            /// Initialize this class
            /// </summary>
            /// <param name="time">Cas zaznamu</param>
            /// <param name="_traceType">Typ zaznamu</param>
            /// <param name="modulName">Meno modulu ktory zaznam vytvoril</param>
            /// <param name="message">Sprava zaznamu</param>
            public TraceItem(DateTime time, TraceTypes traceType, String modulName, String message)
            {
                this._time = time;
                this._traceType = traceType;
                this._modulName = modulName;
                this._message = message;
            }
            #endregion

            #region - Properties -
            /// <summary>
            /// (Get) Cas zaznau
            /// </summary>
            public DateTime Time
            {
                get { return _time; }
            }
            /// <summary>
            /// (Get) Typ zaznamu
            /// </summary>
            public TraceTypes TraceType
            {
                get { return _traceType; }
            }
            /// <summary>
            /// (Get) Meno modulu ktory zaznam vytvoril
            /// </summary>
            public String ModulName
            {
                get { return _modulName; }
            }
            /// <summary>
            /// (Get) Sprava zaznamu
            /// </summary>
            public String Message
            {
                get { return _message; }
            }
            #endregion

            #region - Variable -
            /// <summary>
            /// Cas zaznau
            /// </summary>
            private DateTime _time;
            /// <summary>
            /// Typ zaznamu
            /// </summary>
            private TraceTypes _traceType;
            /// <summary>
            /// Meno modulu ktory zaznam vytvoril
            /// </summary>
            private String _modulName;
            /// <summary>
            /// Sprava zaznamu
            /// </summary>
            private String _message;
            #endregion
        }
        #endregion

        #region - Delegate -
        /// <summary>
        /// Pomocny delegat na asynchronne volanie metody
        /// </summary>
        /// <param name="time">Cas vzniku logovacej spravy</param>
        /// <param name="_traceType">Typ logovacej spravy</param>
        /// <param name="modulName">Meno modulu ktory spravy vytvoril</param>
        /// <param name="message">Text logovacej spravy</param>
        /// <returns>True = logovanie spravy bolo uspesne</returns>
        private delegate Boolean TraceAsyncDelegate(DateTime time, TraceTypes traceType, String modulName, String message);
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje ci sa aktivne loguje aj do konzoly
        /// </summary>
        public Boolean ConsoleTrace
        {
            get { return this._consoleTrace; }
            set { this._consoleTrace = value; }
        }
        /// <summary>
        /// (Get / Set) Definuje ci je poziadavka na odstranovanie starsich zaznamov ako je 
        /// stanoveny interval DeleteOlderThanTimeout
        /// </summary>
        public Boolean DeleteOlderThan
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                return _deleteOlderThan;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                _deleteOlderThan = value;
            }
        }
        /// <summary>
        /// (Get / Set) Timeout po akom su mazane starsie data. 
        /// Min: 000:01:00:00, Max: 365:00:00:00, Default: 007:00:00:00
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Nepovoleny rozsah timeoutu
        /// </exception>
        public TimeSpan DeleteOlderThanTimeout
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                return _deleteOlderThanTimeout;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                //osetrime rozsah
                if (value < (new TimeSpan(1, 0, 0)) ||
                    value > (new TimeSpan(365, 0, 0, 0)))
                    throw new ArgumentOutOfRangeException("RemoveOldTimeout");

                _deleteOlderThanTimeout = value;
            }
        }
        /// <summary>
        /// (Get / Set) Heslo na pristup k databaze
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        public String DatabasePassword
        {
            get
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                return _databasePassword;
            }
            set
            {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                _databasePassword = value;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Kolekcia trace itemov ktore cakaju na ulozenie
        /// </summary>
        private List<TraceItem> _items = null;
        /// <summary>
        /// Pomocny timer na periodicke ukladanie log zaznamod do databazy
        /// </summary>
        private Timer _timer = null;
        /// <summary>
        /// Definuje ci sa aktivne loguje aj do konzoly
        /// </summary>
        private Boolean _consoleTrace = false;
        /// <summary>
        /// Definuje ci je poziadavka na odstranovanie starsich zaznamov ako je 
        /// stanoveny interval DeleteOlderThanTimeout
        /// </summary>
        private Boolean _deleteOlderThan = false;
        /// <summary>
        /// Timeout po akom su mazane starsie data
        /// </summary>
        private TimeSpan _deleteOlderThanTimeout = TimeSpan.MinValue;
        /// <summary>
        /// Heslo na pristup k databaze
        /// </summary>
        private String _databasePassword = String.Empty;
        /// <summary>
        /// Pomocny synchronizacny objekt na pristup k databaze
        /// </summary>
        private readonly Object _lockObj = null;
        /// <summary>
        /// SQLite konektor k suboru
        /// </summary>
        private SQLiteConnection _connection = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Zaloguje spravu do SQLite databazy
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Argument nie je inicializovany
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v stave Start
        /// </exception>
        /// <param name="time">Cas vzniku logovacej spravy</param>
        /// <param name="_traceType">Typ logovacej spravy</param>
        /// <param name="modulName">Meno modulu ktory spravy vytvoril</param>
        /// <param name="message">Text logovacej spravy</param>
        /// <returns>
        /// True = urcuje len uspesne ulozenie zaznamu do kolekcie ktora caka na ulozenie.
        /// True este neznamena ze zaznam bol aj naozaj ulozeny.
        /// </returns>
        public bool Trace(DateTime time, TraceTypes traceType, String modulName, String message)
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            //klient musi byt spusteny aby sa dalo logovat
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException(String.Format("'{0}' is not running.", this));

            //overime vstupne data
            if (String.IsNullOrEmpty(modulName))
                throw new ArgumentNullException("modulName");
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            //zalogujeme spravu
            return this.AddTraceItem(time, traceType, modulName, message);
        }
        /// <summary>
        /// Zaloguje spravu do SQLite databazy asynchronnym volanim
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Argument nie je inicializovany
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v stave Start
        /// </exception>
        /// <param name="time">Cas vzniku logovacej spravy</param>
        /// <param name="_traceType">Typ logovacej spravy</param>
        /// <param name="modulName">Meno modulu ktory spravy vytvoril</param>
        /// <param name="message">Text logovacej spravy</param>
        public void TraceAsync(DateTime time, TraceTypes traceType, String modulName, String message)
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException("Object was disposed");

            //klient musi byt spusteny aby sa dalo logovat
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException(String.Format("'{0}' is not running.", this));

            //overime vstupne data
            if (String.IsNullOrEmpty(modulName))
                throw new ArgumentNullException("modulName");
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            //pomocny delegat
            TraceAsyncDelegate asyncDelegate = new TraceAsyncDelegate(this.AddTraceItem);
            asyncDelegate.BeginInvoke(time, traceType, modulName, message, null, null);
        }
        /// <summary>
        /// Vrati meno / popis klienta
        /// </summary>
        /// <returns>Popis klienta</returns>
        public override string ToString()
        {
            return String.Format("TraceSqlLiteClient");
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Prida dalsi zaznam do kolekcia ktora caka na ulozenie do databazy
        /// </summary>
        /// <param name="time">Cas zaznamu</param>
        /// <param name="_traceType">Typ zaznamu</param>
        /// <param name="modulName">Meno modulu ktory zaznam vytvoril</param>
        /// <param name="message">Sprava zaznamu</param>
        /// <returns>True = zaznam bol uspesne pridany do kolekcie na ulozenie</returns>
        private Boolean AddTraceItem(DateTime time, TraceTypes traceType, String modulName, String message)
        {
            //vytvorime dalsiu strukturu zaznamu
            TraceItem item = new TraceItem(time, traceType, modulName, message);

            try
            {
                //zalogujeme aj do konzoly
                if (this._consoleTrace)
                {
                    //zalogujeme
                    Console.WriteLine(String.Format("[{0}] [{1}]: {2}", time.ToString("HH:mm:ss.fff"),
                        traceType, message));
                }

                //synchronizujeme pristup
                lock (this._lockObj)
                {
                    //pridame dalsi zaznam
                    this._items.Add(item);
                }

                //zaznam bol uspesne pridany
                return true;
            }
            catch (Exception ex)
            {
                //interna chyba
                this.InternalException(ex, "Internal system error");

                //zaznam sa nepodarilo pridat
                return false;
            }
        }
        /// <summary>
        /// Periodicka obsluha pre Timer
        /// </summary>
        /// <param name="obj">Argumenty state</param>
        private void TimerTick(Object obj)
        {
            if (this._timer == null || this.ClientState != ClientStates.Start)
                return;

            try
            {
                //zmenime timout
                this._timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                //ignorujeme
            }

            try
            {
                //vykoname tick
                this.ProcesingTick();
            }
            catch (Exception ex)
            {
                //interna chyba systemu
                this.InternalException(ex, "Internal system error. {0}", ex.Message);
            }

            try
            {
                //zmenime timout
                if (this._timer != null && this.ClientState == ClientStates.Start)
                    this._timer.Change(TIMER_INTERVAL, TIMER_INTERVAL);
            }
            catch (ObjectDisposedException)
            {
                //ignorujeme
            }
        }
        /// <summary>
        /// Vykona periodicke operacie potrebne na ulozenie dat
        /// </summary>
        private void ProcesingTick()
        {
            //vykoname ulozenie data
            this.SaveTraceData();
        }
        /// <summary>
        /// Vykona ulozenie prijatych dat
        /// </summary>
        private void SaveTraceData()
        {
            //pomocne pole
            TraceItem[] items;

            //synchronizujeme pristup
            lock (this._lockObj)
            {
                //vezmeme si data
                items = this._items.ToArray();

                //zmazeme list
                this._items.Clear();
            }

            //ulozime data
            this.WriteTraceItems(items);
        }
        /// <summary>
        /// Zapise pozadovane data do SQLite databazy
        /// </summary>
        /// <param name="items">Pole itemov ktore chceme ulozit do databazy</param>
        private void WriteTraceItems(TraceItem[] items)
        {
            //osetrenie
            if (items == null || items.Length == 0)
                return;

            try
            {
                //overime pripojenie
                if (this._connection == null ||
                    this._connection.State != ConnectionState.Open)
                {
                    //vykonname obnovu pripojenia
                    this.Reconnect();
                }

                //inicializacia command
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = _connection;

                    #region - Create Table -
                    //command na overenie existencie tabulky
                    String commandString = "CREATE TABLE IF NOT EXISTS 'TraceItems' (" +
                                            "'DateTime' DATETIME NOT NULL," +
                                            "'Type' TEXT NOT NULL," +
                                            "'Modul' TEXT NOT NULL," +
                                            "'Message' TEXT NOT NULL" +
                                            ")";
                    command.CommandText = commandString;
                    //vykoname prikaz na pridanie tabulky
                    command.ExecuteNonQuery();
                    #endregion

                    #region - Create Index -
                    //command na pridanie indexov do tabulky
                    commandString = "CREATE INDEX IF NOT EXISTS 'IX_TraceItems_DateTime'" +
                                    "ON 'TraceItems' ('DateTime' ASC, 'Modul' ASC)";
                    command.CommandText = commandString;
                    //vykoname prikaz na pridanie indexu
                    command.ExecuteNonQuery();
                    #endregion

                    #region - Delete Old -
                    //ak je poziadavka na zmazanie starsich zaznamov
                    if (this._deleteOlderThan)
                    {
                        //zmazeme starsie zaznamy ak recipients je pozadovane
                        commandString = "DELETE FROM 'TraceItems' WHERE DateTime <= @dateTime";
                        command.CommandText = commandString;

                        //parametre
                        SQLiteParameter deleteParam = new SQLiteParameter("@dateTime", DbType.DateTime);
                        deleteParam.Value = DateTime.Now.Subtract(this._deleteOlderThanTimeout);

                        //pridame parameter
                        command.Parameters.Clear();
                        command.Parameters.Add(deleteParam);

                        //vykoname prikaz
                        command.ExecuteNonQuery();
                    }
                    #endregion

                    #region - Insert Data -
                    //vynulujeme command
                    commandString = String.Empty;
                    //zmazeme predosle parametre
                    command.Parameters.Clear();

                    //vytvorime command na vsetky inserty
                    for (int i = 0; i < items.Length; i++)
                    {
                        //command na pridanie zaznamu do tabulky
                        commandString += String.Format("INSERT INTO 'TraceItems' (" +
                                        "DateTime, Type, Modul, Message" +
                                        ")" +
                                        "VALUES (" +
                                        "@dateTime{0}, @type{0}, @modul{0}, @message{0}" +
                                        "); ", i);
                        command.CommandText = commandString;

                        //Vytvorime parametre
                        SQLiteParameter dt_param = new SQLiteParameter(
                            String.Format("@dateTime{0}", i), DbType.DateTime);
                        dt_param.Value = items[i].Time;
                        SQLiteParameter type_param = new SQLiteParameter(
                            String.Format("@type{0}", i), DbType.String);
                        type_param.Value = items[i].TraceType.ToString();
                        SQLiteParameter modul_param = new SQLiteParameter(
                            String.Format("@modul{0}", i), DbType.String);
                        modul_param.Value = items[i].ModulName;
                        SQLiteParameter msg_param = new SQLiteParameter(
                            String.Format("@message{0}", i), DbType.String);
                        msg_param.Value = items[i].Message;
                        //pridame parametre do zoznamu
                        command.Parameters.AddRange(new SQLiteParameter[] {dt_param, type_param, 
                                                                       modul_param, msg_param });
                    }

                    //vykoname prikaz na pridanie tabulky
                    command.ExecuteNonQuery();
                    #endregion
                }
            }
            catch (SQLiteException ex)
            {
                //interna chyba
                this.InternalException(ex, "Error during tracing data to SQLite [{0}]. {1}", ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                //interna chyba
                this.InternalException(ex, "Error during tracing data to SQLite. {0}", ex.Message);
            }
        }
        /// <summary>
        /// Vykona obnovu pripojenia k SQLite databaze
        /// </summary>
        private void Reconnect()
        {
            //vytvorime cestu k priecinku na ulozenie Trace
            String subDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                Path.DirectorySeparatorChar + "Trace";

            //zistime ci existuje subfolder ak nie tak ho pridame...
            if (!Directory.Exists(subDir))
                Directory.CreateDirectory(subDir);

            //vytvorime plnu cestu k suboru
            String fileName = subDir + Path.DirectorySeparatorChar + "Trace.db";

            // Set the ConnectionString
            String connectionString = String.Empty;

            //overime ci existuje subor
            if (File.Exists(fileName))
                connectionString = String.Format(@"Data Source={0};Version=3;New=False;Compress=True;", fileName);
            else
                connectionString = String.Format(@"Data Source={0};Version=3;New=True;Compress=True;", fileName);

            try
            {
                //inicitalizaia spojenia
                this._connection = new SQLiteConnection(connectionString);

                //overime ci je dostupne heslo k databaze
                if (!String.IsNullOrEmpty(this._databasePassword))
                    this._connection.SetPassword(this._databasePassword);

                //vytvorime spojenie
                this._connection.Open();
            }
            catch (SQLiteException ex)
            {
                //zrusime pripojenie
                if (this._connection != null)
                {
                    this._connection.Dispose();
                }

                //doslo k chyb, zalogujeme
                ConsoleLogger.Info("SQLite chyba pri otvarani SQLite databazy na logovanie [{0}]. {1}",
                    ex.ErrorCode, ex.Message);
#if DEBUG
                Debug.WriteLine(String.Format("SQLite chyba pri otvarani SQLite databazy na logovanie [{0}]. {1}",
                    ex.ErrorCode, ex));
#endif
                //ukoncime s chybou
                throw;
            }
            catch (Exception ex)
            {
                //zrusime pripojenie
                if (this._connection != null)
                {
                    this._connection.Dispose();
                }

                //doslo k chyb, zalogujeme
                ConsoleLogger.Info("Chyba pri otvarani SQLite databazy na logovanie. {0}", ex);
#if DEBUG
                Debug.WriteLine(String.Format("Chyba pri otvarani SQLite databazy na logovanie. {0}", ex));
#endif
                //preposleme vynimku
                throw;
            }
        }
        #endregion

        #region - Protected Method -
        /// <summary>
        /// Interne spustenie klienta
        /// </summary>
        /// <returns>True = spustenie klienta bolo uspesne</returns>
        protected override bool InternalStart()
        {
            //len pre osetrenie
            if (this._connection != null)
            {
                this._connection.Dispose();
            }

            //vykoname reconnect pri starte
            this.Reconnect();

            //inicializujeme timer
            this._timer = new Timer(new TimerCallback(this.TimerTick),
                null, TIMER_INTERVAL, TIMER_INTERVAL);

            //klient bol uspesne inicializovany
            return true;
        }
        /// <summary>
        /// Pozastavi funkciu klienta
        /// </summary>
        protected override void InternalPause()
        {
            //ukoncime timer
            if (this._timer != null)
                this._timer.Dispose();

            //ulozime data
            this.SaveTraceData();

            //deinicializujeme
            this._items.Clear();

            //ukoncime pripojenie
            if (this._connection != null)
                if (this._connection.State != ConnectionState.Closed)
                {
                    this._connection.Close();
                    this._connection.Dispose();
                }
        }
        /// <summary>
        /// Ukonci funkciu klienta
        /// </summary>
        protected override void InternalStop()
        {
            //ukoncime timer
            if (this._timer != null)
                this._timer.Dispose();

            //ulozime data
            this.SaveTraceData();

            //deinicializujeme
            this._items.Clear();

            //ukoncime pripojenie
            if (this._connection != null)
                if (this._connection.State != ConnectionState.Closed)
                {
                    this._connection.Close();
                    this._connection.Dispose();
                }
        }
        /// <summary>
        /// Vykona pred ukoncenim klienta
        /// </summary>
        protected override void InternalDispose()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}

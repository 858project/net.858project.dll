using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Net.Mail;
using Project858.Diagnostics;
using Project858.ComponentModel.Client;
using Project858.Configuration;
using Project858.Data.SqlClient;
using Project858.Net.Mail;
using Microsoft.Win32;

namespace Project858.ServiceProcess
{
    /// <summary>
    /// Predpis pre Windows Service s podporou pre sledovanie stavu baterie
    /// </summary>
    public abstract class WindowsServiceBase : ServiceBase
    {
        #region - Constant -
        /// <summary>
        /// Status napajania. Online znamena ze pocitac aktualne bezi zo sietoveho napajania
        /// </summary>
        PowerLineStatus powerLineStatus = PowerLineStatus.Online;
        /// <summary>
        /// Interval pod akym prebieha periodicky kontrola stavu napajania
        /// </summary>
        private const Int32 TIMER_INTERVAL = 1000;
        #endregion

        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public WindowsServiceBase()
        {
            base.CanPauseAndContinue = false;
            base.CanShutdown = true;
            base.CanHandlePowerEvent = true;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje ci sa overuje hardverovy kluc systemu
        /// </summary>
        public Boolean HardwareKeyValidate
        {
            get { return _hardwareKeyValidate; }
            protected set { _hardwareKeyValidate = value; }
        }
        /// <summary>
        /// (Get / Set) Cryptovacie heslo na ulozenie hesiel v konfiguracii
        /// </summary>
        public String PasswordConfiguration
        {
            get { return _passwordConfiguration; }
            protected set { _passwordConfiguration = value; }
        }
        /// <summary>
        /// (Get) Value indicating whether the service can be paused and resumed.
        /// </summary>
        public new Boolean CanPauseAndContinue
        {
            get { return false; }
        }
        /// <summary>
        /// (Get) Value indicating whether the service should be notified when 
        /// the system is shutting down.
        /// </summary>
        public new Boolean CanShutdown
        {
            get { return true; }
        }
        /// <summary>
        /// (Get / Set) Klient vytvarajuci spravu modulov beziacich pod WIndows Service
        /// </summary>
        public WindowsServiceClient WindowsServiceClient
        {
            get { return _windowsServiceClient; }
            set { _windowsServiceClient = value; }
        }
        /// <summary>
        /// (Get / internal Set) Trace klient na logovanie informacii
        /// </summary>
        public ITraceClient TraceClient
        {
            get { return _traceClient; }
            internal set { _traceClient = value; }
        }
        /// <summary>
        /// (Get / internal Set) Mail klient na odosielanie emailovych sprav
        /// </summary>
        public IMailClient MailClient
        {
            get { return _mailClient; }
            internal set { _mailClient = value; }
        }
        /// <summary>
        /// (Get / internal Set) SQL klient na pristup k databaze
        /// </summary>
        public ISqlClient SqlClient
        {
            get { return _sqlClient; }
            internal set { _sqlClient = value; }
        }
        /// <summary>
        /// (Get) Vrati True ak je aktualny zdroj napajania AC
        /// </summary>
        public Boolean PowerStatusIsAC
        {
            get { return (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online); }
        }
        /// <summary>
        /// (Get / internal Set) Definuje percento pri ktorom sa oznamuje informacia o kritickom stave baterie
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Ak je argument mimo stanoveny rozsah
        /// </exception>
        public Int32 CriticalBatteryPercents
        {
            get { return _criticalBatteryPercents; }
            internal set
            {
                //osetrime rozsah percent
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("value");

                _criticalBatteryPercents = value; 
            }
        }
        /// <summary>
        /// (Get / internal Set) Definuje percento pri ktorom sa oznamuje informacia o nizskom  stave baterie
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Ak je argument mimo stanoveny rozsah
        /// </exception>
        public Int32 LowBatteryPercents
        {
            get { return _lowBatteryPercents; }
            internal set
            {
                //osetrime rozsah percent
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("value"); 
                
                _lowBatteryPercents = value;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Definuje ci sa overuje hardverovy kluc systemu
        /// </summary>
        private Boolean _hardwareKeyValidate = true;
        /// <summary>
        /// Cryptovacie heslo na ulozenie hesiel v konfiguracii
        /// </summary>
        private String _passwordConfiguration = String.Empty;
        /// <summary>
        /// Klient vytvarajuci spravu modulov beziacich pod WIndows Service
        /// </summary>
        private WindowsServiceClient _windowsServiceClient = null;
        /// <summary>
        /// Trace klient na logovanie informacii
        /// </summary>
        private ITraceClient _traceClient = null;
        /// <summary>
        /// Mail klient na odosielanie emailovych sprav
        /// </summary>
        private IMailClient _mailClient = null;
        /// <summary>
        /// SQL klient na pristup k databaze
        /// </summary>
        private ISqlClient _sqlClient = null;
        /// <summary>
        /// Definuje ci bola oznamena informacia o kritickom stave baterie
        /// </summary>
        private Boolean _criticalBatteryInfo = false;
        /// <summary>
        /// Definuje ci bola oznamena informacia o nizskom stave baterie
        /// </summary>
        private Boolean _lowBatteryInfo = false;
        /// <summary>
        /// Definuje percento pri ktorom sa oznamuje informacia o kritickom stave baterie
        /// </summary>
        private Int32 _criticalBatteryPercents = 5;
        /// <summary>
        /// Definuje percento pri ktorom sa oznamuje informacia o nizskom  stave baterie
        /// </summary>
        private Int32 _lowBatteryPercents = 10;
        /// <summary>
        /// pomocna premenna na ukladanie percentualnej hodnoty stavu baterie
        /// </summary>
        private Int32 _batteryLifePercent = -1;
        /// <summary>
        /// Timer zabezpecujuci sledovanie stavu napajania pri rezime Baterie
        /// </summary>
        private System.Threading.Timer _powerTimer = null;
        #endregion

        #region - Service Method -
        /// <summary>
        /// Zachytava event oznamujuci zmenu napajania, alebo zmenu stavu
        /// </summary>
        /// <param name="powerStatus">PowerBroadcastStatus</param>
        /// <returns>???</returns>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            //ziskame informacie o napajani.
            PowerStatus _powerStatus = SystemInformation.PowerStatus;

            //ak je napajanie v rezime baterie tak aktivujeme timer na kontrolu
            if (_powerStatus.PowerLineStatus == PowerLineStatus.Offline)
            {
                //overime ci je spusteny timer
                if (this._powerTimer == null)
                {
                    //inicializujeme timer
                    this._powerTimer = new System.Threading.Timer(new TimerCallback(
                        this.PowerMonitoringTick), null, TIMER_INTERVAL, TIMER_INTERVAL);
                }
            }

            //base call
            return base.OnPowerEvent(powerStatus);
        }
        /// <summary>
        /// Start service
        /// </summary>
        /// <param name="args">vstupne argumenty</param>
        protected override void OnStart(string[] args)
        {
            //ak je poziadavka na overenie hadrveroveho kluca
            if (this._hardwareKeyValidate)
                if (!this.ValidateHardwareKey())
                {
                    //zalogujeme
                    TraceLogger.Trace(DateTime.Now, TraceTypes.Error, this.ServiceName, "Hardware error !");

                    //hardverovy kluc nesedi, ukoncime service
                    this.Stop();
                }

            //volanie nizsie
            if (this.ServiceStart(args))
            {
                //base call
                base.OnStart(args);
            }
        }
        /// <summary>
        /// Pause service
        /// </summary>
        protected override void OnPause()
        {
            //volanie nizsie
            this.ServicePause();

            //base call
            base.OnPause();
        }
        /// <summary>
        /// Stop service
        /// </summary>
        protected override void OnStop()
        {
            //volanie nizsie
            this.ServiceStop();

            //base call
            base.OnStop();
        }
        /// <summary>
        /// Shutdown service
        /// </summary>
        protected override void OnShutdown()
        {
            //volanie nizsie
            this.ServiceShutdown();

            //base call
            base.OnShutdown();
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Periodicka metoda sledujuca stav napajania v rezime baterie
        /// </summary>
        /// <param name="obj">Arguments</param>
        private void PowerMonitoringTick(Object obj)
        {
            //osetrenie
            if (this._powerTimer == null)
                return;

            try
            {
                //dalsi tick timra v nekonecne
                this._powerTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                //chybu ignorujeme
                return;
            }

            try 
            {
                //kontrola satvu napajania
                this.PowerMonitoringTick();
            }
            catch (Exception ex)
            {
                //zalogujeme chybu
                TraceLogger.Trace(DateTime.Now, TraceTypes.Error,
                    this.ServiceName, String.Format("Chyba pri overovani stavu baterie. {0}", ex));
            }

            //sk sme opat v stave online, ukoncime timer
            if (this.powerLineStatus == PowerLineStatus.Online)
            {
                //ak je aktivny timer tak ho ukoncime
                if (this._powerTimer != null)
                {
                    //deinicializacia timra
                    this._powerTimer.Dispose();
                    this._powerTimer = null;

                    //ukoncime
                    return;
                }
            }

            try
            {
                //spustime tick o prislusny interval
                if (this._powerTimer != null)
                    this._powerTimer.Change(TIMER_INTERVAL, TIMER_INTERVAL);
            }
            catch (ObjectDisposedException)
            {
                //chybu ignorujeme
                return;
            }
        }
        /// <summary>
        /// Metoda ktora periodicky kontoluje stav baterie
        /// </summary>
        private void PowerMonitoringTick()
        {
            //synchronizacia
            lock (this)
            {
                //ziskame informacie o napajani.
                PowerStatus _powerStatus = SystemInformation.PowerStatus;

                //ak doslo k zmene napajania
                if (this.powerLineStatus != _powerStatus.PowerLineStatus)
                {
                    //nastavime zmenu
                    this.powerLineStatus = _powerStatus.PowerLineStatus;

                    //posunieme info o zmene stavu dalej
                    this.OnPowerStatusChange(this.powerLineStatus);

                    //ak doslo k zmene na AC
                    if (_powerStatus.PowerLineStatus == PowerLineStatus.Online)
                    {
                        //resetneme oznamenie hodnot
                        this._lowBatteryInfo = false;
                        this._criticalBatteryInfo = false;
                    }
                }
                //ak je napajanie rovnake, teda bateria, sledujeme zmenu percent
                else if (this.powerLineStatus == PowerLineStatus.Offline &&
                        _powerStatus.PowerLineStatus == PowerLineStatus.Offline)
                {
                    //overime percentualny stav baterie
                    Int32 percent = (Int32)(_powerStatus.BatteryLifePercent * 100);

                    //overime ci je stav baterie ten isty ako rpedtym
                    if (percent != this._batteryLifePercent)
                    {
                        //preposleme informaciu o zmene percent dalej
                        this.OnChangeBatteryLifePercent(percent);

                        //uchovame si aktualny percentualny stav
                        this._batteryLifePercent = percent;

                        //overime nizsky stav baterie
                        if (this._lowBatteryPercents < percent &&
                            this._lowBatteryInfo == false)
                        {
                            //nastavime stav
                            this._lowBatteryInfo = true;

                            //oznamime informaciu dalej
                            this.OnLowBattery();
                        }
                        //ak je stav vacsi ako je pozadovane pre nizsky stav baterie
                        else if (this._lowBatteryPercents > percent)
                        {
                            //nastavime ze informacia zatial nebola oznamena
                            this._lowBatteryInfo = false;
                        }

                        //overime nizsky stav baterie
                        if (this._criticalBatteryPercents < percent &&
                            this._criticalBatteryInfo == false)
                        {
                            //nastavime stav
                            this._criticalBatteryInfo = true;

                            //oznamime informaciu dalej
                            this.OnLowBattery();
                        }
                        //ak je stav vacsi ako je pozadovane pre nizsky stav baterie
                        else if (this._criticalBatteryPercents > percent)
                        {
                            //nastavime ze informacia zatial nebola oznamena
                            this._criticalBatteryInfo = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region - Protected Method -
        /// <summary>
        /// Overi hardverovy kluc systemu v ktorom service bezi
        /// </summary>
        /// <returns>True = hardverovy kluc je spravny</returns>
        protected virtual Boolean ValidateHardwareKey()
        {
            //inicializujeme a ziskame hardverovy kluc systemu
            FingerPrint fp = new FingerPrint();
            String key = fp.Value;

            try
            {
                //nacitame registre
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Hardware Profiles\");

                //overime ci registre existuju
                if (rk == null)
                    return false;

                //nacitame hodnotu
                String validate_key = (String)rk.GetValue("Hardware", String.Empty);

                //vytvorime cryptovacie heslo
                String crypto_str = String.Format("{0}{1}", Assembly.GetExecutingAssembly().GetName().Version,
                    Environment.MachineName);

                //decriptujeme string
                validate_key = Utility.StringDecrypt(validate_key, crypto_str);

                //overime kod
                if (String.Compare(validate_key, key) == 0)
                {
                    //hadrverovy klu je spravny
                    return true;
                }
                else
                {
                    //kluc nie je spravny
                    return false;
                }
            }
            catch (Exception ex)
            {
                //zalogujeme
                TraceLogger.Trace(DateTime.Now, TraceTypes.Error, this.ServiceName, ex.ToString());

                //kod nesedi
                return false;
            }
        }
        /// <summary>
        /// Inicializuje Klienta zabezpecujuceho spravu modulov beziacich pod WIndowsService
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Chyba pri Validacii konfiguracie
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Konfiguracia nie je inicializovana
        /// </exception>
        /// 
        /// <param name="configuration">Nacitana konfiguracia</param>
        /// <returns>Initializovany WindowsServiceClient alebo null</returns>
        protected virtual WindowsServiceClient InitializeWindowsServiceClient(WindowsServiceConfiguration configuration)
        {
            //osetrime vstup
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            //overime konfiguraciu
            if (configuration.Validate(this._passwordConfiguration))
            {
                //inicializujeme klienta
                WindowsServiceClient client = new WindowsServiceClient(configuration.ServiceName);
                //vratime klienta
                return client;
            }

            //ziadny klient nebol inicializovany
            return null;
        }
        /// <summary>
        /// Nacita a inicializuje vsetkych klientov
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Chyba pri Validacii konfiguracie
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Konfiguracia nie je inicializovana
        /// </exception>
        /// 
        /// <param name="configuration">Nacitana konfiguracia</param>
        protected virtual void InitializeClients(WindowsServiceConfiguration configuration)
        {
            //osetrime vstup
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            //overime konfiguraciu
            if (configuration.Validate(this._passwordConfiguration))
            {
                //inicializujeme Parent klienta
                WindowsServiceClient windowsServiceClient = this.InitializeWindowsServiceClient(configuration);

                //overime ci bol klient inicializovany
                if (windowsServiceClient == null)
                    throw new Exception("'WindowsServiceClient' initializet failed !");

                //nacitame Trace klienta
                ITraceClient traceClient = this.InitializeTraceClient(configuration);
                //nacitame Mail klienta
                IMailClient mailClient = this.InitializeMailClient(configuration.MailClientConfiguration);
                //nacitame SQL klienta
                ISqlClient sqlClient = this.InitializeSqlClient(configuration.SqlClientConfiguration);

                //overime mailoveho klienta
                if (mailClient != null)
                {
                    //ak je inicializovany trace klient
                    if (traceClient != null)
                    {
                        ((ClientBase)mailClient).TraceClient = traceClient;
                        ((ClientBase)mailClient).TraceInternalProcess = true;
                        ((ClientBase)mailClient).TraceType = configuration.TraceType;
                        ((ClientBase)mailClient).TraceEventAsync = configuration.TraceErrorAlways;
                    }
                }

                //overime sql klienta
                if (sqlClient != null)
                {
                    //ak je inicializovany trace klient
                    if (traceClient != null)
                    {
                        ((ClientBase)sqlClient).TraceClient = traceClient;
                        ((ClientBase)sqlClient).TraceInternalProcess = true;
                        ((ClientBase)sqlClient).TraceType = configuration.TraceType;
                        ((ClientBase)sqlClient).TraceEventAsync = configuration.TraceErrorAlways;
                    }

                    //ak je inicializovany mail klient
                    if (mailClient != null)
                    {
                        ((ClientBase)mailClient).MailClient = mailClient;
                    }
                }

                //nastavime klientov pre Parent klienta
                if (sqlClient != null)
                    windowsServiceClient.SqlClient = sqlClient;
                if (mailClient != null)
                    windowsServiceClient.MailClient = mailClient;
                if (traceClient != null)
                {
                    windowsServiceClient.TraceClient = traceClient;
                    windowsServiceClient.TraceInternalProcess = true;
                    windowsServiceClient.TraceType = configuration.TraceType;
                    windowsServiceClient.TraceEventAsync = configuration.TraceErrorAlways;
                }

                //nastavime klientov
                this._traceClient = traceClient;
                this._mailClient = mailClient;
                this._sqlClient = sqlClient;
                this._windowsServiceClient = windowsServiceClient;
            }
        }
        /// <summary>
        /// Inicializuje trace klienta na zaklade nacitanej konfiguracie
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Chyba pri Validacii konfiguracie
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Konfiguracia nie je inicializovana
        /// </exception>
        /// 
        /// <param name="configuration">Nacitana konfiguracia</param>
        /// <returns>Inicializovany Trace ClientBase, inak NULL</returns>
        protected virtual ITraceClient InitializeTraceClient(WindowsServiceConfiguration configuration)
        {
            //osetrime vstup
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            //overime konfiguraciu
            if (configuration.Validate(this._passwordConfiguration))
            {
                //overime akeho klienta chceme nacitat
                if (configuration.TraceClientType == TraceClientTypes.SQLite)
                {
                    //inicializujeme klienta
                    ITraceClient client = new TraceSqlLiteClient();
                    ((TraceSqlLiteClient)client).DeleteOlderThan = true;
                    ((TraceSqlLiteClient)client).DeleteOlderThanTimeout = new TimeSpan(7, 0, 0, 0);

                    //vratime klienta
                    return client;
                }
            }

            //ziadny klient nebol inicializovany
            return null;
        }
        /// <summary>
        /// Inicializuje SQL klienta na zaklade nacitanej konfiguracie
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Chyba pri Validacii konfiguracie
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Konfiguracia nie je inicializovana
        /// </exception>
        /// 
        /// <param name="configuration">Nacitana konfiguracia sql clienta</param>
        /// <returns>Inicializovany SQL ClientBase, inak NULL</returns>
        protected virtual ISqlClient InitializeSqlClient(SqlClientConfiguration configuration)
        {
            //osetrime vstup
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            //overime konfiguraciu
            if (configuration.Validate(this._passwordConfiguration))
            {
                //ak je SQL klient aktivny
                if (configuration.Enable)
                {
                    //decryptujeme heslo
                    String password = String.Empty;
                    if (Utility.TryStringDecrypt(configuration.SqlPassword, this.PasswordConfiguration,
                        out password))
                    {
                        //inicializujeme klienta
                        ISqlClient client = new SqlClient(configuration.SqlServer,
                                                          configuration.SqlDatabase,
                                                          configuration.SqlLogin,
                                                          password);

                        //vratime inicializovaneho klienta
                        return client;
                    }
                    else
                    {
                        //zalogujeme
                        TraceLogger.Trace(DateTime.Now, TraceTypes.Error,
                            this.ServiceName, "Chyba pri decryptovani hesla 'SQLPassword'.");
                    }
                }
            }

            //ziadny klient nebol inicializovany
            return null;
        }
        /// <summary>
        /// Inicializuje SQL klienta na zaklade nacitanej konfiguracie
        /// </summary>
        /// 
        /// <exception cref="ArgumentException">
        /// Chyba pri Validacii konfiguracie
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Konfiguracia nie je inicializovana
        /// </exception>
        /// 
        /// <param name="configuration">Nacitana konfiguracia mailoveho klienta</param>
        /// <returns>Inicializovany SQL ClientBase, inak NULL</returns>
        protected virtual IMailClient InitializeMailClient(MailClientConfiguration configuration)
        {
            //osetrime vstup
            if (configuration == null)
                throw new ArgumentNullException("configuration");

             //overime konfiguraciu
            if (configuration.Validate(this._passwordConfiguration))
            {
                //ak je SQL klient aktivny
                if (configuration.Enable)
                {
                    //emailova adresa odosielatela
                    MailAddress sender = new MailAddress(configuration.Sender,
                                                         configuration.SenderName);

                    //emailove adresy prijemcov
                    MailAddressCollection recipients = new MailAddressCollection();
                    foreach (String address in configuration.Recipients)
                        recipients.Add(new MailAddress(address));

                    //inicializujeme klienta
                    IMailClient client = new MailClient(configuration.SmtpServer,
                                                        configuration.SmtpPort,
                                                        configuration.SmtpLogin,
                                                        configuration.SmtpPassword,
                                                        sender,
                                                        recipients);
                    //vratime inicializovaneho klienta
                    return client;
                }
            }

            //ziadny klient nebol inicializovany
            return null;
        }
        /// <summary>
        /// Nacita konfiguracia. Prvou je Binarna, druhou je Config
        /// </summary>
        /// <returns>Nacitana konfiguracia, alebo NULL</returns>
        protected virtual WindowsServiceConfiguration LoadConfiguration()
        {
            //osetrime vstup
            if (String.IsNullOrEmpty(this._passwordConfiguration))
            {
                //zalogujeme
                TraceLogger.Trace(DateTime.Now, TraceTypes.Error, 
                    this.ServiceName, "Heslo na Cryptovanie konfiguracie nie je zadané !");
                //throw new ArgumentNullException("passwordConfiguration");
                //ziadna konfiguracia nebola nacitane
                return null;
            }

            //pomocna premenna 
            WindowsServiceConfiguration configuration = null;

            //cesta k suboru
            String path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Plugins";

            //pokus o nacitanie binarnej konfiguracie
            if (WindowsServiceConfiguration.TryBinaryDeserialize(
                String.Format("{0}\\{1}.bin", path, "Services"), this._passwordConfiguration, 
                out configuration))
            {
                //konfiguracia bola nacitana
                return configuration;
            }

            //pokus o nacitanie xml konfiguracie
            if (WindowsServiceConfiguration.TryXMLDeserialize(
                String.Format("{0}\\{1}.config", path, "Services"), this._passwordConfiguration, 
                out configuration))
            {
                //konfiguracia bola nacitana
                return configuration;
            }

            //konfiguracia nebola nacitana
            return null;
        }
        /// <summary>
        /// Metoda oznamujuca start service
        /// </summary>
        /// <param name="args">Argumenty pri starte</param>
        /// <returns>True = start service bol uspesny</returns>
        protected virtual Boolean ServiceStart(String[] args)
        {
            //nacitame konfiguraciu
            WindowsServiceConfiguration config = this.LoadConfiguration();

            //overime ci bola konfiguracia nacitana
            if (config == null)
            {
                //zalogujeme
                TraceLogger.Trace(DateTime.Now, TraceTypes.Error, this.ServiceName, "Citanie konfiguracie zlyhalo !");
                //ukoncime service
                this.Stop();
                //vratime chybny start
                return false;
            }

            try
            {
                //inicializujeme klientov
                this.InitializeClients(config);

                //spustime trace klienta
                if (this._traceClient != null)
                    if (!this._traceClient.Start())
                        throw new OperationCanceledException("Start 'TraceClient' failed !");

                //spustime mail klienta
                if (this._mailClient != null)
                    if (!this._mailClient.Start())
                        throw new OperationCanceledException("Start 'MailClient' failed !");

                //chvilu pockame na zalogovanie informacii
                Thread.Sleep(1000);

                //spustime sql klienta
                if (this._sqlClient != null)
                    if (!this._sqlClient.Start())
                        throw new OperationCanceledException("Start 'SqlClient' failed !");

                //chvilu pockame na zalogovanie informacii
                Thread.Sleep(1000);

                //spustime windows service klienta
                if (!this._windowsServiceClient.Start())
                    throw new OperationCanceledException("Start 'WindowsServiceClient' failed !");
            }
            catch (Exception ex)
            {
                //zalogujeme
                TraceLogger.Trace(DateTime.Now, TraceTypes.Error, this.ServiceName, 
                    String.Format("Chyba pri starte Sluzby. {0}", ex));

                //chvilu pockame na zalogovanie informacii
                Thread.Sleep(2000);

                //ukoncime sluzbu
                this.Stop();
                //chybny start
                return false;
            }

            //start service bol uspesny
            return true;
        }
        /// <summary>
        /// Metoda oznamujuca pozastavenie service
        /// </summary>
        protected virtual void ServicePause()
        {

        }
        /// <summary>
        /// Metoda oznamujuca ukoncenie service
        /// </summary>
        protected virtual void ServiceStop()
        {
            //overime inicializaciu parent klienta
            if (this._windowsServiceClient != null)
                if (!this._windowsServiceClient.IsDisposed)
                    if (this._windowsServiceClient.IsRun)
                        this._windowsServiceClient.Stop();

            //ukoncime aj mailoveho klienta
            if (this._mailClient != null)
                if (!this._mailClient.IsDisposed)
                    if (this._mailClient.IsRun)
                        this._mailClient.Stop();

            //ukoncime aj sql klienta
            if (this._sqlClient != null)
                if (!this._sqlClient.IsDisposed)
                    if (this._sqlClient.IsRun)
                        this._sqlClient.Stop();

            //ukoncime aj trace klienta
            if (this._traceClient != null)
                if (!this._traceClient.IsDisposed)
                    if (this._traceClient.IsRun)
                        this._traceClient.Stop();
        }
        /// <summary>
        /// Metoda oznamujuca ukoncenie service na zakalde ukoncenia systemu
        /// </summary>
        protected virtual void ServiceShutdown()
        {
            //overime inicializaciu parent klienta
            if (this._windowsServiceClient != null)
                if (!this._windowsServiceClient.IsDisposed)
                    if (this._windowsServiceClient.IsRun)
                        this._windowsServiceClient.Stop();

            //ukoncime aj mailoveho klienta
            if (this._mailClient != null)
                if (!this._mailClient.IsDisposed)
                    if (this._mailClient.IsRun)
                        this._mailClient.Stop();

            //ukoncime aj sql klienta
            if (this._sqlClient != null)
                if (!this._sqlClient.IsDisposed)
                    if (this._sqlClient.IsRun)
                        this._sqlClient.Stop();

            //ukoncime aj trace klienta
            if (this._traceClient != null)
                if (!this._traceClient.IsDisposed)
                    if (this._traceClient.IsRun)
                        this._traceClient.Stop();
        }
        /// <summary>
        /// Metoda oznamujuca zmenu stavu napajania
        /// </summary>
        /// <param name="status">Aktualny stav napajania</param>
        protected virtual void OnPowerStatusChange(PowerLineStatus status)
        {
            //ak doslo k zmene na AC
            if (status == PowerLineStatus.Online)
            {
                //spustime mailoveho klienta
                if (this._mailClient != null)
                    if (!this._mailClient.IsDisposed)
                        if (this._mailClient.ClientState == ClientStates.Pause)
                            this._mailClient.Start();

                //spustime sql klienta
                if (this._sqlClient != null)
                    if (!this._sqlClient.IsDisposed)
                        if (this._sqlClient.ClientState == ClientStates.Pause)
                            this._sqlClient.Start();

                //spustime aj trace klienta
                if (this._traceClient != null)
                    if (!this._traceClient.IsDisposed)
                        if (this._traceClient.ClientState == ClientStates.Pause)
                            this._traceClient.Start();

                //overime inicializaciu parent klienta
                if (this._windowsServiceClient != null)
                    if (!this._windowsServiceClient.IsDisposed)
                    {
                        //overime ci bol klient prepnuty do rezimu Pause
                        if (this._windowsServiceClient.ClientState == ClientStates.Pause)
                            this._windowsServiceClient.Start();

                        //ak je klient spusteny tak oznamime zmenu stavu napajania
                        if (this._windowsServiceClient.ClientState == ClientStates.Start)
                        {
                            //info o zmene napajania
                            this._windowsServiceClient.PowerStatusChange(status);
                        }
                    }
            }
        }
        /// <summary>
        /// Metoda oznamujuca percentualnu zmenu stavu baterie
        /// </summary>
        /// <param name="batteryLifePercent">Percentualna hodnota baterie</param>
        protected virtual void OnChangeBatteryLifePercent(Int32 batteryLifePercent)
        {
            //overime inicializaciu parent klienta
            if (this._windowsServiceClient != null)
                if (!this._windowsServiceClient.IsDisposed)
                    if (this._windowsServiceClient.ClientState == ClientStates.Start)
                    {
                        //info o zmene percentualnej hodnoty baterie
                        this._windowsServiceClient.ChangeBatteryLifePercent(batteryLifePercent);
                    }
        }
        /// <summary>
        /// Metoda oznamujuca nizsky stav baterie
        /// </summary>
        protected virtual void OnLowBattery()
        {
            //overime inicializaciu parent klienta
            if (this._windowsServiceClient != null)
                if (!this._windowsServiceClient.IsDisposed)
                    if (this._windowsServiceClient.ClientState == ClientStates.Start)
                    {
                        //info o nizskom stave baterie
                        this._windowsServiceClient.LowBattery();
                    }
        }
        /// <summary>
        /// Metoda oznamujuca kriticky stav abterie
        /// </summary>
        protected virtual void OnCriticalBattery()
        {
            //overime inicializaciu parent klienta
            if (this._windowsServiceClient != null)
                if (!this._windowsServiceClient.IsDisposed)
                    if (this._windowsServiceClient.ClientState == ClientStates.Start)
                    {
                        //info o kritickom stave baterie
                        this._windowsServiceClient.CriticalBattery();

                        //chvilu pockame
                        Thread.Sleep(1000);

                        //ukoncime klienta
                        this._windowsServiceClient.Pause();
                    }

            //ukoncime aj mailoveho klienta
            if (this._mailClient != null)
                if (!this._mailClient.IsDisposed)
                    if (this._mailClient.IsRun)
                        this._mailClient.Pause();

            //ukoncime aj sql klienta
            if (this._sqlClient != null)
                if (!this._sqlClient.IsDisposed)
                    if (this._sqlClient.IsRun)
                        this._sqlClient.Pause();

            //ukoncime aj trace klienta
            if (this._traceClient != null)
                if (!this._traceClient.IsDisposed)
                    if (this._traceClient.IsRun)
                        this._traceClient.Pause();
        }
        #endregion
    }
}

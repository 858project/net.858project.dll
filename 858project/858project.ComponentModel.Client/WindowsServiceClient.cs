using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Net.Mail;
using Project858.Diagnostics;

namespace Project858.ComponentModel.Client
{
    /// <summary>
    /// Klient predstavujuci Rodica spravujuceho viacero inych klientov
    /// </summary>
    public sealed class WindowsServiceClient : ClientBase
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Vstupne meno klienta nie je zadane
        /// </exception>
        /// <param name="clientName">Meno / popis klienta</param>
        public WindowsServiceClient(String clientName)
            : base()
        {
            //overime vstupny argument
            if (String.IsNullOrEmpty(clientName))
                throw new ArgumentNullException("clientName");

            //inicializacia
            this.childClients = new ClientCollecion();
            this.clientName = clientName;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Kolekcia klientov beziacich pod spravou
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        public ClientCollecion ChildClients
        {
            get {
                //je objekt _disposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");
                
                return childClients;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Meno, popis klienta
        /// </summary>
        private String clientName = String.Empty;
        /// <summary>
        /// Kolekcia klientov ktory bezia pod spravnou this
        /// </summary>
        private ClientCollecion childClients = null;
        #endregion

        #region - Public Method -
        /// <summary>
        /// Metoda oznamujuca zmenu stavu napajania
        /// </summary>
        /// 
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v rezime 'Start'
        /// </exception>
        /// 
        /// <param name="status">Aktualny stav napajania</param>
        public void PowerStatusChange(PowerLineStatus status)
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

            //metodu je mozme volat len ak je klient v rezime start
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Invalid operation,");

            //odosleme prislusny email
            if (status == PowerLineStatus.Online)
                this.EmailPowerBattery();
            else
                this.EmailPowerBattery();
        }
        /// <summary>
        /// Metoda oznamujuca percentualnu zmenu stavu baterie
        /// </summary>
        /// 
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v rezime 'Start'
        /// </exception>
        /// 
        /// <param name="batteryLifePercent">Percentualna hodnota baterie</param>
        public void ChangeBatteryLifePercent(Int32 batteryLifePercent)
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

            //metodu je mozme volat len ak je klient v rezime start
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Invalid operation,");

            //iba zalogujeme
            this.InternalTrace(TraceTypes.Verbose, "Zmena stavu baterie. [{0}%]", batteryLifePercent);
        }
        /// <summary>
        /// Metoda oznamujuca nizsky stav baterie
        /// </summary>
        /// 
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v rezime 'Start'
        /// </exception>
        public void LowBattery()
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

            //metodu je mozme volat len ak je klient v rezime start
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Invalid operation,");

            this.EmailLowBattery();
        }
        /// <summary>
        /// Metoda oznamujuca kriticky stav abterie
        /// </summary>
        /// 
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _disposed
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Ak klient nie je v rezime 'Start'
        /// </exception>
        public void CriticalBattery()
        {
            //je objekt _disposed ?
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

            //metodu je mozme volat len ak je klient v rezime start
            if (this.ClientState != ClientStates.Start)
                throw new InvalidOperationException("Invalid operation,");

            this.EmailCriticalBattery();
        }
        /// <summary>
        /// Vrati meno / popis klienta
        /// </summary>
        /// <returns>Popis klienta</returns>
        public override string ToString()
        {
            return String.Format("{0}", this.clientName);
        }
        #endregion

        #region - Private Method -
        /// <summary>
        /// Interne spustenie klienta
        /// </summary>
        /// <returns>True = spustenie klienta bolo uspesne</returns>
        protected override bool InternalStart()
        {
            //osetrenie, nemalo by nastat
            if (this.childClients == null)
            {
                //inicializujeme
                this.childClients = new ClientCollecion();
            }

            //ak nie su ziadny klienti tak ich treba nacitat
            if (this.childClients.Count == 0)
            {
                //nacitame kolekciu klientov
                ClientCollecion clients = this.GetClients();

                //osetrime
                if (clients.Count == 0)
                    throw new Exception("Nepodarilo sa najst ziadne dostupne pluginy pre inicializaciu.");

                //inicializujeme klientov
                this.InitializeClients(ref clients);

                //skopirujeme klientov
                this.childClients.AddRange(clients.ToArray());
            }

            //spustime vsetkych klientov
            if (!this.childClients.StartAll())
                return false;

            //klient bol uspesne inicializovany
            return true;
        }
        /// <summary>
        /// Pozastavi funkciu klienta
        /// </summary>
        protected override void InternalPause()
        {
            //pozastavime vsetkych klientov
            this.childClients.PauseAll();
        }
        /// <summary>
        /// Ukonci funkciu klienta
        /// </summary>
        protected override void InternalStop()
        {
            //ukonime vsetkych klientov
            this.childClients.StopAll();
        }
        /// <summary>
        /// Vykona pred ukoncenim klienta
        /// </summary>
        protected override void InternalDispose()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Nacita kolekciu klientov
        /// </summary>
        /// <returns>Kolekcia klientov</returns>
        private ClientCollecion GetClients()
        {
            //Directory ktory by mal obsahovat pluginy
            string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Plugins";

            //inicializacia kolekcia klientov
            ClientCollecion clients = new ClientCollecion();

            //overime ci existuje folder
            if (!Directory.Exists(folder))
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Error, "Directory 'Plugins' neexistuje !");
                //nevratime ziadnych klientov
                return clients;
            }

            //nacitame vsetky dll v podadresari
            String[] files = Directory.GetFiles(folder, "*.dll");

            //prejdeme vsetky subory
            foreach (String file in files)
            {
                try
                {
                    //nacitame assembly z najdenej dll
                    Assembly assembly = Assembly.LoadFile(file);

                    //prejdeme vsetky typy dostupne v dll
                    foreach (Type type in assembly.GetTypes())
                    {
                        //len objekty ktore su class a ktore su verejne
                        if (!type.IsClass || type.IsNotPublic)
                            continue;

                        //ziskame interface triedy
                        Type[] interfaces = type.GetInterfaces();

                        //prejdeme vsetky interfejsy triedy a overime ci je odedena od 'IClient'
                        if (((IList)interfaces).Contains(typeof(IClient)))
                        {
                            //moze byt pridany len klient typu ktory sa este v zozname nenachadza
                            if (!clients.TypeContains(type))
                            {
                                //vytvorime instanciu
                                Object obj = Activator.CreateInstance(type);

                                //pretypujeme
                                IClient client = (IClient)obj;

                                //pridame klienta do kolekcie
                                clients.Add(client);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Error, "Chyba pri čítaní pluginov. {0}", ex);
                }
            }

            //vratime kolekciu klientov
            return clients;
        }
        /// <summary>
        /// Iniializuje klientov
        /// </summary>
        /// <param name="clients">Kolekcia klientov ktorych chceme inicializovat</param>
        private void InitializeClients(ref ClientCollecion clients)
        {
            //inicializujeme vsetkych klientov
            for (int i = 0; i < clients.Count; i++)
            {
                //nastavime Trace klienta
                if (this.TraceClient != null)
                {
                    ((ClientBase)clients[i]).TraceClient = this.TraceClient;
                    ((ClientBase)clients[i]).TraceErrorAlways = this.TraceErrorAlways;
                    ((ClientBase)clients[i]).TraceInternalProcess = true;
                    ((ClientBase)clients[i]).TraceType = this.TraceType;
                }

                //nastavime Mail klienta
                if (this.MailClient != null)
                {
                    ((ClientBase)clients[i]).MailClient = this.MailClient;
                    ((ClientBase)clients[i]).MailMessageInternalProcess = true;
                }

                //nastavime SQL klienta
                if (this.SqlClient != null)
                {
                    ((ClientBase)clients[i]).SqlClient = this.SqlClient;
                }
            }
        }
        #endregion

        #region - Email Method -
        /// <summary>
        /// Vytvori spravu oznamujucu nizky stav baterie v systeme pod ktorym klient bezi
        /// </summary>
        private void EmailLowBattery()
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"red\" size=\"3\">Služba detekuje nízky stav batérie.</font></b>");
            body += String.Format("<br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Warning, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        /// <summary>
        /// Vytvori spravu oznamujucu kriticky stav baterie v systeme pod ktorym klient bezi
        /// </summary>
        private void EmailCriticalBattery()
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"red\" size=\"3\">Služba detekuje kritický stav batérie</font></b><br>");
            body += String.Format("<font color=\"red\" size=\"3\">Služba prechádza do režimu 'Pause'...</font><br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Warning, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.High;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        /// <summary>
        /// Vytvori spravu oznamujucu zmenu napajania na AC
        /// </summary>
        private void EmailPowerAc()
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"red\" size=\"3\">Služba detekuje zmenu napájania do režimu AC.</font></b>");
            body += String.Format("<br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Warning, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        /// <summary>
        /// Vytvori spravu oznamujucu zmenu napajania na bateriu
        /// </summary>
        private void EmailPowerBattery()
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"red\" size=\"3\">Služba detekuje zmenu napájania do režimu Battery.</font></b>");
            body += String.Format("<br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Warning, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        /// <summary>
        /// Vytvori spravu oznamujucu spustenie klienta
        /// </summary>
        private void EmailClientStart()
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"blue\" size=\"3\">Služba bola úspešné spustená.</font></b>");
            body += String.Format("<br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Info, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        /// <summary>
        /// Vytvori spravu oznamujucu ukoncenie klienta
        /// </summary>
        private void EmailClientStop()
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"blue\" size=\"3\">Služba bola úspešné ukončená.</font></b>");
            body += String.Format("<br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Info, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        /// <summary>
        /// Vytvori spravu oznamujucu pozastavenie klienta
        /// </summary>
        private void EmailClientPause()
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"blue\" size=\"3\">Služba bola úspešné pozastavená.</font></b>");
            body += String.Format("<br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Info, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        /// <summary>
        /// Vytvori a odosle spravu s chybou klienta
        /// </summary>
        /// <param name="ex">Chyba klienta</param>
        private void EmailClientError(Exception ex)
        {
            //telo spravy
            String body = String.Format("<html><body><div align=\"center\">");
            body += String.Format("<b><font color=\"silver\" size=\"1\">[This e-mail has been automatically generated.]</font></b>");
            body += String.Format("</div><br><br>");
            body += String.Format("<b><font color=\"red\" size=\"3\">Služba detekuje chybu.</font></b><br><br>");
            body += String.Format("<font color=\"black\" size=\"3\">{0}</font>", ex);
            body += String.Format("<br><br><br>");
            //ukoncenie srpravy
            body += String.Format("<font size=\"1\">{0}<font><br>", DateTime.Now.ToString("HH:mm:ss dd:MM:yyyy"));
            body += String.Format("<font size=\"3\">-------------<font><br>");
            body += String.Format("<b><i><font size=\"4\">People Counting<font></i></b>");
            body += String.Format("<br><font size=\"1\">Copyright © PL Technology s.r.o 2010<font></body></html>");

            //vytvorime spravu na odoslanie
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = String.Format("{0} {1}", TraceTypes.Error, this.ToString());
            mailMessage.Body = body;
            mailMessage.Priority = MailPriority.High;
            mailMessage.IsBodyHtml = true;

            //odosleme email
            this.InternalMailMessage(mailMessage);
        }
        #endregion
    }
}

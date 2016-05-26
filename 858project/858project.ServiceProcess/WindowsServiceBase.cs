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

        #region - Service Method -
        /// <summary>
        /// Start service
        /// </summary>
        /// <param name="args">vstupne argumenty</param>
        protected override void OnStart(string[] args)
        {
            //volanie nizsie
            if (this.InternalServiceStart(args))
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
            this.InternalServicePause();

            //base call
            base.OnPause();
        }
        /// <summary>
        /// Stop service
        /// </summary>
        protected override void OnStop()
        {
            //volanie nizsie
            this.InternalServiceStop();

            //base call
            base.OnStop();
        }
        /// <summary>
        /// Shutdown service
        /// </summary>
        protected override void OnShutdown()
        {
            //volanie nizsie
            this.InternalServiceShutdown();

            //base call
            base.OnShutdown();
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Kolekcia aktualnych klientov
        /// </summary>
        private List<IWindowsServiceClient> m_clients = null;
        #endregion

        #region - Private Method -
        /// <summary>
        /// Metoda oznamujuca start service
        /// </summary>
        /// <param name="args">Argumenty pri starte</param>
        /// <returns>True = start service bol uspesny</returns>
        private Boolean InternalServiceStart(String[] args)
        {
            try
            {
                //zalogujeme
                this.InternalTrace("Inicializacia sluzby...");

                //vytvorime instancie
                this.m_clients = this.InternalCreateInstance();

                //spustime klientov
                this.InternalClientStart();

                //zalogujeme
                this.InternalTrace("Inicializacia sluzby bola uspesna.");

                //inicializacia bola uspesna
                return true;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace("Inicializacia sluzby zlyhala. {0}", ex);

                //start sluzby sa nepodaril
                return false;
            }
        }
        /// <summary>
        /// Metoda oznamujuca pozastavenie service
        /// </summary>
        private void InternalServicePause()
        {
            this.InternalClientStop();
        }
        /// <summary>
        /// Metoda oznamujuca ukoncenie service
        /// </summary>
        private void InternalServiceStop()
        {
            this.InternalClientStop();
        }
        /// <summary>
        /// Metoda oznamujuca ukoncenie service na zakalde ukoncenia systemu
        /// </summary>
        private void InternalServiceShutdown()
        {
            this.InternalClientStop();
        }
        /// <summary>
        /// Ukonci vsetky instancie ktore su dostupne
        /// </summary>
        private void InternalClientStop()
        {
            //ak su dostupne nejake instancie
            if (this.m_clients != null && this.m_clients.Count > 0)
            {
                //prejdeme vsetky instancie
                foreach (IWindowsServiceClient client in this.m_clients)
                {
                    try
                    {
                        //start klienta
                        client.Stop();
                    }
                    catch (Exception ex)
                    {
                        //zalogujeme chybu
                        this.InternalTrace("Start instancie sa nepodaril. [{0}]", client.GetType());
                        this.InternalTrace(ex);
                    }
                }
            }
        }
        /// <summary>
        /// Spusti vsetky instancie ktore su dostupne
        /// </summary>
        private void InternalClientStart()
        {
            //ak su dostupne nejake instancie
            if (this.m_clients != null && this.m_clients.Count > 0)
            {
                //prejdeme vsetky instancie
                foreach (IWindowsServiceClient client in this.m_clients)
                {
                    try
                    {
                        //start klienta
                        client.Start();
                    }
                    catch (Exception ex)
                    {
                        //zalogujeme chybu
                        this.InternalTrace("Start instancie sa nepodaril. [{0}]", client.GetType());
                        this.InternalTrace(ex);
                    }
                }
            }
        }
        /// <summary>
        /// Vrati cestu k aktualnemu priecinku odkial je aplikacia spustena
        /// </summary>
        private String InternalGetCurrentPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
        /// <summary>
        /// Vrati cesu k vsetkym knizniciam ktore sa nachadzaju na pozadovanej ceste
        /// </summary>
        /// <param name="path">Cesta v ktorej chceme najst vsetky kniznice</param>
        /// <returns>Kolekci kniznic</returns>
        private String[] InternalGetAllLibrary(String path)
        {
            if (Directory.Exists(path))
            {
               return Directory.GetFiles(path, "*.dll");
            }
            return null;
        }
        /// <summary>
        /// Vrati zoznam Assembly ktore sa nachadzaju v pozadovanych suboroch
        /// </summary>
        /// <param name="files">Zoznam suborov</param>
        /// <returnsZoznam Assembly></returns>
        private List<Assembly> InternalGetAllAssemblyFromFiles(String[] files)
        {
            List<Assembly> collection = new List<Assembly>(files.Length);
            foreach (string file in files)
            {
                AssemblyName an = AssemblyName.GetAssemblyName(file);
                Assembly assembly = Assembly.Load(an);
                collection.Add(assembly);
            }
            return collection;
        }
        /// <summary>
        /// Vrati kolekciu typov ktore su zhodne s pozadovanym typom pluginu
        /// </summary>
        /// <param name="assemblies">Kolekcia assemblies ktore su dostupne</param>
        /// <param name="pluginType">Typ pluginu ktory hladame</param>
        /// <returns>Kolekcia typov</returns>
        private List<Type> InternalGetAllInstanceType(List<Assembly> assemblies, Type pluginType)
        {
            List<Type> collection = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (!type.IsInterface && !type.IsAbstract)
                        {
                            if (type.GetInterface(pluginType.FullName) != null)
                            {
                                collection.Add(type);
                            }
                        }
                    }
                }
            }
            return collection;
        }
        /// <summary>
        /// Vytvori instancie pluginov ktore su urcene na spustenie
        /// </summary>
        /// <param name="types">Typy pluginov ktore maju byt vytvorene</param>
        /// <returns>Kolekcia instancii</returns>
        private List<IWindowsServiceClient> InternalCreateInstance(List<Type> types)
        {
            List<IWindowsServiceClient> collection = new List<IWindowsServiceClient>();
            foreach (Type type in types)
            {
                try
                {
                    IWindowsServiceClient plugin = (IWindowsServiceClient)Activator.CreateInstance(type);
                    collection.Add(plugin);
                }
                catch (Exception ex)
                {
                    //zalogujeme
                    this.InternalTrace("Inicializacia pluginu '{0}' sa nepodarila.", type);
                    this.InternalTrace(ex);
                }
            }
            return collection;
        }
        /// <summary>
        /// Zaloguje spravu do suboru
        /// </summary>
        /// <param name="message">Sprava</param>
        /// <param name="args">String.Format argumenty pre spravu</param>
        private void InternalTrace(String message, params Object[] args)
        {
            TraceLogger.Info(message, args);
        }
        /// <summary>
        /// Zaloguje chybu do log suboru
        /// </summary>
        /// <param name="exception">Chyba ktoru chceme zalogovat</param>
        private void InternalTrace(Exception exception)
        {
            TraceLogger.Error(exception);
        }
        /// <summary>
        /// Vytvori a vrati vsetky instancie pluginov ktore su dostupne
        /// </summary>
        /// <returns>Kolekcia pluginov alebo null</returns>
        private List<IWindowsServiceClient> InternalCreateInstance()
        {
            //ziskame cestu
            String path = this.InternalGetCurrentPath();

            //zalogujeme
            this.InternalTrace("Nacitavanie dostupnych pluginov... [{0}]", path);

            //nacitame vsetky dostupne kniznice
            String[] files = this.InternalGetAllLibrary(path);

            //zalogujeme
            this.InternalTrace("Pocet dostupnych kniznic: {0}", files == null ? 0 : files.Length);

            //nacitame vsetky assemblies
            List<Assembly> assemblies = this.InternalGetAllAssemblyFromFiles(files);

            //zalogujeme
            this.InternalTrace("Pocet dostupnych assemblies: {0}", assemblies == null ? 0 : assemblies.Count);

            //nacitame dostupne typy
            List<Type> types = this.InternalGetAllInstanceType(assemblies, typeof(IWindowsServiceClient));

            //zalogujeme
            this.InternalTrace("Pocet dostupnych typov: {0}", types == null ? 0 : types.Count);

            //inicializujeme instancie
            List<IWindowsServiceClient> clients = this.InternalCreateInstance(types);

            //zalogujeme
            this.InternalTrace("Pocet dostupnych instancii: {0}", clients == null ? 0 : clients.Count);

            //vratime dostupne instancie
            return clients;
        }
        #endregion
    }
}

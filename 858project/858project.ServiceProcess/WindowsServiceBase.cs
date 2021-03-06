﻿using System;
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
            this.TraceType = this.InternalGetTraceType();
            this.TraceErrorAlways = true;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get / Set) Definuje typ logovania informacii
        /// </summary>
        public TraceTypes TraceType
        {
            get;
            set;
        }
        /// <summary>
        /// (Get / Set) Definuje ci je logovanie chyb zapnute za kazdych okolnosti
        /// </summary>
        public Boolean TraceErrorAlways
        {
            get;
            set;
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
            else
            {
                //ukoncime
                this.Stop();
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
                this.InternalTrace("Initializing service...");

                //vytvorime instancie
                this.m_clients = this.InternalCreateInstance();

                //overime ci je co spustit
                if (this.m_clients.Count == 0)
                {
                    //ukoncime
                    this.InternalTrace(TraceTypes.Warning, "There are not any instances on starting");

                    //start sluzby sa nepodaril
                    return false;
                }

                //spustime klientov
                Boolean result = this.InternalClientStart();

                //check result
                if (result)
                {
                    //zalogujeme
                    this.InternalTrace("Initializing service was successful");
                }
                else
                {
                    //zalogujeme
                    this.InternalTrace("Initializing service was not successful");
                }

                //inicializacia bola uspesna
                return result;
            }
            catch (Exception ex)
            {
                //zalogujeme
                this.InternalTrace("Initializing service failed. {0}", ex.Message);
                this.InternalTrace(ex);

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
                        //zalogujeme
                        this.InternalTrace("Ending instance {0}", client.GetType());

                        //start klienta
                        client.Stop();

                        //zalogujeme
                        this.InternalTrace("Ending instance was successful");
                    }
                    catch (Exception ex)
                    {
                        //zalogujeme chybu
                        this.InternalTrace("Ending instance {0} failed", client.GetType());
                        this.InternalTrace(ex);
                    }
                }
            }
        }
        /// <summary>
        /// Spusti vsetky instancie ktore su dostupne
        /// </summary>
        private Boolean InternalClientStart()
        {
            //ak su dostupne nejake instancie
            if (this.m_clients == null || this.m_clients.Count == 0)
            {
                return false;
            }

            //prejdeme vsetky instancie
            foreach (IWindowsServiceClient client in this.m_clients)
            {
                try
                {
                    //zalogujeme
                    this.InternalTrace("Starting instance {0}", client.GetType());

                    //overime ci ide o klienta
                    if (client is ClientBase)
                    {
                        (client as ClientBase).TraceType = this.InternalGetTraceType();
                        (client as ClientBase).TraceEvent += (sender, e) =>
                        {
                            this.InternalTrace(e.TraceType, e.Message);
                        };
                    }

                    //start klienta
                    if (client.Start())
                    {
                        //zalogujeme
                        this.InternalTrace("Starting instance was successful");
                    }
                    else
                    {
                        //zalogujeme
                        this.InternalTrace("Starting instance was not successful");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    //zalogujeme chybu
                    this.InternalTrace("Starting instance {0} failed", client.GetType());
                    this.InternalTrace(ex);
                    return false;
                }
            }
            return true;
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
        /// <param name="instanceType">Typ pluginu ktory hladame</param>
        /// <returns>Kolekcia typov</returns>
        private List<Type> InternalGetAllInstanceType(List<Assembly> assemblies, Type instanceType)
        {
            List<Type> collection = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (!type.IsInterface && !type.IsAbstract)
                            {
                                if (type.GetInterface(instanceType.FullName) != null)
                                {
                                    collection.Add(type);
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException exception)
                {
                    //zapiseme chybu do logu
                    exception.Print();
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
                    this.InternalTrace(TraceTypes.Error, "Initializing plugin {0} failed", type);
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
            //trace
            this.InternalTrace(TraceTypes.Verbose, message, args);
        }
        /// <summary>
        /// Zaloguje spravu do suboru
        /// </summary>
        /// <param name="type">Trace type for log</param>
        /// <param name="message">Sprava</param>
        /// <param name="args">String.Format argumenty pre spravu</param>
        private void InternalTrace(TraceTypes type, String message, params Object[] args)
        {
            //check type
            if (type == this.TraceType || (type == TraceTypes.Error && this.TraceErrorAlways))
            {
                //update message
                message = String.Format(message, args);

                //trace
                TraceLogger.Trace(type, message);
            }
        }
        /// <summary>
        /// Zaloguje chybu do log suboru
        /// </summary>
        /// <param name="exception">Chyba ktoru chceme zalogovat</param>
        private void InternalTrace(Exception exception)
        {
            //check type
            if (this.TraceType == TraceTypes.Error || this.TraceErrorAlways)
            {
                TraceLogger.Error(exception);
            }
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
            this.InternalTrace("Loading of available plugins... [{0}]", path);

            //nacitame vsetky dostupne kniznice
            String[] files = this.InternalGetAllLibrary(path);

            //zalogujeme
            this.InternalTrace("Number of available library: {0}", files == null ? 0 : files.Length);

            //nacitame vsetky assemblies
            List<Assembly> assemblies = this.InternalGetAllAssemblyFromFiles(files);

            //zalogujeme
            this.InternalTrace("Number of available assemblies: {0}", assemblies == null ? 0 : assemblies.Count);

            //nacitame dostupne typy
            List<Type> types = this.InternalGetAllInstanceType(assemblies, typeof(IWindowsServiceClient));

            //zalogujeme
            this.InternalTrace("Number of available type: {0}", types == null ? 0 : types.Count);

            //inicializujeme instancie
            List<IWindowsServiceClient> clients = this.InternalCreateInstance(types);

            //zalogujeme
            this.InternalTrace("Number of available instances: {0}", clients == null ? 0 : clients.Count);

            //sort clients
            clients.Sort((a, b) => a.ServiceClientPriority.CompareTo(b.ServiceClientPriority));

            //vratime dostupne instancie
            return clients;
        }
        #endregion

        #region - Protected Method -
        /// <summary>
        /// This function return trace type for all clients
        /// </summary>
        /// <returns>TraceType</returns>
        protected abstract TraceTypes InternalGetTraceType();
        #endregion
    }
}

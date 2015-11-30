using Project858.ComponentModel.Client;
using Project858.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace Project858.Web
{
    /// <summary>
    /// Klient zabezpecujuci jednoduche statistiky navstevnosti stranok
    /// </summary>
    public sealed class AnalyticsClient : ClientBase
    {
        #region - Properties -
        /// <summary>
        /// Timeout na obsluhu klienta
        /// </summary>
        private int ClientTimeout { get; set; }
        #endregion

        #region - Variables -
        /// <summary>
        /// Objekt na synchronizaciu pristupu
        /// </summary>
        private readonly Object m_lockObject = new Object();
        /// <summary>
        /// Posledny datum operacie
        /// </summary>
        private Nullable<DateTime> m_lastDate = null;
        /// <summary>
        /// Timer na obsluhu klienta
        /// </summary>
        private Timer m_timer = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vytvori statistiku z aktualneho requestu
        /// </summary>
        /// <param name="request">Aktualny request</param>
        public void Process(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            lock (this.m_lockObject)
            {
                this.InternalProcess(request);
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
            Boolean flag = base.InternalStart();
            if (flag)
            {
                this.InternalInitializeTimer();
            }
            return flag;
        }
        /// <summary>
        /// Ukonci funkciu klienta
        /// </summary>
        protected override void InternalStop()
        {
            this.InternalDeinitializeTimer();
            base.InternalStop();
        }
        /// <summary>
        /// Vykona pred ukoncenim klienta
        /// </summary>
        protected override void InternalDispose()
        {
            this.InternalDeinitializeTimer();
            base.InternalDispose();
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vytvori statistiku z aktualneho requestu
        /// </summary>
        /// <param name="request">Aktualny request</param>
        public void InternalProcess(HttpRequest request)
        {

        }
        /// <summary>
        /// Vykona inicializaciu timra na obsluhu klienta
        /// </summary>
        private void InternalInitializeTimer()
        {
            if (this.m_timer != null)
            {
                this.InternalDeinitializeTimer();
            }
            this.m_timer = new Timer(new TimerCallback(this.InternalTick), null, this.ClientTimeout, this.ClientTimeout);
        }
        /// <summary>
        /// Vykona deinicializaciu timra na obsluhu klienta
        /// </summary>
        private void InternalDeinitializeTimer()
        {
            if (this.m_timer != null)
            {
                this.m_timer.Dispose();
            }
            this.m_timer = null;
        }
        /// <summary>
        /// Obsluha timra
        /// </summary>
        /// <param name="state">Object</param>
        private void InternalTick(Object state)
        {
            //osetrenie
            if (this.m_timer == null)
                return;

            try
            {
                //dalsi tick timra v nekonecne
                this.m_timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                //chybu ignorujeme
                return;
            }

            try
            {
                //kontrola satvu napajania
                this.ClientProcess();
            }
            catch (Exception ex)
            {
                //zalogujeme chybu
                this.InternalTrace(TraceTypes.Error, ex.ToString());
            }

            try
            {
                //spustime tick o prislusny interval
                if (this.m_timer != null)
                    this.m_timer.Change(this.ClientTimeout, this.ClientTimeout);
            }
            catch (ObjectDisposedException)
            {
                //chybu ignorujeme
                return;
            }
        }
        /// <summary>
        /// Vykona cinnost klienta
        /// </summary>
        private void ClientProcess()
        {

        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Project858.ComponentModel.Client
{
    /// <summary>
    /// Predpis pre implementaciu jednodlivych modulov / klientov v ramci aplikacie
    /// </summary>
    public interface IClient : IDisposable
    {
        #region - Event -
        /// <summary>
        /// Event oznamujuci start klienta
        /// </summary>
        event EventHandler ClientStartEvent;
        /// <summary>
        /// Event oznamujuci pozastavenie klienta
        /// </summary>
        event EventHandler ClientPauseEvent;
        /// <summary>
        /// Event oznamujuci ukoncenie klienta
        /// </summary>
        event EventHandler ClientStopEvent;
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Definuje ci bolo na tiedu zavolane Dispose()
        /// </summary>
        Boolean IsDisposed { get; }
        /// <summary>
        /// (Get) Urcuje ci je funkcia vrstvy spustena
        /// </summary>
        Boolean IsRun { get; }
        /// <summary>
        /// (Get / Set) Nastavi asynchronny mod na event oznamujuci start klienta
        /// </summary>
        Boolean ClientStartEventAsync { get; set; }
        /// <summary>
        /// (Get / Set) Nastavi asynchronny mod na event oznamujuci pozastavenie klienta
        /// </summary>
        Boolean ClientPauseEventAsync { get; set; }
        /// <summary>
        /// (Get / Set) Nastavi asynchronny mod na event oznamujuci ukoncenie klienta
        /// </summary>
        Boolean ClientStopEventAsync { get; set; }
        /// <summary>
        /// (Get) Vrati stav v akom sa klient nachádza
        /// </summary>
        ClientStates ClientState { get; }
        #endregion

        #region - Method -
        /// <summary>
        /// Spusti funkciu vrstvy
        /// </summary>
        /// <returns>True = vrstva bola uspesne spustena</returns>
        Boolean Start();
        /// <summary>
        /// Pozastavi funkciu vrstvy
        /// </summary>
        void Pause();
        /// <summary>
        /// Ukonci funkciu vrstvy
        /// </summary>
        void Stop();
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Project858.Diagnostics;

namespace Project858.Net
{
    /// <summary>
    /// Nadstavba TCP servera s rozsirenim
    /// </summary>
    public abstract class TcpContractServer : TcpServer
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public TcpContractServer()
            : base()
        {
        }
        /// <summary>
		/// Initialize this class
		/// </summary>
		/// <param name="ipAddress">Ip adresa servera</param>
		/// <param name="ipPort">Ip port servera</param>
        public TcpContractServer(IPAddress ipAddress, Int32 ipPort)
            : base(ipAddress, ipPort)
        {
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// (Get) Kolekcia aktualne pripojenych klientov a ich obsluh
        /// </summary>
        public List<ITransportClient> Contracts
        {
            get { return m_contracts; }
        }
        /// <summary>
        /// (Get / Set) Maximalny pocet pripojenych klientov. Povoleny rozsah je od 1 do 100
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Mimo povoleneho rozsahu. Povoleny rozsah je od 1 do 100
        /// </exception>
        public Int32 IsConnected
        {
            get
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                return this.m_maxConnection;
            }
            set
            {
                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                //osetrenie rozsahu
                if (value < 1 || value > 100)
                    throw new ArgumentOutOfRangeException("value");

                //nastavime hodnotu
                m_maxConnection = value;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Maximalny pocet pripojenych klientov
        /// </summary>
        private Int32 m_maxConnection = 10;
        /// <summary>
        /// Kolekcia aktualne pripojenych klientov a ich obsluh
        /// </summary>
        private List<ITransportClient> m_contracts = null;
        #endregion

        #region - Protected and Private Method -
        /// <summary>
        /// Inicializuje server na pzoadovanej adrese a porte
        /// </summary>
        /// <returns>True = start klienta bol uspesny</returns>
        protected override bool InternalStart()
        {
            //spustime tcp server
            if (!base.InternalStart())
            {
                //start servera sa nepodaril
                return false;
            }

            //inicializujeme
            this.m_contracts = new List<ITransportClient>();

            //start servera bol uspesny
            return true;
        }
        /// <summary>
        /// Ukonci server a akceptovanie klientov
        /// </summary>
        protected override void InternalStop()
        {
            try
            {
                //ukoncime base server
                base.InternalStop();

                //ukoncime contracty
                this.InternalStopAllContract();
            }
            catch (Exception)
            {
                //preposleme vynimku vyssie
                throw;
            }
            finally
            {
                //deinicializujeme
                this.m_contracts.Clear();
                this.m_contracts = null;
            }
        }
        /// <summary>
        /// Inicializuje, spusti a vrati contract na obsluhu akceptovaneho klienta
        /// </summary>
        /// <param name="client">Client ktory bol akceptovany</param>
        /// <returns>Contract na obsluhu akceptovaneho klienta</returns>
        protected virtual ITransportClient InternalCreateContract(TcpClient client)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Odoberie contract klienta ktory sa odpojil. Prepis metody by mal zabezpecit volanie base.InternalRemoveContract(ITransportClient contrat);
        /// </summary>
        /// <param name="contrat">Contract ktory vykonaval obsluhu klienta ktory zrusil spojenie</param>
        protected virtual void InternalRemoveContract(ITransportClient contrat)
        {
            //odstranime klienta
            this.InternalRemoveSpecifiedContract(contrat);
        }
        /// <summary>
        /// Ukonci vsetky contracty pred ukoncim celeho servera
        /// </summary>
        protected virtual void InternalStopAllContract()
        {
            //ukoncime beziace contracty
            lock (this)
            {
                for (int i = 0; i < this.m_contracts.Count; i++)
                {
                    try
                    {
                        //ziskame pristup
                        ITransportClient contract = this.m_contracts[i];

                        //ukoncime contract
                        this.InternalStopContract(contract);
                    }
                    catch (Exception)
                    {
                        //chybu ignorujeme
                    }
                }
            }
        }
        /// <summary>
        /// Ukonci contract pred ukoncenim celeho servera
        /// </summary>
        /// <param name="contract">Contract ktory chceme ukoncit</param>
        protected virtual void InternalStopContract(ITransportClient contract)
        {
            //ukoncime klienta
            if (!contract.IsDisposed)
                if (contract.IsRun)
                {
                    //odoberieme event oznamujuci odhlasenie klienta
                    contract.DisconnectedEvent -= new EventHandler(contract_DisconnectedEvent);
                    contract.Stop();
                }
        }
        /// <summary>
        /// Prepis a rozsirenie prijatia / akceptovania klienta
        /// </summary>
        /// <param name="e">TcpClientEventArgs</param>
        protected override void OnTcpClientReceived(TcpClientEventArgs e)
        {
            //base volanie na vytvorenie eventu
            base.OnTcpClientReceived(e);

            //overime ci je mozne pridat dalsieho klienta
            if (this.InternalCheckContracts())
            {
                try
                {
                    //pridame instaniu servera
                    ITransportClient contract = this.InternalCreateContract(e.Client);

                    if (contract != null && contract.IsConnected)
                    {
                        //namapujeme event oznamujuci odhlasenie klienta
                        contract.DisconnectedEvent += new EventHandler(contract_DisconnectedEvent);

                        //pridame contract 
                        this.InternalCreateContract(contract);
                    }
                    else
                    {
                        //ukoncime inicializovany contract
                        if (contract != null)
                        {
                            contract.Dispose();
                            contract = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //chybu ignorujeme
                    this.InternalTrace(TraceTypes.Error, "Interna chyba pri udalostiach spojenych z pridavanim pripojeneho klienta. {0}", ex.Message);
                }
            }
            else
            {
                try
                {
                    //zalogujeme
                    this.InternalTrace(TraceTypes.Verbose, "Ukoncovanie akceptovaneho klienta...");

                    //ziskame pristup
                    TcpClient client = e.Client;
                    client.Close();
                }
                catch (Exception)
                {
                    //ignorujeme
                }
            }
        }
        /// <summary>
        /// Reaguje na ukoncenie klienta. Odoberie contract z kolekcie
        /// </summary>
        /// <param name="sender">Odosielatel udalosti</param>
        /// <param name="e">EventArgs</param>
        private void contract_DisconnectedEvent(object sender, EventArgs e)
        {
            //zalogujeme
            this.InternalTrace(TraceTypes.Verbose, "Odoberanie ukonceneho klienta.");

            //ziskame pristup
            ITransportClient client = sender as ITransportClient;

            try
            {
                //odoberame klienta
                this.InternalRemoveContract(client);
            }
            catch (Exception ex)
            {
#if DEBUG
                //chybu ignorujeme
				ConsoleLogger.WriteLine("Interna chyba pri udalostiach spojenych z odstranovanim ukonceneho klienta. {0}", ex.ToString());
#endif
                this.InternalTrace(TraceTypes.Error, "Interna chyba pri udalostiach spojenych z odstranovanim ukonceneho klienta. {0}", ex.Message);
            }
        }
        /// <summary>
        /// Prida instanciu ktora obsluhuje prijateho / akceptovaneho klienta
        /// </summary>
        /// <param name="contract">Contract obsluhujuci akceptovaneho klienta</param>
        private void InternalCreateContract(ITransportClient contract)
        {
            //overime instaniu
            if (contract == null || contract.IsRun == false || contract.IsDisposed)
                return;

            lock (this)
            {
                //pridame dalsi contract do kolekcia
                this.m_contracts.Add(contract);
#if DEBUG
				ConsoleLogger.WriteLine("Pocet klientov: po prijati dalsieho {0}", this._contracts.Count);
#endif
            }
        }
        /// <summary>
        /// Odoberie instanciu ktora obsluhovala prijateho / akceptovaneho klienta ktory ukoncil spojenie
        /// </summary>
        /// <param name="contract">Contract obsluhujuci akceptovaneho klienta</param>
        private void InternalRemoveSpecifiedContract(ITransportClient contract)
        {
            //overime instaniu
            if (contract == null)
                return;

            lock (this)
            {
				//zalofujeme
                this.InternalTrace(TraceTypes.Error, "Pocet klientov: pred odobratim dalsieho {0}", this.m_contracts.Count);
                //odoberieme contract
                this.m_contracts.Remove(contract);
				//zalogujeme
                this.InternalTrace(TraceTypes.Error, "Pocet klientov: po odobrati dalsieho {0}", this.m_contracts.Count);
            }

			//ukoncime inicializovany contract
			if (contract != null)
			{
				contract.Dispose();
				contract = null;
			}
        }
        /// <summary>
        /// Overi maximalny pocet moznych klientov a vykona start alebo stop listenera
        /// </summary>
        /// <returns>True = je mozne aktivovat dalsieho klient</returns>
        private Boolean InternalCheckContracts()
        {
            //ziskame pocet aktualnych instancii
			Int32 count = 0;
			
			//synchronizujeme pristup
			lock (this)
			{
				count = this.m_contracts.Count;
			}

            //zalogujeme
            this.InternalTrace(TraceTypes.Verbose, "Overovanie stavu listenera. {0}", count);

            //ak je pocet vacsi ako povoleny
            if (count >= this.m_maxConnection)
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Maximalny pocet pripojenych klientov bol dosiahnuty. [IsListened: {0}]", 
                    this.IsListened);

                return false;
            }
            //ak este mozme prijimat klientov
            else
            {
                //zalogujeme
                this.InternalTrace(TraceTypes.Verbose, "Maximalny pocet pripojenych klientov este nebol dosiahnuty. [IsListened: {0}]", 
                    this.IsListened);

                return true;
            }
        }
        #endregion
    }
}

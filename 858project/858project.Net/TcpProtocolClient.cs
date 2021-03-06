﻿using Project858.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// Implement TCP client with custom protocol
    /// </summary>
    public class TcpProtocolClient : TcpTransportClient
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovana IPAddress
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybny argument / rozsah IP portu
        /// </exception>
        /// <param name="ipEndPoint">IpPoint ku ktoremu sa chceme pripojit</param>
        public TcpProtocolClient(IPEndPoint ipEndPoint)
            : base(ipEndPoint)
        {

        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovana IPAddress
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybny argument / rozsah IP portu
        /// </exception>
        /// <param name="ipEndPoints">Kolekcia IpEndPointov ku ktory sa bude klient striedavo pripajat pri autoreconnecte</param>
        public TcpProtocolClient(IpEndPointCollection ipEndPoints)
            : base(ipEndPoints)
		{

        }
        /// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovana IPAddress
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov, alebo IP portu
		/// </exception>
        /// <param name="ipEndPoint">IpPoint ku ktoremu sa chceme pripojit</param>
		/// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
		/// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
		/// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
		/// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
        public TcpProtocolClient(IPEndPoint ipEndPoint, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
            : base(ipEndPoint, tcpReadTimeout, tcpWriteTimeout, nsReadTimeout, nsWriteTimeout)
        {

        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Neinicializovana IPAddress
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybny argument jedneho z timeoutov, alebo IP portu
        /// </exception>
        /// <param name="ipEndPoints">Kolekcia IpEndPointov ku ktory sa bude klient striedavo pripajat pri autoreconnecte</param>
        /// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
        /// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
        /// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
        /// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
        public TcpProtocolClient(IpEndPointCollection ipEndPoints, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
            : base(ipEndPoints, tcpReadTimeout, tcpWriteTimeout, nsReadTimeout, nsWriteTimeout)
        {
        }
        /// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovany TcpClient
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		/// Ak je RemoteEndPoint v tcpClientovi Disposed
		/// </exception>
		/// <param name="tcpClient">Client zabezpecujuci tcp komunikaciu</param>
		public TcpProtocolClient(TcpClient tcpClient)
			: base(tcpClient)
		{
		}
		/// <summary>
		/// Initialize this class
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Neinicializovany TcpClient
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Chybny argument jedneho z timeoutov
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		/// Ak je RemoteEndPoint v tcpClientovi Disposed
		/// </exception>
		/// <param name="tcpClient">Client zabezpecujuci tcp komunikaciu</param>
		/// <param name="tcpReadTimeout">TcpClient Read Timeout</param>
		/// <param name="tcpWriteTimeout">TcpClient Write Timeout</param>
		/// <param name="nsReadTimeout">NetworkStream Read Timeout</param>
		/// <param name="nsWriteTimeout">NetworkStream Write Timeout</param>
        public TcpProtocolClient(TcpClient tcpClient, Int32 tcpReadTimeout, Int32 tcpWriteTimeout, Int32 nsReadTimeout, Int32 nsWriteTimeout)
            : base(tcpClient, tcpReadTimeout, tcpWriteTimeout, nsReadTimeout, nsWriteTimeout)
		{

		}
        #endregion

        #region - Event -
        /// <summary>
        /// Event na oznamenue prichodu frame na transportnej vrstve
        /// </summary>
        private event PackageEventHandler m_receivedFrameEvent = null;
        /// <summary>
        /// Event na oznamenue prichodu frame na transportnej vrstve
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        public event PackageEventHandler ReceivedFrameEvent
        {
            add
            {

                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                lock (this.m_eventLock)
                    this.m_receivedFrameEvent += value;
            }
            remove
            {

                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Object was disposed");

                //netusim preco ale to mi zvykne zamrznut thread a ja fakt neviem preco
                lock (this.m_eventLock)
                    this.m_receivedFrameEvent -= value;
            }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Synchronization object
        /// </summary>
        private readonly Object m_lockObject = new Object();
        /// <summary>
        /// Buffer collection for processing data
        /// </summary>
        private List<Byte> m_buffer = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This function sends frame to transport layer
        /// </summary>
        /// <param name="frame">Frame to send</param>
        /// <returns>True | false</returns>
        public virtual Boolean Send(Package frame)
        {
            return this.Write(frame.ToByteArray());
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
        /// oznamujuceho prijatie dat
        /// </summary>
        /// <param name="e">DataEventArgs</param>
        protected override void OnReceivedData(DataEventArgs e)
        {
            //base event
            base.OnReceivedData(e);

            //lock
            lock (this.m_lockObject)
            {
                //check buffer
                if (this.m_buffer == null)
                {
                    this.m_buffer = new List<Byte>();
                }

                //add data to buffer
                this.m_buffer.AddRange(e.Data);

                //loop
                while (true)
                {
                    //find frame
                    Package frame = PackageHelper.FindFrame(this.m_buffer, this.InternalGetFrameItemType);
                    if (frame != null)
                    {
                        //send receive event
                        this.OnReceivedFrame(new PackageEventArgs(frame, e.RemoteEndPoint));
                    }
                    else
                    {
                        //any data for frame
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// This function returns frame item type from address
        /// </summary>
        /// <param name="address">Frame address</param>
        /// <param name="itemAddress">Address to detect type</param>
        /// <returns>Frame item type</returns>
        protected virtual PackageItemTypes InternalGetFrameItemType(UInt16 address, UInt32 itemAddress)
        {
            switch (itemAddress)
            {
                case Package.Defines.TAG_STATE:
                    return PackageItemTypes.Byte;
                default:
                    return PackageItemTypes.Unkown;
            }
        }
        #endregion

        #region - Call Event Method -
        /// <summary>
        /// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
        /// oznamujuceho prijatie dat
        /// </summary>
        /// <param name="e">EventArgs obsahujuci data</param>
        protected virtual void OnReceivedFrame(PackageEventArgs e)
        {
            PackageEventHandler handler = this.m_receivedFrameEvent;

            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// Implement TCP client with custom protocol
    /// </summary>
    public abstract class TcpProtocolClient : TcpTransportClient
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
        #endregion

        #region - Event -
        /// <summary>
        /// Event na oznamenue prichodu frame na transportnej vrstve
        /// </summary>
        private event FrameEventHandler m_receivedFrameEvent = null;
        /// <summary>
        /// Event na oznamenue prichodu frame na transportnej vrstve
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Ak je object v stave _isDisposed
        /// </exception>
        public event FrameEventHandler ReceivedFrameEvent
        {
            add
            {

                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

                lock (this.m_eventLock)
                    this.m_receivedFrameEvent += value;
            }
            remove
            {

                //je objekt _isDisposed ?
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.ToString(), "Object was disposed.");

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
        public Boolean Send(Frame frame)
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

                //find frame
                Frame frame = this.internalFindFrame(this.m_buffer);
                if (frame != null)
                {
                    //send receive event
                    this.OnReceivedFrame(new FrameEventArgs(frame, e.RemoteEndPoint));
                }
            }
        }
        /// <summary>
        /// This function finds frame in array
        /// </summary>
        /// <param name="array">Input array data</param>
        /// <returns>Frame | null</returns>
        private Frame internalFindFrame(List<Byte> array)
        {
            //variables
            int count = array.Count;

            //find start byte
            for (int index = 0; index < count; index++)
            {
                //check start byte and length
                if (array[index] == 0x68 && (count - (index + 2)) >= 2)
                {
                    //get length from array
                    short length = (short)(array[index + 2] << 8 | array[index + 1]);

                    //get command from array
                    short commandAddress = (short)(array[index + 4] << 8 | array[index + 3]);

                    //overime ci je dostatok dat na vytvorenie package
                    if (length <= (count - 1))
                    {
                        Frame frame = internalConstructFrame(array, index + 5, length - 5, commandAddress);
                        if (frame != null)
                        {
                            //return package
                            return frame;
                        }
                    }
                    else
                    {
                        //remove first data
                        if (index > 0)
                        {
                            array.RemoveRange(0, index + 1);
                        }

                        //nedostatok dat
                        return null;
                    }
                }
            }

            //clear all data from buffer
            array.Clear();

            //any package
            return null;
        }
        /// <summary>
        /// This function constructs frame from data array
        /// </summary>
        /// <param name="array">Data array</param>
        /// <param name="index">Start frame index</param>
        /// <param name="length">Frame length</param>
        /// <param name="commandAddress">Command address from frame</param>
        /// <returns>Frame | null</returns>
        private Frame internalConstructFrame(List<Byte> array, int index, int length, short commandAddress)
        {
            //check data length available
            if ((array.Count - index) >= length)
            {
                //get checksum
                Byte checkSum = this.internalGetFrameDataCheckSum(array, index - 4, length + 4);
                Byte currentCheckSum = array[index + length + 0];
                if (checkSum != currentCheckSum)
                {
                    return null;
                }

                //copy data block
                List<Byte> temp = array.GetRange(index, length);

                //initialize package
                Frame frame = new Frame(commandAddress, temp);

                //remove data
                array.RemoveRange(0, length + index + 1);

                //return package
                return frame;
            }
            return null;
        }
        /// <summary>
        /// This function calculate check sum from frame
        /// </summary>
        /// <param name="array">Data array</param>
        /// <param name="index">Start frame index</param>
        /// <param name="length">Frame length</param>
        /// <returns>Check sum</returns>
        private Byte internalGetFrameDataCheckSum(List<Byte> array, int index, int length)
        {
            int sum = 0;
            for (int currentIndex = index; currentIndex < (length + index); currentIndex++)
            {
                sum += (int)array[currentIndex];
            }
            sum += 0xA5;
            sum = sum & 0xFF;
            return (byte)(256 - sum);
        }
        #endregion

        #region - Call Event Method -
        /// <summary>
        /// Vytvori asynchronne volanie na metodu zabezpecujucu vytvorenie eventu
        /// oznamujuceho prijatie dat
        /// </summary>
        /// <param name="e">EventArgs obsahujuci data</param>
        protected virtual void OnReceivedFrame(FrameEventArgs e)
        {
            FrameEventHandler handler = this.m_receivedFrameEvent;

            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}

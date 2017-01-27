using Project858.Net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Project858.IO.Ports
{
    /// <summary>
    /// Serial port client with protocol
    /// </summary>
    public class SerialPortProtocolClient : SerialPortClient
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        public SerialPortProtocolClient(String portName) :
            base(portName, 9600, Parity.None, 8, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortProtocolClient(String portName, Int32 readTimeout, Int32 writeTimeout) :
            base(portName, 9600, Parity.None, 8, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentu baudRate
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate) :
            base(portName, baudRate, Parity.None, 8, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate, Int32 readTimeout, Int32 writeTimeout) :
            base(portName, baudRate, Parity.None, 8, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentu baudRate 
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate, Parity parity) :
            base(portName, baudRate, parity, 8, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate, Parity parity, Int32 readTimeout, Int32 writeTimeout) :
            base(portName, baudRate, parity, 8, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate, Parity parity, int dataBits) :
            base(portName, baudRate, parity, dataBits, StopBits.One)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate, Parity parity, int dataBits, Int32 readTimeout, Int32 writeTimeout) :
            base(portName, baudRate, parity, dataBits, StopBits.One, readTimeout, writeTimeout)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        /// <param name="stopBits">StopBits</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate, Parity parity, Int32 dataBits, StopBits stopBits) :
            base(portName, baudRate, parity, dataBits, stopBits, 1000, 2000)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Chybna argumentu portName
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Chybna argumentov baudRate, dataBits, readTimeout, writeTimeout
        /// </exception>
        /// <param name="portName">Meno COM portu</param>
        /// <param name="baudRate">Prenosova rychlost</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">DataBits</param>
        /// <param name="stopBits">StopBits</param>
        /// <param name="readTimeout">Timeout na citanie</param>
        /// <param name="writeTimeout">Timeout na zapis</param>
        public SerialPortProtocolClient(String portName, Int32 baudRate, Parity parity, int dataBits, StopBits stopBits, Int32 readTimeout, Int32 writeTimeout)
            : base(portName, baudRate, parity, dataBits, stopBits, readTimeout, writeTimeout)
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
                    Frame frame = this.internalFindFrame(this.m_buffer);
                    if (frame != null)
                    {
                        //send receive event
                        this.OnReceivedFrame(new FrameEventArgs(frame, e.ComPortName));
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
                    UInt16 length = (UInt16)(array[index + 2] << 8 | array[index + 1]);

                    //get command from array
                    UInt16 commandAddress = (UInt16)(array[index + 4] << 8 | array[index + 3]);

                    //overime ci je dostatok dat na vytvorenie package
                    if (count >= (length - 1))
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
        private Frame internalConstructFrame(List<Byte> array, int index, int length, UInt16 commandAddress)
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
                Frame frame = new Frame(commandAddress, temp, this.InternalGetFrameItemType);

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
        /// <summary>
        /// This function returns frame item type from address
        /// </summary>
        /// <param name="address">Frame address</param>
        /// <param name="itemAddress">Address to detect type</param>
        /// <returns>Frame item type</returns>
        protected virtual FrameItemTypes InternalGetFrameItemType(UInt16 address, UInt16 itemAddress)
        {
            return FrameItemTypes.Unkown;
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

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Project858.IO.Ports
{
    public sealed class SerialPortProtocolClient : SerialPortClient
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
    }
}

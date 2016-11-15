using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858;

namespace Project858.Net
{
    /// <summary>
    /// Frame item with Int32 value
    /// </summary>
    public sealed class FrameItemInt32 : FrameItemBase<Int32>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemInt32(UInt16 address, Int32 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemInt32(UInt16 address, Byte[] data)
            : base(address, data)
        {
 
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override Int32 InternalParseValue(Byte[] data)
        {
            Int32 result = data[3] << 24 | data[2] << 16 | data[1] << 8 | data[0];
            return result;
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Int32 value)
        {
            Byte[] result = new Byte[4];
            result[0] = (byte)value;
            result[1] = (Byte)(value >> 8);
            result[2] = (Byte)(value >> 16);
            result[3] = (Byte)(value >> 24);
            return result;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858;

namespace Project858.Net
{
    /// <summary>
    /// Frame item with Int64 value
    /// </summary>
    public sealed class FrameItemUInt64 : FrameItemBase<UInt64>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemUInt64(UInt16 address, UInt64 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemUInt64(UInt16 address, Byte[] data)
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
        protected override UInt64 InternalParseValue(Byte[] data)
        {
            return BitConverter.ToUInt64(data, 0);
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(UInt64 value)
        {
            return BitConverter.GetBytes(value);
        }
        #endregion
    }
}

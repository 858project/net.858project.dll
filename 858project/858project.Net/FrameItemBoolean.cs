using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858;

namespace Project858.Net
{
    /// <summary>
    /// Frame item with Byte value
    /// </summary>
    public sealed class FrameItemBoolean : FrameItemBase<Boolean>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemBoolean(Int16 address, Boolean value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemBoolean(Int16 address, Byte[] data)
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
        protected override Boolean InternalParseValue(Byte[] data)
        {
            return data[0] == 0x01;
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Boolean value)
        {
            Byte[] result = new Byte[] { (byte)(value ? 0x01 : 0x00) };
            return result;
        }
        #endregion
    }
}

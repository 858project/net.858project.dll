﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858;

namespace Project858.Net
{
    /// <summary>
    /// Frame item with Int16 value
    /// </summary>
    public sealed class FrameItemInt16 : FrameItemBase<Int16>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemInt16(Int16 address, Int16 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemInt16(Int16 address, Byte[] data)
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
        protected override Int16 InternalParseValue(Byte[] data)
        {
            Int16 result = (Int16)(data[1] << 8 | data[0]);
            return result;
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Int16 value)
        {
            Byte[] result = new Byte[2];
            result[0] = (Byte)value;
            result[1] = (Byte)(value >> 8);
            return result;
        }
        #endregion
    }
}
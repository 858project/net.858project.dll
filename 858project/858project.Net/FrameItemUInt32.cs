﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858;

namespace Project858.Net
{
    /// <summary>
    /// Frame item with Int32 value
    /// </summary>
    public sealed class FrameItemUInt32 : FrameItemBase<UInt32>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemUInt32(UInt16 address, UInt32 value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemUInt32(UInt16 address, Byte[] data)
            : base(address, data)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override FrameItemTypes ItemType { get { return FrameItemTypes.Int32; } }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("[Int32] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override UInt32 InternalParseValue(Byte[] data)
        {
            return BitConverter.ToUInt32(data, 0);
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(UInt32 value)
        {
            return BitConverter.GetBytes(value);
        }
        #endregion
    }
}
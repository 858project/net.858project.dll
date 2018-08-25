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
    public sealed class FrameItemEnum : FrameItemBase<Object>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemEnum(UInt32 address, Object value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemEnum(UInt32 address, Byte[] data)
            : base(address, data)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override FrameItemTypes ItemType { get { return FrameItemTypes.Enum; } }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("[Enum] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override Object InternalParseValue(Byte[] data)
        {
            if (data.Length == 0x01)
            {
                return data[0];
            }
            else if (data.Length == 0x02)
            {
                return BitConverter.ToInt16(data, 0x00);
            }
            else if (data.Length == 0x04)
            {
                return BitConverter.ToInt32(data, 0x00);
            }
            else if (data.Length == 0x08)
            {
                return BitConverter.ToInt64(data, 0x00);
            }
            else
            {
                throw new Exception(String.Format("Unknown length {0} for enum", data.Length));
            }
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Object value)
        {
            Type type = value.GetType();
            Type baseType = Enum.GetUnderlyingType(type);
            if (baseType == typeof(Byte))
            {
                return BitConverter.GetBytes((Byte)value);
            }
            else if (baseType == typeof(Int16))
            {
                return BitConverter.GetBytes((Int16)value);
            }
            else if (baseType == typeof(Int32))
            {
                return BitConverter.GetBytes((Int32)value);
            }
            else if (baseType == typeof(Int32))
            {
                return BitConverter.GetBytes((Int64)value);
            }
            else
            {
                throw new Exception(String.Format("Unknown type {0} for enum", value.GetType()));
            }
        }
        #endregion
    }
}

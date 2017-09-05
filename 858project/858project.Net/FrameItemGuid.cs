using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858;

namespace Project858.Net
{
    /// <summary>
    /// Frame item with byte array value
    /// </summary>
    public sealed class FrameItemGuid : FrameItemBase<Guid>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemGuid(UInt32 address, Guid value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemGuid(UInt32 address, Byte[] data)
            : base(address, data)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override FrameItemTypes ItemType { get { return FrameItemTypes.Guid; } }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("[Guid] : 0x{0:X4} = {1}", this.Address, this.Value);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override Guid InternalParseValue(Byte[] data)
        {
            if (data.Length == 32)
            {
                String hashValue = Encoding.ASCII.GetString(data);
                Nullable<Guid> value = hashValue.ToGuidWithoutDash();
                if (value.HasValue)
                {
                    return value.Value;
                }
            }
            return new Guid(data);
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override Byte[] InternalParseFromValue(Guid value)
        {
            return value.ToByteArray();
        }
        #endregion
    }
}

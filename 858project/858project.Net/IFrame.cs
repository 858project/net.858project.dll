using System;

namespace Project858.Net
{
    /// <summary>
    /// Protocol frame
    /// </summary>
    public interface IFrame
    {
         #region - Properties -
        /// <summary>
        /// Command address
        /// </summary>
        UInt16 Address
        {
            get;
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Change frame address
        /// </summary>
        /// <param name="address">new address</param>
        void ChangeAddress(UInt16 address);
        /// <summary>
        /// This finction converts frame to byte array
        /// </summary>
        /// <returns>Byte array | null</returns>
        Byte[] ToByteArray();
        #endregion
    }
}

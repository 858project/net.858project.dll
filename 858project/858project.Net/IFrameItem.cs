using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// Base frame item from tcp protocol
    /// </summary>
    public interface IFrameItem
    {
        #region - Properties -
        /// <summary>
        /// Item address
        /// </summary>
        Int16 Address { get; }
        /// <summary>
        /// Data item
        /// </summary>
        Byte[] Data { get; }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// this function returns value
        /// </summary>
        /// <returns>Value</returns>
        Object GetValue();
        /// <summary>
        /// This function returns item data
        /// </summary>
        /// <returns>Item array data</returns>
        Byte[] ToByteArray();
        #endregion
    }
}

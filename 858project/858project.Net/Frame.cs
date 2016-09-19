using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Project858.Net
{
    /// <summary>
    /// Protocol frame
    /// </summary>
    public sealed class Frame
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="commandAddress">Command address</param>
        public Frame(Int16 commandAddress)
            : this(commandAddress, null)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="commandAddress">Command address</param>
        /// <param name="data">Frame data</param>
        public Frame(Int16 commandAddress, List<Byte> data)
        {
            this.CommandAddress = commandAddress;
            this.m_items = new List<FrameItem>();
            if (data != null)
            {
                this.InternalParseFrame(data);
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Command address
        /// </summary>
        public Int16 CommandAddress
        {
            get;
            private set;
        }
        /// <summary>
        /// Item collections
        /// </summary>
        public ReadOnlyCollection<FrameItem> Items
        {
            get { return this.m_items.AsReadOnly(); }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Item collections
        /// </summary>
        private List<FrameItem> m_items = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="item">The item to be added to the end of the Frame</param>
        public void AddItem(FrameItem item)
        {
            this.m_items.Add(item);
        }
        /// <summary>
        /// This finction converts frame to byte array
        /// </summary>
        /// <returns>Byte array | null</returns>
        public Byte[] ToByteArray()
        {
            return null;
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse frame from data
        /// </summary>
        /// <param name="data">Data to parse</param>
        private void InternalParseFrame(List<Byte> data)
        {

        }
        #endregion
    }
}

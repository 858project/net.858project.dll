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
            : this(commandAddress, null, null)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="commandAddress">Command address</param>
        /// <param name="data">Frame data</param>
        public Frame(Int16 commandAddress, List<Byte> data, Func<Int16, FrameItemTypes> action)
        {
            this.CommandAddress = commandAddress;
            this.m_items = new List<IFrameItem>();
            if (data != null && action != null)
            {
                this.InternalParseFrame(data.ToArray(), action);
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
        public ReadOnlyCollection<IFrameItem> Items
        {
            get { return this.m_items.AsReadOnly(); }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Item collections
        /// </summary>
        private List<IFrameItem> m_items = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="item">The item to be added to the end of the Frame</param>
        public void AddItem(IFrameItem item)
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
        /// <param name="action">Function to get frame item type</param>
        private void InternalParseFrame(Byte[] data, Func<Int16, FrameItemTypes> action)
        {
            //vriables
            Int16 address = 0x00;
            Int16 length = 0x00;
            Byte[] dataItem = null;
            FrameItemTypes type = FrameItemTypes.Unkown;
            int count = data.Length;
            
            //parse frame
            for (int i = 0; i < count; i++)
            {
                //read address
                address = (Int16)(data[i + 1] << 8 | data[i]);
                length = (Int16)(data[i + 3] << 8 | data[i + 2]);

                //read frame item type
                type = action(address);
 
                //read data
                dataItem = new Byte[length];
                Buffer.BlockCopy(data, i + 4, dataItem, 0, length);

                //parse 
                IFrameItem item = this.InternalParseFrame(type, address, length, dataItem);
                if (item != null)
                {
                    this.m_items.Add(item);
                }

                //next
                i += 4 + length;
            }
        }
        /// <summary>
        /// This function parse frame item from item type
        /// </summary>
        /// <param name="type">Type of frame</param>
        /// <param name="address">Frame item address</param>
        /// <param name="length">Frame item length</param>
        /// <param name="data">Data frame item</param>
        /// <returns>Frame item or null</returns>
        private IFrameItem InternalParseFrame(FrameItemTypes type, Int16 address, Int16 length, Byte[] data)
        {
            switch (type)
            {
                case FrameItemTypes.DateTime:
                    return new FrameItemDateTime(address, data);
                case FrameItemTypes.Guid:
                    return new FrameItemGuid(address, data);
                case FrameItemTypes.String:
                    return new FrameItemString(address, data);
                default:
                    return new FrameItemUnkown(address, data);
            }
        }
        #endregion
    }
}

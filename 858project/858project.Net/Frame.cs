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
            if (data != null)
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
            return this.InternalToByteArray();
        }
        /// <summary>
        /// This function returns value 
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="address">Address</param>
        /// <returns>Value | null</returns>
        public T GetValue<T>(Int16 address)
        {
            foreach (IFrameItem item in this.m_items)
            {
                if (item.Address == address)
                {
                    return (T)Convert.ChangeType(item.GetValue(), typeof(T));
                }
            }
            return default(T);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This finction converts frame to byte array
        /// </summary>
        /// <returns>Byte array | null</returns>
        private Byte[] InternalToByteArray()
        {
            //variables
            List<Byte> collection = new List<Byte>();
            Int16 length = 0x05;

            //add items
            foreach (IFrameItem item in this.m_items)
            {
                Byte[] data = item.ToByteArray();
                length += (Int16)data.Length;
                collection.AddRange(data);
            }

            //add headers
            Byte[] header = new Byte[5];
            header[0] = 0x68;
            header[1] = (byte)(length);
            header[2] = (byte)(length >> 8);
            header[3] = (byte)(this.CommandAddress);
            header[4] = (byte)(this.CommandAddress >> 8);
            collection.InsertRange(0, header);

            //check sum
            Byte checkSum = this.internalGetFrameDataCheckSum(collection);
            collection.Add(checkSum);

            //return frame as data array
            return collection.ToArray();
        }
        /// <summary>
        /// This function calculate check sum from frame
        /// </summary>
        /// <param name="array">Data array</param>
        /// <returns>Check sum</returns>
        private Byte internalGetFrameDataCheckSum(List<Byte> array)
        {
            int sum = 0;
            int count = array.Count;
            for (int currentIndex = 1; currentIndex < count; currentIndex++)
            {
                sum += (int)array[currentIndex];
            }
            sum += 0xA5;
            sum = sum & 0xFF;
            return (byte)(256 - sum);
        }
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
                type = action != null ? action(address) : FrameItemTypes.Unkown;
 
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
                i += 3 + length;
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

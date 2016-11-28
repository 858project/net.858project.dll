using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
        public Frame(UInt16 commandAddress)
            : this(commandAddress, null, null)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="commandAddress">Command address</param>
        /// <param name="data">Frame data</param>
        public Frame(UInt16 commandAddress, List<Byte> data, Func<UInt16, FrameItemTypes> action)
        {
            this.Address = commandAddress;
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
        public UInt16 Address
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

        #region - Public Static Methods -
        /// <summary>
        /// Create frame from Byte value
        /// </summary>
        /// <param name="commandAddress">Frame command address</param>
        /// <param name="address">Tag address for value</param>
        /// <param name="value">Value as Byte</param>
        /// <returns>New frame</returns>
        public static Frame CreateFrame(UInt16 commandAddress, UInt16 address, Byte value)
        {
            Frame frame = new Frame(commandAddress);
            frame.AddItem(new FrameItemByte(address, value));
            return frame;
        }
        /// <summary>
        /// This function initialize item of type
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="address">Item address for value</param>
        /// <param name="value">Value</param>
        /// <returns>Frame item | null</returns>
        public static IFrameItem CreateFrameItem(FrameItemTypes type, UInt16 address, Object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            switch (type)
            {
                case FrameItemTypes.DateTime:
                    return new FrameItemDateTime(address, (DateTime)value);
                case FrameItemTypes.Guid:
                    return new FrameItemGuid(address, (Guid)value);
                case FrameItemTypes.String:
                    return new FrameItemString(address, (String)value);
                case FrameItemTypes.Int16:
                    return new FrameItemInt16(address, (Int16)value);
                case FrameItemTypes.Int32:
                    return new FrameItemInt32(address, (Int32)value);
                case FrameItemTypes.Int64:
                    return new FrameItemInt64(address, (Int64)value);
                case FrameItemTypes.Byte:
                    return new FrameItemByte(address, (Byte)value);
                case FrameItemTypes.Boolean:
                    return new FrameItemBoolean(address, (Boolean)value);
                case FrameItemTypes.UInt64:
                    return new FrameItemUInt64(address, (UInt64)value);
                default:
                    return new FrameItemUnkown(address, (List<Byte>)value);
            }
        }
        /// <summary>
        /// This function return NET object type from Frame Item type
        /// </summary>
        /// <param name="type">Frame item type</param>
        /// <returns>NET object type</returns>
        public static Type GetObjectTypeFromFrameItemType(FrameItemTypes type)
        {
            switch (type)
            {
                case FrameItemTypes.DateTime:
                    return typeof(DateTime);
                case FrameItemTypes.Guid:
                    return typeof(Guid);
                case FrameItemTypes.Int16:
                    return typeof(Int16);
                case FrameItemTypes.Int32:
                    return typeof(Int32);
                case FrameItemTypes.Int64:
                    return typeof(Int64);
                case FrameItemTypes.String:
                    return typeof(String);
                case FrameItemTypes.Byte:
                    return typeof(Byte);
                case FrameItemTypes.Boolean:
                    return typeof(Boolean);
                case FrameItemTypes.UInt64:
                    return typeof(UInt64);
                default:
                    return typeof(Object);
            }
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Change frame address
        /// </summary>
        /// <param name="address">new address</param>
        public void ChangeAddress(UInt16 address)
        {
            this.Address = address;
        }
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
        public T GetValue<T>(UInt16 address)
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
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            //initialize builder
            StringBuilder builder = new StringBuilder();

            //base information
            builder.AppendFormat("Address: {0}, Items: {1}", this.Address, this.Items.Count);
            builder.Append(Environment.NewLine);

            //items
            foreach (IFrameItem item in this.m_items)
            {
                builder.AppendFormat("[{0}] - [{1} - {1:x4}] - {2}", item.GetType(), item.Address, item.GetValue());
                builder.Append(Environment.NewLine);
            }

            //return string
            return builder.ToString();
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
            header[3] = (byte)(this.Address);
            header[4] = (byte)(this.Address >> 8);
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
        private void InternalParseFrame(Byte[] data, Func<UInt16, FrameItemTypes> action)
        {
            //vriables
            UInt16 address = 0x00;
            UInt16 length = 0x00;
            Byte[] dataItem = null;
            FrameItemTypes type = FrameItemTypes.Unkown;
            int count = data.Length;
            
            //parse frame
            for (int i = 0; i < count; i++)
            {
                //read address
                address = (UInt16)(data[i + 1] << 8 | data[i]);
                length = (UInt16)(data[i + 3] << 8 | data[i + 2]);

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
        private IFrameItem InternalParseFrame(FrameItemTypes type, UInt16 address, UInt16 length, Byte[] data)
        {
            switch (type)
            {
                case FrameItemTypes.DateTime:
                    return new FrameItemDateTime(address, data);
                case FrameItemTypes.Guid:
                    return new FrameItemGuid(address, data);
                case FrameItemTypes.String:
                    return new FrameItemString(address, data);
                case FrameItemTypes.Int16:
                    return new FrameItemInt16(address, data);
                case FrameItemTypes.Int32:
                    return new FrameItemInt32(address, data);
                case FrameItemTypes.Int64:
                    return new FrameItemInt64(address, data);
                case FrameItemTypes.Byte:
                    return new FrameItemByte(address, data);
                case FrameItemTypes.Boolean:
                    return new FrameItemBoolean(address, data);
                case FrameItemTypes.UInt64:
                    return new FrameItemUInt64(address, data);
                default:
                    return new FrameItemUnkown(address, data);
            }
        }
        #endregion
    }
}

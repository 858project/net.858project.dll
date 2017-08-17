using Project858.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;

namespace Project858.Net
{
    /// <summary>
    /// Protocol frame
    /// </summary>
    public sealed class Frame : IFrame
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
        /// <param name="address">Command address</param>
        /// <param name="data">Frame data</param>
        /// <param name="action">Action for returning frame item type</param>
        public Frame(UInt16 address, List<Byte> data, Func<UInt16, UInt32, FrameItemTypes> action)
        {
            this.Address = address;
            this.m_items = new List<IFrameItem>();
            if (data != null)
            {
                this.InternalParseFrame(data.ToArray(), action);
            }
        }
        #endregion

        #region - Public Class -
        /// <summary>
        /// Define base value for frame
        /// </summary>
        public static class Defines
        {
            /// <summary>
            /// Command for testing connection
            /// </summary>
            public const UInt16 CP_TEST = 0xFFFF;
            /// <summary>
            /// Command for sending state
            /// </summary>
            public const UInt16 CP_STATE = 0x0001;

            ///
            /// State tag, see STATE* constants
            ///
            public const UInt16 TAG_STATE = 0xFF01;

            /// <summary>
            /// State OK
            /// </summary>
            public const Byte STATE_OK = 0x00;
            /// <summary>
            /// Frame contains data
            /// </summary>
            public const Byte STATE_DATA = 0x01;
            /// <summary>
            /// In the target system has experienced an internal error
            /// </summary>
            public const Byte STATE_INTERNAL_ERROR = 0x02;
            /// <summary>
            /// Required data or operation are not available
            /// </summary>
            public const Byte STATE_NOT_AVAILABLE = 0x03;
            /// <summary>
            /// The target system is busy
            /// </summary>
            public const Byte STATE_BUSY = 0x04;
            /// <summary>
            /// Command was denied
            /// </summary>
            public const Byte STATE_DENIED = 0x05;
            /// <summary>
            /// Start sequence, after this response continues frames with data, see STATE_START_SEQUENCE
            /// </summary>
            public const Byte STATE_START_SEQUENCE = 0x0B;
            /// <summary>
            /// End sequence, see STATE_START_SEQUENCE
            /// </summary>
            public const Byte STATE_END_SEQUENCE = 0x0C;
            /// <summary>
            /// Frame contains message
            /// </summary>
            public const Byte STATE_MESSAGE = 0x0D;
            /// <summary>
            /// Command was canceled
            /// </summary>
            public const Byte STATE_CANCELED = 0x0E;
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
        /// <param name="frameAddress">Frame address</param>
        /// <param name="address">Tag address for value</param>
        /// <param name="value">Value as Byte</param>
        /// <returns>New frame</returns>
        public static Frame CreateFrame(UInt16 frameAddress, UInt16 address, Byte value)
        {
            Frame frame = new Frame(frameAddress);
            frame.Add(new FrameItemByte(address, value));
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
                    {
                        if (value is Guid)
                        {
                            return new FrameItemString(address, ((Guid)value).ToStringWithoutDash());
                        }
                        else
                        {
                            return new FrameItemString(address, (String)value);
                        }
                    }
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
                case FrameItemTypes.UInt32:
                    return new FrameItemUInt32(address, (UInt32)value);
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
                case FrameItemTypes.UInt32:
                    return typeof(UInt32);
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
        ///  Removes the first occurrence of a specific object with address
        /// </summary>
        /// <param name="address">Specific address</param>
        public void Remove(UInt16 address)
        {
            for (int i = 0; i < this.m_items.Count; i++) 
            {
                IFrameItem item = this.m_items[i];
                if (item.Address == address)
                {
                    this.Remove(item);
                    return;
                }
            }
        }
        /// <summary>
        ///  Removes the first occurrence of a specific object
        /// </summary>
        /// <param name="item">The object to remove</param>
        public void Remove(IFrameItem item)
        {
            this.m_items.Remove(item);
        }
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="item">The item to be added to the end of the Frame</param>
        public void Add(IFrameItem item)
        {
            this.m_items.Add(item);
        }
        /// <summary>
        /// Inserts an item into the Frame at the specified index.
        /// </summary>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        public void Add(IFrameItem item, int index)
        {
            this.m_items.Insert(index, item);
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
        /// This function returns frame item with address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Item | null</returns>
        public IFrameItem GetFrameItem(UInt32 address)
        {
            foreach (IFrameItem item in this.m_items)
            {
                if (item.Address == address)
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// This function returns value 
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="address">Address</param>
        /// <returns>Value | null</returns>
        public T GetValue<T>(UInt32 address)
        {
            foreach (IFrameItem item in this.m_items)
            {
                if (item.Address == address)
                {
                    try
                    {
                        return (T)Convert.ChangeType(item.GetValue(), typeof(T));
                    }
                    catch (Exception ex)
                    {
                        Object value = item.GetValue();
                        throw new InvalidCastException(String.Format("Value: {0}, Type: {1}", (value == null ? "NULL" : value.ToString()), typeof(T)), ex);
                    }
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
            Byte checkSum = FrameHelper.GetFrameDataCheckSum(collection, 0x01, collection.Count - 0x01);
            collection.Add(checkSum);

            //return frame as data array
            return collection.ToArray();
        }
        /// <summary>
        /// This function parse frame from data
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="action">Function to get frame item type</param>
        private void InternalParseFrame(Byte[] data, Func<UInt16, UInt32, FrameItemTypes> action)
        {
            //vriables
            UInt32 address = 0x00;
            UInt16 length = 0x00;
            Byte[] dataItem = null;
            FrameItemTypes type = FrameItemTypes.Unkown;
            int count = data.Length;
            
            //parse frame
            for (int i = 0; i < count; i++)
            {
                //read address
                address = (UInt32)(data[i + 3] << 24 | data[i + 2] << 16 | data[i + 1] << 8 | data[i]);
                length = (UInt16)(data[i + 5] << 8 | data[i + 4]);

                //read frame item type
                type = action != null ? action(this.Address, address) : FrameItemTypes.Unkown;
 
                //read data
                dataItem = new Byte[length];
                Buffer.BlockCopy(data, i + 6, dataItem, 0, length);

                //parse 
                IFrameItem item = this.InternalParseFrame(type, address, length, dataItem);
                if (item != null)
                {
                   this.m_items.Add(item);
                }

                //next
                i += 5 + length;
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
        private IFrameItem InternalParseFrame(FrameItemTypes type, UInt32 address, UInt16 length, Byte[] data)
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
                case FrameItemTypes.UInt32:
                    return new FrameItemUInt32(address, data);
                default:
                    return new FrameItemUnkown(address, data);
            }
        }
        #endregion
    }
}

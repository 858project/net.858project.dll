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
    public sealed class Frame : IPackage
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
        public Frame(UInt16 address, List<Byte> data, Func<UInt16, UInt32, PackageItemTypes> action)
        {
            this.Address = address;
            this.m_items = new List<IPackageItem>();
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
            public const UInt32 TAG_STATE = 0xFFFFFFFF;

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
        public ReadOnlyCollection<IPackageItem> Items
        {
            get { return this.m_items.AsReadOnly(); }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Item collections
        /// </summary>
        private List<IPackageItem> m_items = null;
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
            frame.Add(new PackageItemByte(address, value));
            return frame;
        }
        /// <summary>
        /// This function initialize item of type
        /// </summary>
        /// <param name="type">Item type</param>
        /// <param name="address">Item address for value</param>
        /// <param name="value">Value</param>
        /// <returns>Frame item | null</returns>
        public static IPackageItem CreateFrameItem(PackageItemTypes type, UInt32 address, Object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            switch (type)
            {
                case PackageItemTypes.DateTime:
                    return new PackageItemDateTime(address, (DateTime)value);
                case PackageItemTypes.Guid:
                    return new PackageItemGuid(address, (Guid)value);
                case PackageItemTypes.String:
                    {
                        if (value is Guid)
                        {
                            return new PackageItemString(address, ((Guid)value).ToStringWithoutDash());
                        }
                        else
                        {
                            return new PackageItemString(address, (String)value);
                        }
                    }
                case PackageItemTypes.Int16:
                    return new PackageItemInt16(address, (Int16)value);
                case PackageItemTypes.Int32:
                    return new PackageItemInt32(address, (Int32)value);
                case PackageItemTypes.Int64:
                    return new PackageItemInt64(address, (Int64)value);
                case PackageItemTypes.Byte:
                    return new PackageItemByte(address, (Byte)value);
                case PackageItemTypes.Boolean:
                    return new PackageItemBoolean(address, (Boolean)value);
                case PackageItemTypes.UInt16:
                    return new PackageItemUInt16(address, (UInt16)value);
                case PackageItemTypes.UInt32:
                    return new PackageItemUInt32(address, (UInt32)value);
                case PackageItemTypes.UInt64:
                    return new PackageItemUInt64(address, (UInt64)value);
                case PackageItemTypes.Enum:
                    return new PackageItemEnum(address, value);
                default:
                    return new PackageItemUnkown(address, (List<Byte>)value);
            }
        }
        /// <summary>
        /// This function return NET object type from Frame Item type
        /// </summary>
        /// <param name="type">Frame item type</param>
        /// <returns>NET object type</returns>
        public static Type GetObjectTypeFromFrameItemType(PackageItemTypes type)
        {
            switch (type)
            {
                case PackageItemTypes.DateTime:
                    return typeof(DateTime);
                case PackageItemTypes.Guid:
                    return typeof(Guid);
                case PackageItemTypes.Int16:
                    return typeof(Int16);
                case PackageItemTypes.Int32:
                    return typeof(Int32);
                case PackageItemTypes.Int64:
                    return typeof(Int64);
                case PackageItemTypes.String:
                    return typeof(String);
                case PackageItemTypes.Byte:
                    return typeof(Byte);
                case FrameItemTypes.Enum:
                    return typeof(Object);
                case FrameItemTypes.Boolean:
                    return typeof(Boolean);
                case PackageItemTypes.UInt64:
                    return typeof(UInt64);
                case PackageItemTypes.UInt32:
                    return typeof(UInt32);
                case PackageItemTypes.Enum:
                    return typeof(Enum);
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
                IPackageItem item = this.m_items[i];
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
        public void Remove(IPackageItem item)
        {
            this.m_items.Remove(item);
        }
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="item">The item to be added to the end of the Frame</param>
        public void Add(IPackageItem item)
        {
            this.m_items.Add(item);
        }
        /// <summary>
        /// Inserts an item into the Frame at the specified index.
        /// </summary>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        public void Add(IPackageItem item, int index)
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
        public IPackageItem GetFrameItem(UInt32 address)
        {
            foreach (IPackageItem item in this.m_items)
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
            foreach (IPackageItem item in this.m_items)
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
            foreach (IPackageItem item in this.m_items)
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
            foreach (IPackageItem item in this.m_items)
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
            Byte checkSum = PackageHelper.GetFrameDataCheckSum(collection, 0x01, collection.Count - 0x01);
            collection.Add(checkSum);

            //return frame as data array
            return collection.ToArray();
        }
        /// <summary>
        /// This function parse frame from data
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="action">Function to get frame item type</param>
        private void InternalParseFrame(Byte[] data, Func<UInt16, UInt32, PackageItemTypes> action)
        {
            //vriables
            UInt32 address = 0x00;
            UInt16 length = 0x00;
            Byte[] dataItem = null;
            PackageItemTypes type = PackageItemTypes.Unkown;
            int count = data.Length;
            
            //parse frame
            for (int i = 0; i < count; i++)
            {
                //read address
                address = (UInt32)(data[i + 3] << 24 | data[i + 2] << 16 | data[i + 1] << 8 | data[i]);
                length = (UInt16)(data[i + 5] << 8 | data[i + 4]);

                //read frame item type
                type = action != null ? action(this.Address, address) : PackageItemTypes.Unkown;
 
                //read data
                dataItem = new Byte[length];
                Buffer.BlockCopy(data, i + 6, dataItem, 0, length);

                //parse 
                IPackageItem item = this.InternalParseFrame(type, address, length, dataItem);
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
        private IPackageItem InternalParseFrame(PackageItemTypes type, UInt32 address, UInt16 length, Byte[] data)
        {
            switch (type)
            {
                case PackageItemTypes.DateTime:
                    return new PackageItemDateTime(address, data);
                case PackageItemTypes.Guid:
                    return new PackageItemGuid(address, data);
                case PackageItemTypes.String:
                    return new PackageItemString(address, data);
                case PackageItemTypes.Int16:
                    return new PackageItemInt16(address, data);
                case PackageItemTypes.Int32:
                    return new PackageItemInt32(address, data);
                case PackageItemTypes.Int64:
                    return new PackageItemInt64(address, data);
                case PackageItemTypes.Byte:
                    return new PackageItemByte(address, data);
                case PackageItemTypes.Boolean:
                    return new PackageItemBoolean(address, data);
                case PackageItemTypes.UInt64:
                    return new PackageItemUInt64(address, data);
                case PackageItemTypes.UInt32:
                    return new PackageItemUInt32(address, data);
                case PackageItemTypes.Enum:
                    return new PackageItemEnum(address, data);
                default:
                    return new PackageItemUnkown(address, data);
            }
        }
        #endregion
    }
}

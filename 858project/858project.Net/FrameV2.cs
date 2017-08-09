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
    public sealed class FrameV2 : IFrame
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Command address</param>
        /// <param name="state">State value</param>
        public FrameV2(UInt16 address, Byte state)
            : this(address, state, null, null)
        {
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Command address</param>
        /// <param name="state">State value</param>
        /// <param name="data">Frame data</param>
        /// <param name="action">Action for returning frame item type</param>
        public FrameV2(UInt16 address, Byte state, List<Byte> data, Func<UInt16, UInt16, FrameItemTypes> action)
        {
            this.Address = address;
            this.State = state;
            this.m_groups = new List<FrameGroupItem>();
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
        /// Frame state
        /// </summary>
        public Byte State
        {
            get;
            set;
        }
        /// <summary>
        /// Item collections
        /// </summary>
        public ReadOnlyCollection<FrameGroupItem> Groups
        {
            get { return this.m_groups.AsReadOnly(); }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Group collections
        /// </summary>
        private List<FrameGroupItem> m_groups = null;
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// Create frame from Byte value
        /// </summary>
        /// <param name="frameAddress">Frame address</param>
        /// <param name="state">Frate state</param>
        /// <param name="value">Value as Byte</param>
        /// <returns>New frame</returns>
        public static FrameV2 CreateFrame(UInt16 frameAddress, Byte state, Byte value)
        {
            FrameV2 frame = new FrameV2(frameAddress, state);
            frame.State = state;
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
        ///  Removes the first occurrence of a specific object
        /// </summary>
        /// <param name="group">The object to remove</param>
        public void Remove(FrameGroupItem group)
        {
            this.m_groups.Remove(group);
        }
        /// <summary>
        /// Adds an item to the end of the Frame
        /// </summary>
        /// <param name="group">The item to be added to the end of the Frame</param>
        public void Add(FrameGroupItem group)
        {
            this.m_groups.Add(group);
        }
        /// <summary>
        /// Inserts an item into the Frame at the specified index.
        /// </summary>
        /// <param name="group">The object to insert. The value can be null for reference types.</param>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        public void Add(FrameGroupItem group, int index)
        {
            this.m_groups.Insert(index, group);
        }
        /// <summary>
        /// Change frame address
        /// </summary>
        /// <param name="address">new address</param>
        public void ChangeAddress(UInt16 address)
        {
            this.Address = address;
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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            //initialize builder
            StringBuilder builder = new StringBuilder();

            //base information
            builder.Append("------------------");
            builder.AppendFormat("Address: {0}, Groups: {1}", this.Address, this.Groups.Count);
            builder.Append(Environment.NewLine);

            //items
            int groupCount = this.m_groups.Count;
            for (int i = 0; i < groupCount; i++)
            {
                FrameGroupItem group = this.m_groups[i];
                builder.Append("---------");
                builder.Append(Environment.NewLine);
                builder.AppendFormat("Group: {0}, Items: {1}", (i + 1), group.Count);
                builder.Append(Environment.NewLine);
                foreach (IFrameItem item in group.Items)
                {
                    builder.AppendFormat("[{0}] - [{1} - {1:x4}] - {2}", item.GetType(), item.Address, item.GetValue());
                    builder.Append(Environment.NewLine);
                }
            }

            //create end line
            builder.Append("------------------");

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
            Int16 length = 0x06;

            //add items
            foreach (FrameGroupItem item in this.m_groups)
            {
                Byte[] data = item.ToByteArray();
                length += (Int16)data.Length;
                collection.AddRange(data);
            }

            //add headers
            Byte[] header = new Byte[6];
            header[0] = 0x70;
            header[1] = (byte)(length);
            header[2] = (byte)(length >> 8);
            header[3] = (byte)(this.Address);
            header[4] = (byte)(this.Address >> 8);
            header[5] = this.State;
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
                TraceLogger.Info("{1}-{1:X2}", currentIndex, array[currentIndex]);
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
        private void InternalParseFrame(Byte[] data, Func<UInt16, UInt16, FrameItemTypes> action)
        {
            //vriables
            UInt16 address = 0x00;
            UInt16 length = 0x00;
            Byte[] dataItem = null;
            FrameItemTypes type = FrameItemTypes.Unkown;
            int count = data.Length;
            UInt16 itemCount = 0x00;
            int currentIndex = 0x00;

            //parse data
            while (currentIndex < count)
            {
                //check group value
                if (data[currentIndex++] != 0x67)
                {
                    throw new Exception("Data are not valid!");
                }

                //get count+
                itemCount = (UInt16)(data[currentIndex + 1] << 8 | data[currentIndex]);
                currentIndex += 0x02;

                //create group
                FrameGroupItem group = new FrameGroupItem();

                //parse items
                for (int i = 0; i < itemCount; i++)
                {
                    //read address
                    address = (UInt16)(data[currentIndex + 1] << 8 | data[currentIndex]);
                    length = (UInt16)(data[currentIndex + 3] << 8 | data[currentIndex + 2]);

                    //read frame item type
                    type = action != null ? action(this.Address, address) : FrameItemTypes.Unkown;

                    //read data
                    dataItem = new Byte[length];
                    Buffer.BlockCopy(data, currentIndex + 4, dataItem, 0, length);

                    //parse 
                    IFrameItem item = this.InternalParseFrame(type, address, length, dataItem);
                    if (item != null)
                    {
                        group.Add(item);
                    }

                    //next
                    currentIndex += 4 + length;
                }

                //add group to frame
                this.m_groups.Add(group);
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
                case FrameItemTypes.UInt32:
                    return new FrameItemUInt32(address, data);
                default:
                    return new FrameItemUnkown(address, data);
            }
        }
        #endregion
    }
}

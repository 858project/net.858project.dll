using Project858.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project858.Net
{
    /// <summary>
    /// One group for frame v2
    /// </summary>
    public sealed class FrameGroupItem
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Data adress for this group</param>
        public FrameGroupItem(UInt16 address)
        {
            this.m_items = new List<IPackageItem>();
            this.Address = address;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Data address
        /// </summary>
        public UInt16 Address
        {
            get;
            private set;
        }
        /// <summary>
        /// Command address
        /// </summary>
        public UInt16 Count
        {
            get { return Convert.ToUInt16(this.m_items.Count); }
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

        #region - Public Methods -
        /// <summary>
        ///  Removes the first occurrence of a specific object with address
        /// </summary>
        /// <param name="address">Specific address</param>
        public void Remove(UInt16 address)
        {
            int itemCount = this.m_items.Count;
            for (int i = 0; i < itemCount; i++)
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
                    Object value = item.GetValue();
                    try
                    {
                        //check type
                        if (typeof(T) == typeof(Object))
                        {
                            return (T)value;
                        }
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException(String.Format("Value: {0}, Type: {1}, Item: {2}", (value == null ? "NULL" : value.ToString()), typeof(T), item), ex);
                    }
                }
            }
            return default(T);
        }
        /// <summary>
        /// This function returns item data
        /// </summary>
        /// <returns>Item array data</returns>
        public Byte[] ToByteArray()
        {
            //create collection
            List<Byte> collection = new List<Byte>();

            //add headers
            Byte[] header = new Byte[5];
            header[0] = 0x67;
            int count = this.m_items.Count;
            header[1] = (byte)(this.Address);
            header[2] = (byte)(this.Address >> 8);
            header[3] = (byte)(count);
            header[4] = (byte)(count >> 8);
            collection.InsertRange(0, header);
 
            //add items
            foreach (IPackageItem item in this.m_items)
            {
                Byte[] data = item.ToByteArray();
                collection.AddRange(data);
            }

            //return collection
            return collection.ToArray();
        }
        #endregion
    }
}

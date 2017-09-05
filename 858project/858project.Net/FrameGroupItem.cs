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
        public FrameGroupItem()
        {
            this.m_items = new List<IFrameItem>();
        }
        #endregion

        #region - Properties -
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
        ///  Removes the first occurrence of a specific object with address
        /// </summary>
        /// <param name="address">Specific address</param>
        public void Remove(UInt16 address)
        {
            int itemCount = this.m_items.Count;
            for (int i = 0; i < itemCount; i++)
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
            Byte[] header = new Byte[3];
            header[0] = 0x67;
            int count = this.m_items.Count;
            header[1] = (byte)(count);
            header[2] = (byte)(count >> 8);
            collection.InsertRange(0, header);
 
            //add items
            foreach (IFrameItem item in this.m_items)
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

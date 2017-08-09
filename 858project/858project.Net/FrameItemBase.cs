using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// Base frame item from tcp protocol
    /// </summary>
    public abstract class FrameItemBase<T> : IFrameItem
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemBase(UInt32 address, T value)
        {
            try
            {
                this.Address = address;
                this.Data = this.InternalParseFromValue(value);
                this.Value = value;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Frame item 0x{0:X4} failed [Value: {1}].", address, value), ex);
            }
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemBase(UInt32 address, Byte[] data)
        {
            try
            {
                this.Data = data;
                this.Value = this.InternalParseValue(data);
                this.Address = address;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Frame item 0x{0:X4} failed [Data Length: {1}].", address, data.Length), ex);
            }
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public abstract FrameItemTypes ItemType { get; }
        /// <summary>
        /// Generics value
        /// </summary>
        public T Value { get; protected set; }
        /// <summary>
        /// Item address
        /// </summary>
        public UInt32 Address { get; protected set; }
        /// <summary>
        /// Data item
        /// </summary>
        public Byte[] Data { get; protected set; }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// This method changes value to other
        /// </summary>
        /// <param name="value">New value</param>
        public void ChangeValue(T value)
        {
            if (value != null)
            {
                this.Value = value;
            }
        }
        /// <summary>
        /// This function returns value
        /// </summary>
        /// <returns></returns>
        public Object GetValue()
        {
            return this.Value;
        }
        /// <summary>
        /// This function returns item data
        /// </summary>
        /// <returns>Item array data</returns>
        public Byte[] ToByteArray()
        {
            //initialize data
            int length = this.Data.Length;
            Byte[] result = new Byte[length + 6];

            //address
            result[0] = (Byte)(this.Address);
            result[1] = (Byte)(this.Address >> 8);
            result[2] = (Byte)(this.Address >> 16);
            result[3] = (Byte)(this.Address >> 24);

            //length
            result[4] = (Byte)(length);
            result[5] = (Byte)(length >> 8);

            //data
            Buffer.BlockCopy(this.Data, 0, result, 6, length);

            //return result
            return result;
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected abstract T InternalParseValue(Byte[] data);
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected abstract Byte[] InternalParseFromValue(T value);
        #endregion

        #region - Public Static Methods -
        /// <summary>
        /// def GET_BITS_VAL( data, bh, bl )
        /// brief Makro vrati hodnotu z \b data, ktora je definovana rozsahom bitov \b bl az \b bh. \n
        /// Nizsi bit: bl, vyssi bit: bh. \n
        /// Pre hodnotu data: 0x1234 a rozsah bitov bl: 4 bh: 7 makro vrati hodnotu 0x3. \n
        /// see GET_BITS, GET_BIT, SET_BITS_VAL
        /// <param name="data"></param>
        /// <param name="bh">horny bit</param>
        /// <param name="bl">dolny bit</param>
        /// <returns>int</returns>
        public static int GET_BITS_VAL(UInt32 data, int bh, int bl)
        {
            return (int)(GET_BITS(data, bh, bl) >> (bl));
        }
        /// <summary>
        /// def GET_BITS_VAL( data, bh, bl )
        /// brief Makro vrati hodnotu z \b data, ktora je definovana rozsahom bitov \b bl az \b bh. \n
        /// Nizsi bit: bl, vyssi bit: bh. \n
        /// Pre hodnotu data: 0x1234 a rozsah bitov bl: 4 bh: 7 makro vrati hodnotu 0x3. \n
        /// see GET_BITS, GET_BIT, SET_BITS_VAL
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bh">horny bit</param>
        /// <param name="bl">dolny bit</param>
        /// <returns>int</returns>
        public static int GET_BITS_VAL(byte data, int bh, int bl)
        {
            return (int)(GET_BITS(data, bh, bl) >> (bl));
        }
        /// <summary>
        /// def GET_BITS( data, bh, bl )
        /// brief Makro vrati hodnotu data, ktora je v reprezentovana len v oblasti definovanej rozsahom bitov \b bl az \b bh. \n
        /// Nizsi bit: bl, vyssi bit: bh. \n
        /// Pre hodnotu data: 0xABCD a rozsah bitov bl: 4 bh: 7 makro vrati hodnotu: 0x00C0
        /// see BITS_VAL, SET_BITS_VAL, GET_BITS_VAL, BITS_MASK
        /// </summary>
        /// <param name="data">byte</param>
        /// <param name="bh">hodny bit</param>
        /// <param name="bl">dolny bit</param>
        /// <returns></returns>
        public static int GET_BITS(byte data, int bh, int bl)
        {
            return (int)((data) & BITS_MASK(bh, bl));
        }
        /// <summary>
        /// def GET_BITS( data, bh, bl )
        /// brief Makro vrati hodnotu data, ktora je v reprezentovana len v oblasti definovanej rozsahom bitov \b bl az \b bh. \n
        /// Nizsi bit: bl, vyssi bit: bh. \n
        /// Pre hodnotu data: 0xABCD a rozsah bitov bl: 4 bh: 7 makro vrati hodnotu: 0x00C0
        /// see BITS_VAL, SET_BITS_VAL, GET_BITS_VAL, BITS_MASK
        /// </summary>
        /// <param name="data">byte</param>
        /// <param name="bh">hodny bit</param>
        /// <param name="bl">dolny bit</param>
        /// <returns></returns>
        public static int GET_BITS(UInt32 data, int bh, int bl)
        {
            return (int)(((data) & BITS_MASK(bh, bl)));
        }
        /// <summary>
        /// def BITS_MASK( bh, bl )
        /// brief Makro vytvori bitovu masku, definovanu rozsahom bitov \b bl az \b bh. \n
        /// Nizsi bit: bl, vyssi bit: bh. Pre bl: 1 a bh: 3 bude maska: 0x0E.
        /// see BIT
        /// </summary>
        /// <param name="bh"></param>
        /// <param name="bl"></param>
        /// <returns></returns>
        public static int BITS_MASK(int bh, int bl)
        {
            return (int)((((1L << ((bh) - (bl) + 1)) - 1) << (bl)));
        }
        /// <summary>
        /// Vrati hodnotu bitov v rozsahu horny a dolny bit
        /// </summary>
        /// <param name="bh">Horny bit</param>
        /// <param name="bl">dolnt bit/param>
        /// <param name="val">hodnota</param>
        /// <returns>Hodnota bitov</returns>
        public static int BITS_VAL(int bh, int bl, int val)
        {
            return (int)(((val) << (bl)) & BITS_MASK(bh, bl));
        }
        #endregion
    }
}

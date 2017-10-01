using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project858;

namespace Project858.Net
{
    /// <summary>
    /// Frame item with datetime value
    /// </summary>
    public sealed class FrameItemDateTime : FrameItemBase<DateTime>
    {
        #region - Constructors -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="value">Value</param>
        public FrameItemDateTime(UInt32 address, DateTime value)
            : base(address, value)
        {
 
        }
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="address">Item address</param>
        /// <param name="data">Byte array</param>
        public FrameItemDateTime(UInt32 address, Byte[] data)
            : base(address, data)
        {
 
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Item type
        /// </summary>
        public override FrameItemTypes ItemType { get { return FrameItemTypes.DateTime; } }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("[DateTime] : 0x{0:X4} = {1}", this.Address, this.Value.ToIso8601String());
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This function parse value from byt array
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Value</returns>
        protected override DateTime InternalParseValue(byte[] data)
        {
            int tmp = (int)(data[0] | (data[1] << 8));
            var tm_msec = tmp % 1000;
            var tm_sec = tmp / 1000;
            var tm_min = GET_BITS_VAL(data[2], 5, 0);
            var tm_hour = GET_BITS_VAL(data[3], 4, 0);
            var tm_wday = GET_BITS_VAL(data[4], 7, 5);
            var tm_mday = GET_BITS_VAL(data[4], 4, 0);
            var tm_mon = GET_BITS_VAL(data[5], 3, 0);
            var tm_year = GET_BITS_VAL(data[6], 6, 0);

            if (tm_sec > 59 || tm_min > 59 || tm_hour > 23 || tm_wday < 0 || tm_wday > 7 ||
                tm_mday < 1 || tm_mday > 31 || tm_mon < 1 || tm_mon > 12 || tm_year > 99)
            {
                throw new Exception("Data is not valid!");
            }

            return new DateTime(tm_year + 2000, tm_mon, tm_mday, tm_hour, tm_min, tm_sec, tm_msec, DateTimeKind.Utc);
        }
        /// <summary>
        /// This function parse byt array from value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Byte array</returns>
        protected override byte[] InternalParseFromValue(DateTime value)
        {
            //read date time items
            var tm_msec = value.Millisecond;
            var tm_sec = value.Second;
            var tm_min = value.Minute;
            var tm_hour = value.Hour;
            var tm_mday = value.Day;
            var tm_mon = value.Month;
            var tm_year = value.Year % 100;
            var tm_wday = value.DayOfWeek != 0 ? (int)value.DayOfWeek : 7;

            //convert date item to byte
            Byte[] data = new Byte[7];
            uint tmp = (uint)tm_msec + (uint)tm_sec * 1000;
            data[0] = ((byte)(GET_BITS_VAL(tmp, 7, 0)));
            data[1] = ((byte)(GET_BITS_VAL(tmp, 15, 8)));
            data[2] = (byte)((byte)tm_min | 0x00);
            data[3] = ((byte)tm_hour);
            tmp = (uint)(BITS_VAL(7, 5, tm_wday) | BITS_VAL(4, 0, tm_mday));
            data[4] = ((byte)tmp);
            data[5] = ((byte)tm_mon);
            data[6] = ((byte)tm_year);

            //return byte array result
            return data;
        }
        #endregion
    }
}

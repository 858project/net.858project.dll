using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Net
{
    /// <summary>
    /// Frame item types
    /// </summary>
    public enum FrameItemTypes
    {
        /// <summary>
        /// Date Time
        /// </summary>
        DateTime,
        /// <summary>
        /// Guid / UUID
        /// </summary>
        Guid,
        /// <summary>
        /// UTF-8 string
        /// </summary>
        String,
        /// <summary>
        /// Bool hodnota
        /// </summary>
        Boolean,
        /// <summary>
        /// Bytova hodnota
        /// </summary>
        Byte,
        /// <summary>
        /// Short value
        /// </summary>
        Int16,
        /// <summary>
        /// Int value
        /// </summary>
        Int32,
        /// <summary>
        /// Long value
        /// </summary>
        Int64,
        /// <summary>
        /// Unsigned short value
        /// </summary>
        UInt16,
        /// <summary>
        /// Unsigned int value
        /// </summary>
        UInt32,
        /// <summary>
        /// Unsigned long value
        /// </summary>
        UInt64,
        /// <summary>
        /// Enum type
        /// </summary>
        Enum,
        /// <summary>
        /// Unkown type
        /// </summary>
        Unkown,
    }
}

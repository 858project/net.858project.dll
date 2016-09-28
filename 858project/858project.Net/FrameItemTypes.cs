﻿using System;
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
        /// Unkown type
        /// </summary>
        Unkown,
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Net
{
    /// <summary>
    /// Element na definiciu adresy skupiny
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class FrameGroupAttribute : Attribute
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public FrameGroupAttribute()
        {
            this.Address = 0x0000;
        }
        #endregion
 
        #region - Properties -
        /// <summary>
        /// Hodnota adresy
        /// </summary>
        public UInt16 Address { get; set; }
        #endregion
    }
}

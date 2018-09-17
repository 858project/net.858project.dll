using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Project858.Net
{
    /// <summary>
    /// Element na definiciu adresy tagu
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class FrameItemAttribute : Attribute
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public FrameItemAttribute()
        {
            this.Type = PackageItemTypes.Unkown;
            this.Address = 0x0000;
        }
        #endregion
 
        #region - Properties -
        /// <summary>
        /// Typ objektu
        /// </summary>
        public PackageItemTypes Type { get; set; }
        /// <summary>
        /// Hodnota adresy
        /// </summary>
        public UInt32 Address { get; set; }
        #endregion
    }
}

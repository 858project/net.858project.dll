using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.Web
{
    /// <summary>
    /// Typ pouzitelny v drop down liste
    /// </summary>
    public interface ISelectListItem
    {
        /// <summary>
        /// Gets or sets the text of the selected item.
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// Gets or sets the value of the selected item.
        /// </summary>
        string Value { get; set; }
    }
}

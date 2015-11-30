using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project858.ComponentModel
{
    /// <summary>
    /// Model definujuci vzhlad objektov prenasajucich dat
    /// </summary>
    public interface IModel
    {
        #region - Public Methods -
        /// <summary>
        /// Overi spravnost modelu
        /// </summary>
        /// <returns>True = model je spravny, inak false</returns>
        Boolean Validate();
        #endregion
    }
}

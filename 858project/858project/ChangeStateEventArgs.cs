using System;
using System.Collections.Generic;
using System.Text;

namespace Project858
{
    /// <summary>
    /// EventArgs ako sucast event na oznamenie zmeny stav
    /// </summary>
    public class ChangeStateEventArgs
    {
        #region - Variable -
        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="state">Aktualny stav</param>
        public ChangeStateEventArgs(Boolean state)
        {
            this._state = state;
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Aktualny stav
        /// </summary>
        public Boolean State
        {
            get { return _state; }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Aktualny stav
        /// </summary>
        private Boolean _state = false;
        #endregion
    }
}

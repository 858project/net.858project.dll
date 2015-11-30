using System;
using System.Collections.Generic;
using System.Text;

namespace Project858
{
    /// <summary>
    /// Delegat ako predpis pre evnet oznamujuci zmenu stav
    /// </summary>
    /// <param name="sender">Odosielatel udalosti</param>
    /// <param name="e">ChangeStateEventArgs</param>
    public delegate void ChangeStateEventHandler(Object sender, ChangeStateEventArgs e);
}
